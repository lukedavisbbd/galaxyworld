using Xunit;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Cli.Commands.Star;
using System.Collections.Generic;

public class FakeApiClient
{
    public Task<Star> GetStar(int id) => Task.FromResult(new Star
    {
        StarId = id,
        ProperName = "Vega",
        Constellation = 1,
        RightAscension = 14.73,
        Declination = 19.18,
        PosSrc = "Hip",
        X0 = 0.1,
        Y0 = 0.2,
        Z0 = 0.3,
        Distance = 7.68,
        DistanceSrc = "Hip",
        AbsoluteMagnitude = 0.5,
        ColourIndex = 0.3,
        Magnitude = 0.03,
        MagnitudeSrc = "Vis",
        RadialVelocity = -20.2,
        RadialVelocitySrc = "GAIA",
        ProperMotionRightAscension = 0.1,
        ProperMotionDeclination = 0.1,
        ProperMotionSrc = "GAIA",
        VelocityX = 0.1,
        VelocityY = 0.1,
        VelocityZ = 0.1,
        SpectralType = "A0V",
        SpectralTypeSrc = "SimBad",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    });

    public Task<List<Catalogue>> GetCatalogues() => Task.FromResult(new List<Catalogue>
    {
        new Catalogue { CatalogueId = 1, CatalogueName = "Hipparcos", CatalogueSlug = "hip" },
        new Catalogue { CatalogueId = 2, CatalogueName = "Tycho", CatalogueSlug = "tycho" }
    });

    public Task<List<CatalogueEntry>> GetStarCatalogueEntries(int starId, int start, int length)
    {
        return Task.FromResult(new List<CatalogueEntry>
        {
            new CatalogueEntry
            {
                EntryId = "101",
                EntryDesignation = "H001",
                StarId = starId,
                CatalogueId = 1
            },
            new CatalogueEntry
            {
                EntryId = "202",
                EntryDesignation = "T202",
                StarId = starId,
                CatalogueId = 2
            }
        });
    }
}

public class GetStarByIdCommandShim : AsyncCommand<GetStarByIdCommand.Settings>
{
    private readonly FakeApiClient _client;
    private readonly IAnsiConsole _console;

    public GetStarByIdCommandShim(FakeApiClient client, IAnsiConsole console)
    {
        _client = client;
        _console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, GetStarByIdCommand.Settings settings)
    {
        try
        {
            var star = await _client.GetStar(settings.Id);

            _console.MarkupLineInterpolated($"[green]Star: {star.ProperName}[/]");

            var catalogues = await _client.GetCatalogues();
            var entries = await _client.GetStarCatalogueEntries(settings.Id, 0, 999);

            _console.MarkupLine("[bold]Catalogue Entries:[/]");
            foreach (var entry in entries)
            {
                var catalogue = catalogues.First(cat => cat.CatalogueId == entry.CatalogueId);
                _console.MarkupLineInterpolated($"[yellow]{catalogue.CatalogueName}:[/] {entry.EntryId} {entry.EntryDesignation}");
            }

            return 0;
        }
        catch (Exception ex)
        {
            _console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
            return 1;
        }
    }
}

public class GetStarByIdCommandTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPrintCatalogueEntries_WhenDataExists()
    {
        // Arrange
        var fakeClient = new FakeApiClient();
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var command = new GetStarByIdCommandShim(fakeClient, testConsole);
        var settings = new GetStarByIdCommand.Settings { Id = 123 };

        // Act
        int result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        // Assert
        Assert.Contains("Catalogue Entries", output);
        Assert.Contains("Hipparcos", output);
        Assert.Contains("Tycho", output);
        Assert.Equal(0, result);
    }
}