using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Core.Models.Constellation;

namespace GalaxyWorld.Cli.Commands.Constellations;

public class DrawConstellationCommand : Command<DrawConstellationCommand.Settings>
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
            var constellation = client.GetAsync<Constellation>($"/constellations/{settings.Id}").Result;
            var stars = client.GetWithQueryAsync<StarSort, Star>($"/constellations/{settings.Id}/stars", 0, 500, StarSort.Magnitude, []).Result;

            DrawConstellation.DrawStars(constellation, stars);
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get constellation."}[/]");
            return 1;
        }
    }
}