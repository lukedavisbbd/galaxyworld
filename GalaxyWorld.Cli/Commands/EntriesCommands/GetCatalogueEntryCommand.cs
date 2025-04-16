using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class GetCatalogueEntryCommand : AsyncCommand<GetCatalogueEntryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogue_id>")]
        public int CatalogueId { get; set; }
        [CommandArgument(1, "<star_id>")]
        public int StarId { get; set; }
        
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var entry = await client.GetCatalogueEntry(settings.CatalogueId, settings.StarId);

            ModelHelper.PrintModel(entry);
            
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get catalogue."}[/]");
            return 1;
        }
    }
}
