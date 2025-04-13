using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;

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

            try
            {
                await client.DeleteConstellation(settings.Id);
                AnsiConsole.MarkupLine("[green]Constellation deleted successfully.[/]");
                return 0;
            }
            catch (CliException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to delete constellation."}[/]");
                return 1;
            }
        }
    }
}