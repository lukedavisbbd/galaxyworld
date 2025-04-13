using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Constellation
{
    public class UpdateConstellationCommand : AsyncCommand<UpdateConstellationCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }

            [CommandOption("-n|--name <NAME>")]
            public string? Name { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var result = await client.PatchAsync<object>($"/constellations/{settings.Id}", new
            {
                name = settings.Name
            });

            AnsiConsole.MarkupLine("[green]Constellation updated:[/]");
            AnsiConsole.WriteLine(result?.ToString());
            return 0;
        }
    }
}