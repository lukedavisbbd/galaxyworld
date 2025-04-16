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
            AnsiConsole.WriteLine($"No. Stars: {planetarySystem.NumStars}");
            AnsiConsole.WriteLine($"No. Planets: {planetarySystem.NumPlanets}");
            AnsiConsole.WriteLine($"No. Moons: {planetarySystem.NumMoons}");

            foreach (var planet in planetarySystem.Planets)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"* [bold]{planet.PlanetName}[/] [grey]({planet.SolutionType})[/]");
                if (planet.Controversial)
                    AnsiConsole.WriteLine("[yellow]Discovery Disputed[/]");
                AnsiConsole.WriteLine($"Discovery Method: {planet.DiscoveryMethod}");
                AnsiConsole.WriteLine($"Discovery Year: {planet.DiscYear}");
                AnsiConsole.WriteLine($"Discovery Facility: {planet.DiscFacility}");
                AnsiConsole.WriteLine($"Discovery Telescope: {planet.DiscTelescope}");
                AnsiConsole.MarkupLine($"Radius (Earth Radii): {FormatNullable(planet.RadiusEarth)}");
                AnsiConsole.MarkupLine($"Radius (Jupiter Radii): {FormatNullable(planet.RadiusJupiter)}");
                AnsiConsole.MarkupLine($"Mass (Earth Masses): {FormatNullable(planet.MassEarth)}");
                AnsiConsole.MarkupLine($"Mass (Jupiter Masses): {FormatNullable(planet.MassJupiter)}");

            }

            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get constellation."}[/]");
            return 1;
        }
    }

    static string FormatNullable<T>(T? value)
    {
        return value == null ? "[grey]unknown[/]" : value.ToString() ?? "";
    }
}