using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
    public class UpdateCatalogueCommand : AsyncCommand<UpdateCatalogueCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }

            [CommandOption("-n|--name <NAME>")]
            public string? Name { get; set; }

            [CommandOption("-d|--description <DESCRIPTION>")]
            public string? Description { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var result = await client.PatchAsync<object>($"/catalogues/{settings.Id}", new
            {
                name = settings.Name,
                description = settings.Description
            });

            AnsiConsole.MarkupLine("[green]Catalogue updated:[/]");
            AnsiConsole.WriteLine(result?.ToString());
            return 0;
        }
    }
}