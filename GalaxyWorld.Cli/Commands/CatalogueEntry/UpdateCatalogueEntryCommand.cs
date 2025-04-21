using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.CatalogueEntry;

public class UpdateCatalogueEntryCommand : AsyncCommand<UpdateCatalogueEntryCommand.Settings>
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
            var patch = ModelHelper.PromptModel<CatalogueEntryPatch>();
            var entry = await client.PatchCatalogueEntry(settings.CatalogueId, settings.StarId, patch);

            AnsiConsole.MarkupLine("[green]Updated Entry[/]");
            ModelHelper.PrintModel(entry);
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to update catalogue entry."}[/]");
            return 1;
        }
    }
}
