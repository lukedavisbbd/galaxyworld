using Spectre.Console.Cli;
using GalaxyWorld.Cli.Services;
using GalaxyWorld.Cli.Helper;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class DeleteCatalogueStarEntryCommand : AsyncCommand<DeleteCatalogueStarEntryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogueId>")] public int catalogueId { get; set; }
        [CommandArgument(1, "<starId>")] public int starId { get; set; }
        
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var result = await client.DeleteCatalogueStarEntry(settings.catalogueId, settings.starId);
            AnsiConsole.MarkupLine("[green]Catalogue star entry deleted successfully.[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red] Failed to execute DeleteCatalogueStarEntryCommand: {ex.Message}[/]");
            return 1;
        }
    }
}
