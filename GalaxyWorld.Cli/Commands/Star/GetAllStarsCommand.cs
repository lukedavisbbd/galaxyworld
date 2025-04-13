using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using StarModel = GalaxyWorld.Core.Models.Star.Star;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Star
{
    public class GetAllStarsCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            var client = new ApiClient();

            try
            {
                var stars = await client.GetStars();

                if (stars is null || stars.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No stars found.[/]");
                    return 0;
                }

                var table = new Table().Title("[bold]Stars[/]").AddColumns("ID", "Name", "RA", "Dec", "Magnitude");

                foreach (var star in stars)
                {
                    table.AddRow(star.StarId.ToString(), 
                    star.ProperName, 
                    star.RightAscension.ToString(), 
                    star.Declination.ToString(), 
                    star.Magnitude.ToString());
                }

                AnsiConsole.Write(table);
                return 0;
            }
            catch (CliException e) 
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get stars list."}[/]");
                return 1;
            }
        }
    }
}