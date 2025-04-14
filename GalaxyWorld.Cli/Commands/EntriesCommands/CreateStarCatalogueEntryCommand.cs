using Spectre.Console.Cli;
using GalaxyWorld.Cli.Services;
using GalaxyWorld.Cli.Helper;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaxyWorld.Cli.ApiHandler;
using Microsoft.VisualBasic;
using CoreModels = GalaxyWorld.Core.Models;
using Spectre.Console;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class CreateStarCatalogueEntryCommand : AsyncCommand<CreateStarCatalogueEntryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogueId>")] public int catalogueId { get; set; }
        [CommandArgument(1, "<starId>")] public int starId { get; set; }
        [CommandOption("--notes <notes>")] public string? Notes { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();
        
        try
        {
            var EntryId = InputHelper.Prompt<string>("Entry Id");
            var EntryDesignation = InputHelper.Prompt<string>("Entry Designation");

            var result = await client.PostStarCatalogueEntry(settings.starId, new CatalogueEntryInsert
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
            AnsiConsole.MarkupLine($"[red] Failed to create star entry: {ex.Message}[/]");
            return 1;
        }
    }
}
