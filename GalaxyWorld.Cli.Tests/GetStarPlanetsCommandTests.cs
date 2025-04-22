using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using Spectre.Console;
using Spectre.Console.Testing;
using GalaxyWorld.Cli.Commands.Star;
using GalaxyWorld.Core.Models.Planets;
using GalaxyWorld.Cli.Exceptions;

public class FakeApiClient_Planets
{
    public Func<int, Task<PlanetarySystem?>>? OnGetStarPlanets;

    public Task<PlanetarySystem?> GetStarPlanets(int id)
    {
        return OnGetStarPlanets?.Invoke(id) ?? Task.FromResult<PlanetarySystem?>(null);
    }
}

public class GetStarPlanetsCommandShim : AsyncCommand<GetStarPlanetsCommand.Settings>
{
    private readonly FakeApiClient_Planets _client;
    private readonly IAnsiConsole _console;

    public GetStarPlanetsCommandShim(FakeApiClient_Planets client, IAnsiConsole console)
    {
        _client = client;
        _console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, GetStarPlanetsCommand.Settings settings)
    {
        try
        {
            var planetarySystem = await _client.GetStarPlanets(settings.Id);

            if (planetarySystem is null)
            {
                _console.MarkupLine("[yellow]No information found.[/]");
                return 0;
            }

            _console.MarkupLine("[bold]Planetary System[/]");
            _console.WriteLine($"No. Stars: {planetarySystem.SystemNumStars}");
            _console.WriteLine($"No. Planets: {planetarySystem.SystemNumPlanets}");
            _console.WriteLine($"No. Moons: {planetarySystem.SystemNumMoons}");

            foreach (var planet in planetarySystem.Planets)
            {
                _console.WriteLine();
                _console.MarkupLineInterpolated($"* [bold]{planet.PlanetName}[/] [grey]({planet.SolutionType})[/]");
                if (planet.Controversial)
                    _console.MarkupLine("[yellow]Discovery Disputed[/]");
                _console.WriteLine($"Discovery Method: {planet.DiscoveryMethod}");
                _console.WriteLine($"Discovery Year: {planet.DiscoveryYear}");
                _console.WriteLine($"Discovery Facility: {planet.DiscoveryFacility}");
                _console.WriteLine($"Discovery Telescope: {planet.DiscoveryTelescope}");
                _console.MarkupLine($"Radius (Earth Radii): {FormatNullable(planet.RadiusEarth)}");
                _console.MarkupLine($"Radius (Jupiter Radii): {FormatNullable(planet.RadiusJupiter)}");
                _console.MarkupLine($"Mass (Earth Masses): {FormatNullable(planet.MassEarth)}");
                _console.MarkupLine($"Mass (Jupiter Masses): {FormatNullable(planet.MassJupiter)}");
            }

            return 0;
        }
        catch (AppException e)
        {
            _console.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to get constellation."}[/]");
            return 1;
        }
    }

    private static string FormatNullable<T>(T? value) where T : struct
    {
        return value == null ? "[grey]unknown[/]" : value.ToString() ?? "";
    }
}

public class GetStarPlanetsCommandTests
{
    [Fact]
    public async Task Should_PrintSystem_WhenDataExists()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_Planets
        {
            OnGetStarPlanets = id => Task.FromResult<PlanetarySystem?>(new PlanetarySystem
            {
                SystemNumStars = 1,
                SystemNumPlanets = 2,
                SystemNumMoons = 5,
                Planets = new List<Planet>
                {
                    new Planet
                    {
                        PlanetName = "TestPlanet",
                        SolutionType = "Confirmed",
                        Controversial = true,
                        DiscoveryMethod = "Transit",
                        DiscoveryYear = 2020,
                        DiscoveryFacility = "SpaceLab",
                        DiscoveryTelescope = "SkyEye",
                        RadiusEarth = 1.5m,
                        RadiusJupiter = null,
                        MassEarth = 5.9m,
                        MassJupiter = null
                    }
                }
            })
        };

        var command = new GetStarPlanetsCommandShim(fakeClient, testConsole);
        var settings = new GetStarPlanetsCommand.Settings { Id = 123 };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("Planetary System", output);
        Assert.Contains("TestPlanet", output);
        Assert.Contains("Discovery Disputed", output);
        Assert.Contains("Radius (Earth Radii): 1.5", output);
        Assert.Contains("Mass (Earth Masses): 5.9", output);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Should_PrintNoInfo_WhenNullReturned()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_Planets
        {
            OnGetStarPlanets = id => Task.FromResult<PlanetarySystem?>(null)
        };

        var command = new GetStarPlanetsCommandShim(fakeClient, testConsole);
        var settings = new GetStarPlanetsCommand.Settings { Id = 999 };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("No information found", output);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Should_PrintError_WhenAppExceptionThrown()
    {
        var testConsole = new TestConsole();
        AnsiConsole.Console = testConsole;

        var fakeClient = new FakeApiClient_Planets
        {
            OnGetStarPlanets = id => throw new AppException("Simulated failure")
        };

        var command = new GetStarPlanetsCommandShim(fakeClient, testConsole);
        var settings = new GetStarPlanetsCommand.Settings { Id = 42 };

        var result = await command.ExecuteAsync((CommandContext?)null, settings);
        var output = testConsole.Output;

        Assert.Contains("Simulated failure", output);
        Assert.Equal(1, result);
    }
}
