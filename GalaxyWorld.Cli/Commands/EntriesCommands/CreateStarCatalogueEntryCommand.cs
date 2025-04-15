using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;

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
            var insert = ModelUtil.PromptModel<CatalogueEntryInsert>();
            var star = await client.PostStarCatalogueEntry(settings.starId, insert);

            AnsiConsole.Write(ModelUtil.ModelToTable(star, "Created"));
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create star entry."}[/]");
            return 1;
        }
    }
}
