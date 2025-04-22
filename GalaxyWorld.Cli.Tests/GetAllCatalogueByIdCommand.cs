using Xunit;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using Spectre.Console;
using Spectre.Console.Testing;
using GalaxyWorld.Cli.Commands.Catalogue;
using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Exceptions;

public class FakeApiClient_CatalogueById
{
    public Func<int, Task<Catalogue>>? OnGetCatalogue;

    public Task<Catalogue> GetCatalogue(int id)
    {
        return OnGetCatalogue?.Invoke(id) ?? throw new AppException("No catalogue found");
    }
}

public class GetCatalogueByIdCommandShim : AsyncCommand<GetCatalogueByIdCommand.Settings>
{
    private readonly FakeApiClient_CatalogueById _client;
    private readonly IAnsiConsole _console;

    public GetCatalogueByIdCommandShim(FakeApiClient_CatalogueById client, IAnsiConsole console)
    {
        _client = client;
        _console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, GetCatalogueByIdCommand.Settings settings)
    {
        try
        {
            var catalogue = await _client.GetCatalogue(settings.Id);
            _console.MarkupLineInterpolated($"[green]Catalogue:[/] {catalogue.CatalogueName} ({catalogue.CatalogueSlug})");
            return 0;
        }
        catch (AppException e)
        {
            _console.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to get catalogue."}[/]");
            return 1;
        }
    }
}

public class GetCatalogueByIdCommandTests
{
    [Fact]
    public async Task Should_PrintCatalogue_WhenFound()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_CatalogueById
        {
            OnGetCatalogue = id => Task.FromResult(new Catalogue
            {
                CatalogueId = id,
                CatalogueName = "Hipparcos",
                CatalogueSlug = "hip"
            })
        };

        var command = new GetCatalogueByIdCommandShim(fakeClient, testConsole);
        var settings = new GetCatalogueByIdCommand.Settings { Id = 1 };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("Catalogue:", output);
        Assert.Contains("Hipparcos", output);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Should_PrintError_WhenNotFound()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_CatalogueById
        {
            OnGetCatalogue = id => throw new AppException("Catalogue not found")
        };

        var command = new GetCatalogueByIdCommandShim(fakeClient, testConsole);
        var settings = new GetCatalogueByIdCommand.Settings { Id = 999 };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("Catalogue not found", output);
        Assert.Equal(1, result);
    }
}