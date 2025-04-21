using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models.Planets;
namespace GalaxyWorld.Cli.Commands.Stars;

public class GetStarPlanetsCommand : Command<GetStarPlanetsCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int Id { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var planetarySystem = client.GetAsync<PlanetarySystem>($"/stars/{settings.Id}/planets").Result;

            if (planetarySystem is null)
            {
                AnsiConsole.MarkupLine("[yellow]No information found.[/]");
                return 0;
            }

            AnsiConsole.MarkupLine("[bold green]Planetary System[/]");
            AnsiConsole.MarkupLine($"[grey]No. Stars:[/] {planetarySystem.NumStars}");
            AnsiConsole.MarkupLine($"[grey]No. Planets:[/] {planetarySystem.NumPlanets}");
            AnsiConsole.MarkupLine($"[grey]No. Moons:[/] {planetarySystem.NumMoons}");

            foreach (var planet in planetarySystem.Planets)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"* [green]{planet.PlanetName}[/] [grey]({planet.SolutionType})[/]");
                if (planet.Controversial)
                    AnsiConsole.WriteLine("[yellow]Discovery Disputed[/]");

                ModelHelper.PrintModel(planet);

            }
            AnsiConsole.WriteLine();
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get constellation."}[/]");
            return 1;
        }
    }
}