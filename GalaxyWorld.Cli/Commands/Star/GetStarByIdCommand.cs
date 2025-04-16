using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using StarModels = GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Star;

public class GetStarByIdCommand : AsyncCommand<GetStarByIdCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int Id { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var star = await client.GetStar(settings.Id);

            ModelHelper.PrintModel(star);

            var catalogues = await client.GetCatalogues();
            var entries = await client.GetStarCatalogueEntries(settings.Id, 0, 999);

            AnsiConsole.MarkupLine($"[bold]Catalogue Entries:[/]");
            foreach (var entry in entries)
            {
                var catalogue = catalogues.First(cat => cat.CatId == entry.CatId);
                AnsiConsole.MarkupLine($"[yellow]{catalogue.CatName}:[/] {entry.EntryId} {entry.EntryDesignation}");
            }

            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get constellation."}[/]");
            return 1;
        }
    }
}