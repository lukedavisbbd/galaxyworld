using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModels = GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Catalogue;

public class CreateCatalogueCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var client = new ApiClient();

        try
        {
            var insert = ModelHelper.PromptModel<CatalogueModels::CatalogueInsert>();
            var catalogue = await client.PostCatalogue(insert);

            AnsiConsole.MarkupLine($"[green]Created Catalogue[/]");
            ModelHelper.PrintModel(catalogue);
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create catalogue."}[/]");
            return 1;
        }
    }
}