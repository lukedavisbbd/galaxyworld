using Spectre.Console.Cli;
using GalaxyWorld.Cli.Services;
using GalaxyWorld.Cli.Helper;
using System.ComponentModel;
using System.Threading.Tasks;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Core.Models.CatalogueEntry;
using CoreModels = GalaxyWorld.Core.Models;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class UpdateCatalogueStarEntryCommand : AsyncCommand<UpdateCatalogueStarEntryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogueId>")]
        public int CatalogueId { get; set; }

        [CommandArgument(1, "<starId>")]
        public int StarId { get; set; }

        [CommandOption("--notes <notes>")]
        public string? Notes { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var EntryId = new CoreModels::Optional<string>(InputHelper.Prompt<string>("Entry Id"));
            var EntryDesignation = new CoreModels::Optional<string>(InputHelper.Prompt<string>("Entry Designation"));

            var result = await client.PatchCatalogueStarEntry(settings.CatalogueId, settings.StarId, new CatalogueEntryPatch
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
            Console.WriteLine($"[red] Failed to patch entry: {ex.Message}[/]");
            return 1;
        }
    }
}
