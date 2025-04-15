using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;
using GalaxyWorld.Core.Models.CatalogueEntry;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

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
            var insert = ModelUtil.PromptModel<CatalogueEntryInsert>();
            var catalogue = await client.PostCatalogueStarEntry(settings.CatalogueId, insert);

            AnsiConsole.Write(ModelUtil.ModelToTable(catalogue, "Created"));
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create catalogue entry."}[/]");
            return 1;
        }
    }
}
