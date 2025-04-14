using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models.CatalogueEntry;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class CreateCatalogueStarEntryCommand : AsyncCommand<CreateCatalogueStarEntryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogueId>")]
        [Description("Catalogue ID")]
        public int CatalogueId { get; set; }

        [CommandOption("--notes <notes>")]
        [Description("Optional notes about the entry")]
        public string? Notes { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var EntryId = InputHelper.Prompt<string>("Entry Id");
            var EntryDesignation = InputHelper.Prompt<string>("Entry Designation");
            
            var result = await client.PostCatalogueStarEntry(settings.CatalogueId, new CatalogueEntryInsert
            {
                EntryId = EntryId,
                EntryDesignation = EntryDesignation,
            });

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
            AnsiConsole.MarkupLine($"[red] Failed to create catalogue entry: {ex.Message}[/]");
            return 1;
        }
    }
}
