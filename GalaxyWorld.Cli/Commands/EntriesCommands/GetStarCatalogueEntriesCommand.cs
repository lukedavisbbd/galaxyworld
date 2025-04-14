using Spectre.Console.Cli;
using GalaxyWorld.Cli.Services;
using GalaxyWorld.Cli.Helper;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class GetStarCatalogueEntriesCommand : AsyncCommand<GetStarCatalogueEntriesCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<starId>")] public int starId { get; set; }
        
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var results = await client.GetStarCatalogueEntries(settings.starId);
            
            if (results is null || results.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No entries found.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Entries[/]").AddColumns("StarId", "CatId", "EntryId", "EntryDesignation");

            foreach (var result in results)
            {
                table.AddRow(result.StarId.ToString(), 
                result.CatId.ToString(), 
                result.EntryId.ToString(), 
                result.EntryDesignation.ToString());
            }

            AnsiConsole.Write(table);
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red] Failed to fetch catalogue entries: {ex.Message}[/]");
            return 1;
        }
    }
}
