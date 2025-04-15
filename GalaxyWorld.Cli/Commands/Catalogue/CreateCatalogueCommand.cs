using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModels = GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;

namespace GalaxyWorld.Cli.Commands.Catalogue;

public class CreateCatalogueCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var client = new ApiClient();

        try
        {
            var insert = ModelUtil.PromptModel<CatalogueModels::CatalogueInsert>();
            var catalogue = await client.PostCatalogue(insert);

            ModelUtil.PrintModel(catalogue);
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create catalogue."}[/]");
            return 1;
        }
    }
}