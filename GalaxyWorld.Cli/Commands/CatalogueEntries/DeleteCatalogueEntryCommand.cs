using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Cli.Commands.CatalogueEntries;

public class DeleteCatalogueEntryCommand : Command<DeleteCatalogueEntryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogue_id>")]
        public int CatalogueId { get; set; }
        [CommandArgument(1, "<star_id>")]
        public int StarId { get; set; }
        
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var result = client.DeleteAsync<CatalogueEntry>($"/catalogues/{settings.CatalogueId}/stars/{settings.StarId}").Result;

            AnsiConsole.MarkupLine("[green]Entry with Catalogue ID {settings.CatalogueId} and Star ID {settings.StarId} deleted successfully.[/]");
            AnsiConsole.WriteLine();
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to delete catalogue entry."}[/]");
            return 1;
        }
    }
}
