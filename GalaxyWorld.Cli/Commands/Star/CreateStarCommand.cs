using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models.CatalogueEntry;
using StarModels = GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;

namespace GalaxyWorld.Cli.Commands.Star;

public class CreateStarCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var client = new ApiClient();

        var insert = ModelUtil.PromptModel<StarModels::StarInsert>([nameof(StarModels::StarInsert.CatalogueEntries)]);

        var entries = new List<CatalogueEntryInsertWithStar>();
        
        while (AnsiConsole.Confirm("Add a catalogue entry?"))
        {
            entries.Add(new CatalogueEntryInsertWithStar
            {
                CatId = InputHelper.Prompt<int>("Catalogue ID"),
                EntryId = InputHelper.Prompt<string>("Catalogue Entry ID (Unique)"),
                EntryDesignation = InputHelper.Prompt<string>("Catalogue Entry Designation (Non-unique)")
            });
        }

        try {
            var star = await client.PostStar(insert);

            ModelUtil.PrintModel(star);
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create star"}[/]");
            return 1;
        }
    }
}