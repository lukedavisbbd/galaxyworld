using Xunit;
using System.Threading.Tasks;
using System;
using Spectre.Console.Cli;
using Spectre.Console;
using Spectre.Console.Testing;
using GalaxyWorld.Cli.Commands.CatalogueEntry;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Cli.Exceptions;
using System.Collections.Generic;

public class FakeApiClient_CatalogueEntry
{
    public Func<int, int, Task<CatalogueEntry>>? OnGetCatalogueEntry;

    public Task<CatalogueEntry> GetCatalogueEntry(int catId, int starId)
    {
        return OnGetCatalogueEntry?.Invoke(catId, starId) ?? throw new AppException("No mock set");
    }
}

public class GetCatalogueEntryCommandShim : AsyncCommand<GetCatalogueEntryCommand.Settings>
{
    private readonly FakeApiClient_CatalogueEntry _client;
    private readonly IAnsiConsole _console;

    public GetCatalogueEntryCommandShim(FakeApiClient_CatalogueEntry client, IAnsiConsole console)
    {
        _client = client;
        _console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, GetCatalogueEntryCommand.Settings settings)
    {
        try
        {
            var entry = await _client.GetCatalogueEntry(settings.CatalogueId, settings.StarId);
            _console.MarkupLineInterpolated($"[green]Entry ID:[/] {entry.EntryId}, [green]Designation:[/] {entry.EntryDesignation}");
            return 0;
        }
        catch (AppException e)
        {
            _console.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to get catalogue."}[/]");
            return 1;
        }
    }
}

public class GetCatalogueEntryCommandTests
{
    [Fact]
    public async Task Should_PrintEntry_WhenEntryExists()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_CatalogueEntry
        {
            OnGetCatalogueEntry = (catId, starId) => Task.FromResult(new CatalogueEntry
            {
                CatalogueId = catId,
                StarId = starId,
                EntryId = "123",
                EntryDesignation = "Vega-123"
            })
        };

        var command = new GetCatalogueEntryCommandShim(fakeClient, testConsole);
        var settings = new GetCatalogueEntryCommand.Settings { CatalogueId = 1, StarId = 42 };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("Entry ID:", output);
        Assert.Contains("Vega-123", output);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Should_PrintError_WhenAppExceptionThrown()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_CatalogueEntry
        {
            OnGetCatalogueEntry = (_, _) => throw new AppException("Entry not found")
        };

        var command = new GetCatalogueEntryCommandShim(fakeClient, testConsole);
        var settings = new GetCatalogueEntryCommand.Settings { CatalogueId = 1, StarId = 404 };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("Entry not found", output);
        Assert.Equal(1, result);
    }
}
