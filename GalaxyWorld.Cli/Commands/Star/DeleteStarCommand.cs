using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;

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

            try 
            {
                var success = await client.DeleteStar(settings.Id);
                AnsiConsole.MarkupLine("[green]Star deleted successfully.[/]");
                return 0;
            }
            catch (CliException e) 
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to delete star."}[/]");
                return 1;
            }
        }
    }
}