using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.CatalogueEntry;

public class DeleteCatalogueEntryCommand : AsyncCommand<DeleteCatalogueEntryCommand.Settings>
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
            var result = await client.DeleteCatalogueEntry(settings.CatalogueId, settings.StarId);
            AnsiConsole.MarkupLine("[green]Catalogue entry deleted successfully.[/]");
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to delete catalogue entry."}[/]");
            return 1;
        }
    }
}
