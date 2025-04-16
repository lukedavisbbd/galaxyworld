using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Catalogue;

public class DeleteCatalogueCommand : AsyncCommand<DeleteCatalogueCommand.Settings>
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
            await client.DeleteCatalogue(settings.Id);
            AnsiConsole.MarkupLine("[green]Catalogue deleted successfully.[/]");
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to delete catalogue."}[/]");
            return 1;
        }
    }
}