using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Constellation
{
    public class DeleteConstellationCommand : AsyncCommand<DeleteConstellationCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var success = await client.DeleteAsync($"/constellations/{settings.Id}");

            if (success)
                AnsiConsole.MarkupLine("[green]Constellation deleted successfully.[/]");
            else
                AnsiConsole.MarkupLine("[red]Failed to delete constellation.[/]");

            return 0;
        }
    }
}