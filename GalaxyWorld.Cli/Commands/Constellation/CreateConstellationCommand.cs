using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using ConstellationModels = GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;

namespace GalaxyWorld.Cli.Commands.Constellation;

public class CreateConstellationCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var client = new ApiClient();

        try
        {
            var insert = ModelUtil.PromptModel<ConstellationModels::ConstellationInsert>();
            var constellation = await client.PostConstellation(insert);

            ModelUtil.PrintModel(constellation);

            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create constellation."}[/]");
            return 1;
        }
    }
}