using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class GetCatalogueStarEntriesCommand : AsyncCommand<GetCatalogueStarEntriesCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogueId>")]
        [Description("Catalogue ID to fetch entries from")]
        public int CatalogueId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var catalogue = await client.GetCatalogueStarEntries(settings.CatalogueId);

            AnsiConsole.Write(ModelUtil.ModelToTable(catalogue, "Details"));
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get catalogue."}[/]");
            return 1;
        }
    }
}
