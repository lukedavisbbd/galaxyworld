using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Star;

public class GetStarPlanetsCommand : AsyncCommand<GetStarPlanetsCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int Id { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var planetarySystem = await client.GetStarPlanets(settings.Id);

            if (planetarySystem is null)
            {
                AnsiConsole.MarkupLine("[yellow]No information found.[/]");
                return 0;
            }

            AnsiConsole.MarkupLine("[bold]Planetary System[/]");
            AnsiConsole.WriteLine($"No. Stars: {planetarySystem.SystemNumStars}");
            AnsiConsole.WriteLine($"No. Planets: {planetarySystem.SystemNumPlanets}");
            AnsiConsole.WriteLine($"No. Moons: {planetarySystem.SystemNumMoons}");

            foreach (var planet in planetarySystem.Planets)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLineInterpolated($"* [bold]{planet.PlanetName}[/] [grey]({planet.SolutionType})[/]");
                if (planet.Controversial)
                    AnsiConsole.WriteLine("[yellow]Discovery Disputed[/]");
                AnsiConsole.WriteLine($"Discovery Method: {planet.DiscoveryMethod}");
                AnsiConsole.WriteLine($"Discovery Year: {planet.DiscoveryYear}");
                AnsiConsole.WriteLine($"Discovery Facility: {planet.DiscoveryFacility}");
                AnsiConsole.WriteLine($"Discovery Telescope: {planet.DiscoveryTelescope}");
                AnsiConsole.WriteLine($"Radius (Earth Radii): {FormatNullable(planet.RadiusEarth)}");
                AnsiConsole.WriteLine($"Radius (Jupiter Radii): {FormatNullable(planet.RadiusJupiter)}");
                AnsiConsole.WriteLine($"Mass (Earth Masses): {FormatNullable(planet.MassEarth)}");
                AnsiConsole.WriteLine($"Mass (Jupiter Masses): {FormatNullable(planet.MassJupiter)}");

            }

            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to get constellation."}[/]");
            return 1;
        }
    }

    static string FormatNullable<T>(T? value)
    {
        return value == null ? "[grey]unknown[/]" : value.ToString() ?? "";
    }
}