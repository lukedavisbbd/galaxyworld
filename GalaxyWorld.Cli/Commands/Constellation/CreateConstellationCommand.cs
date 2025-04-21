using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using ConstellationModels = GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Constellation;

public class CreateConstellationCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var client = new ApiClient();

        try
        {
            var insert = ModelHelper.PromptModel<ConstellationModels::ConstellationInsert>();
            var constellation = await client.PostConstellation(insert);

            AnsiConsole.MarkupLine("[green]Created Constellation[/]");
            ModelHelper.PrintModel(constellation);

            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to create constellation."}[/]");
            return 1;
        }
    }
}