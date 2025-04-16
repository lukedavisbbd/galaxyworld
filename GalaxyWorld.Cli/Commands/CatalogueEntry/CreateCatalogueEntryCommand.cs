using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.CatalogueEntry;

public class CreateCatalogueEntryCommand : AsyncCommand<CreateCatalogueEntryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogue_id>")]
        public int CatalogueId { get; set; }
        [CommandArgument(1, "<star_id>")]
        public int StarId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();
        
        try
        {
            var insert = ModelHelper.PromptModel<CatalogueEntryInsert>();
            
            var entry = await client.PostCatalogueEntry(settings.CatalogueId, settings.StarId, insert);

            ModelHelper.PrintModel(entry);
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create star entry."}[/]");
            return 1;
        }
    }
}
