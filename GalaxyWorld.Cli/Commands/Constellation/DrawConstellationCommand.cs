using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Constellation;

public class DrawConstellationCommand : AsyncCommand<DrawConstellationCommand.Settings>
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
            var constellation = await client.GetConstellation(settings.Id);
            var stars = await client.GetConstellationStars(settings.Id);

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