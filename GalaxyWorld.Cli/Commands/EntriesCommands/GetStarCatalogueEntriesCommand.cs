using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Util;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class GetStarCatalogueEntriesCommand : AsyncCommand<GetStarCatalogueEntriesCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<starId>")] public int starId { get; set; }
        
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var star = await client.GetStarCatalogueEntries(settings.starId);

            AnsiConsole.Write(ModelUtil.ModelToTable(star, "Details"));
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get star."}[/]");
            return 1;
        }
    }
}
