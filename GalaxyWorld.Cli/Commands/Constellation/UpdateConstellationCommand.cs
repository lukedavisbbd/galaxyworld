using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using ConstellationModels = GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Constellation;

public class UpdateConstellationCommand : AsyncCommand<UpdateConstellationCommand.Settings>
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
            var patch = ModelHelper.PromptModel<ConstellationModels::ConstellationPatch>();
            var constellation = await client.PatchConstellation(settings.Id, patch);

            AnsiConsole.MarkupLine("[green]Updated Constellation[/]");
            ModelHelper.PrintModel(constellation);
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to update constellation."}[/]");
            return 1;
        }
    }
}