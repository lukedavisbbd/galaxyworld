using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using Spectre.Console;
using Spectre.Console.Testing;
using GalaxyWorld.Cli.Commands.Catalogue;
using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Cli.Exceptions;

public class FakeApiClient_Catalogues
{
    public Func<int, int, CatalogueSort, Filter<Catalogue>[]?, Task<List<Catalogue>>>? OnGetCatalogues;

    public Task<List<Catalogue>> GetCatalogues(int start, int length, CatalogueSort sort, Filter<Catalogue>[]? filters)
    {
        return OnGetCatalogues?.Invoke(start, length, sort, filters) ?? Task.FromResult(new List<Catalogue>());
    }
}

public class GetAllCataloguesCommandShim : AsyncCommand<GetAllCataloguesCommand.Settings>
{
    private readonly FakeApiClient_Catalogues _client;
    private readonly IAnsiConsole _console;

    public GetAllCataloguesCommandShim(FakeApiClient_Catalogues client, IAnsiConsole console)
    {
        _client = client;
        _console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, GetAllCataloguesCommand.Settings settings)
    {
        try
        {
            var page = int.Max(settings.Page, 1);
            var length = int.Max(settings.Length, 1);
            var filters = (settings.Filter ?? []).Select(filter => Filter<Catalogue>.Parse(filter, null)).ToArray();

            var catalogues = await _client.GetCatalogues((page - 1) * length, length, settings.Sort, filters);

            if (catalogues.Count == 0)
            {
                _console.MarkupLine("[yellow]No catalogues found.[/]");
                return 0;
            }

            var table = new Table().Title($"[bold]Catalogues (Sorted by {settings.Sort})[/]").AddColumns("ID", "Name", "Slug");

            foreach (var cat in catalogues)
            {
                table.AddRow(cat.CatId.ToString(), cat.CatName, cat.CatSlug);
            }

            _console.Write(table);
            _console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
            return 0;
        }
        catch (AppException e)
        {
            _console.MarkupLine($"[red]{e.Message ?? "No catalogues found."}[/]");
            return 1;
        }
    }
}

public class GetAllCataloguesCommandTests
{
    [Fact]
    public async Task Should_PrintCatalogueTable_WhenCataloguesExist()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_Catalogues
        {
            OnGetCatalogues = (_, _, _, _) => Task.FromResult(new List<Catalogue>
            {
                new Catalogue { CatId = 1, CatName = "Hipparcos", CatSlug = "hip" },
                new Catalogue { CatId = 2, CatName = "Tycho", CatSlug = "tyc" }
            })
        };

        var command = new GetAllCataloguesCommandShim(fakeClient, testConsole);
        var settings = new GetAllCataloguesCommand.Settings
        {
            Page = 1,
            Length = 20,
            Sort = CatalogueSort.CatName,
            Filter = []
        };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("Hipparcos", output);
        Assert.Contains("Tycho", output);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Should_PrintNoCatalogues_WhenListIsEmpty()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_Catalogues
        {
            OnGetCatalogues = (_, _, _, _) => Task.FromResult(new List<Catalogue>())
        };

        var command = new GetAllCataloguesCommandShim(fakeClient, testConsole);
        var settings = new GetAllCataloguesCommand.Settings
        {
            Page = 1,
            Length = 10,
            Sort = CatalogueSort.CatSlug,
            Filter = []
        };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("No catalogues found", output);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Should_PrintError_WhenAppExceptionThrown()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_Catalogues
        {
            OnGetCatalogues = (_, _, _, _) => throw new AppException("Backend explosion 💥")
        };

        var command = new GetAllCataloguesCommandShim(fakeClient, testConsole);
        var settings = new GetAllCataloguesCommand.Settings
        {
            Page = 1,
            Length = 5,
            Sort = CatalogueSort.CatName,
            Filter = []
        };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("Backend explosion", output);
        Assert.Equal(1, result);
    }
}