using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Star
{
    public class DeleteStarCommand : AsyncCommand<DeleteStarCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var success = await client.DeleteAsync($"/stars/{settings.Id}");

            if (success)
                AnsiConsole.MarkupLine("[green]Star deleted successfully.[/]");
            else
                AnsiConsole.MarkupLine("[red]Failed to delete star.[/]");

            return 0;
        }
    }
}