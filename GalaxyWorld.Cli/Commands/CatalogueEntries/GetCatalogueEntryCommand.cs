using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Cli.Commands.CatalogueEntries;

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
            var result = await client.GetAsync<CatalogueEntry>($"/catalogues/{settings.CatalogueId}/stars/{settings.StarId}");

            AnsiConsole.MarkupLine($"[bold green]Catalogue Entry Found:[/]");
            ModelHelper.PrintModel(result);
            
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get catalogue."}[/]");
            return 1;
        }
    }
}
