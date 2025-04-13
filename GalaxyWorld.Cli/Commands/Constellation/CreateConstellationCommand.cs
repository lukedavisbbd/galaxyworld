using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Constellation
{
    public class CreateConstellationCommand : AsyncCommand<CreateConstellationCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            public string Name { get; set; } = "";
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var result = await client.PostAsync<object>("/constellations", new
            {
                name = settings.Name
            });

            AnsiConsole.MarkupLine("[green]Constellation created:[/]");
            AnsiConsole.WriteLine(result?.ToString());
            return 0;
        }
    }
}