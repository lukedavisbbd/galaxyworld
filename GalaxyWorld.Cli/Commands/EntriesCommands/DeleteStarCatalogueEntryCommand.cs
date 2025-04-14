using Spectre.Console.Cli;
using GalaxyWorld.Cli.Services;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class DeleteStarCatalogueEntryCommand : AsyncCommand<DeleteStarCatalogueEntryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogueId>")]
        public int CatalogueId { get; set; }

        [CommandArgument(1, "<starId>")]
        public int StarId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var result = await client.DeleteStarCatalogueEntry(settings.StarId, settings.CatalogueId);
            AnsiConsole.MarkupLine("[green]Star catalogue entry deleted successfully.[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red] Failed to delete entry: {ex.Message}[/]");
            return 1;
        }
    }
}
