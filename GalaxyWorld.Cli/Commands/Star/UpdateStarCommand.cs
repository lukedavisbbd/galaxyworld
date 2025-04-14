using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using CoreModels = GalaxyWorld.Core.Models;
using StarModels = GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;
using System.Text.Json;

namespace GalaxyWorld.Cli.Commands.Star
{
    public class UpdateStarCommand : AsyncCommand<UpdateStarCommand.Settings>
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
                var patch = ModelUtil.PromptModel<StarModels::StarPatch>();

                Console.WriteLine(JsonSerializer.Serialize(patch.Constellation));
                Console.WriteLine(JsonSerializer.Serialize(patch));

                var star = await client.PatchStar(settings.Id, patch);

                AnsiConsole.Write(ModelUtil.ModelToTable(star, "Updated"));
                return 0;
            }
            catch (AppException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to update constellation."}[/]");
                return 1;
            }
        }
    }
}