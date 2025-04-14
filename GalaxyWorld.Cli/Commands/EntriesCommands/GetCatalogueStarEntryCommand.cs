using Spectre.Console.Cli;
using GalaxyWorld.Cli.Services;
using GalaxyWorld.Cli.Helper;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class GetCatalogueStarEntryCommand : AsyncCommand<GetCatalogueStarEntryCommand.Settings>
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
            var result = await client.GetCatalogueStarEntry(settings.catalogueId, settings.starId);
            
            if (result is null)
            {
                AnsiConsole.MarkupLine("[yellow]No entries found.[/]");
                return 0;
            }
            
            var table = new Table().Title("[bold]Entries[/]").AddColumns("Field", "Value");

            foreach (var prop in typeof(CatalogueEntry).GetProperties())
            {
                var value = prop.GetValue(result)?.ToString() ?? "[grey]null[/]";
                table.AddRow(prop.Name, value);
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
