using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModels = GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;

namespace GalaxyWorld.Cli.Commands.Catalogue;

public class UpdateCatalogueCommand : AsyncCommand<UpdateCatalogueCommand.Settings>
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
            var patch = ModelUtil.PromptModel<CatalogueModels::CataloguePatch>();
            var catalogue = await client.PatchCatalogue(settings.Id, patch);

            ModelUtil.PrintModel(catalogue);

            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to update catalogue."}[/]");
            return 1;
        }
    }
}