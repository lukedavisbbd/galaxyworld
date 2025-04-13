using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
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
            var success = await client.DeleteAsync($"/catalogues/{settings.Id}");

            if (success)
                AnsiConsole.MarkupLine("[green]Catalogue deleted successfully.[/]");
            else
                AnsiConsole.MarkupLine("[red]Failed to delete catalogue.[/]");

            return 0;
        }
    }
}