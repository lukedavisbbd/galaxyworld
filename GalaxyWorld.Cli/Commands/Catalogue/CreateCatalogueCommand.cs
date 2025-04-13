using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
    public class CreateCatalogueCommand : AsyncCommand<CreateCatalogueCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            public string Name { get; set; } = "";

            [CommandOption("-d|--description <DESCRIPTION>")]
            public string Description { get; set; } = "";
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var result = await client.PostAsync<object>("/catalogues", new
            {
                name = settings.Name,
                description = settings.Description
            });

            AnsiConsole.MarkupLine("[green]Catalogue created:[/]");
            AnsiConsole.WriteLine(result?.ToString());
            return 0;
        }
    }
}