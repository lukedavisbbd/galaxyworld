using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Star
{
    public class UpdateStarCommand : AsyncCommand<UpdateStarCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }

            [CommandOption("-n|--name <NAME>")]
            public string? Name { get; set; }

            [CommandOption("-c|--catalogueId <CATALOGUE_ID>")]
            public int? CatalogueId { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var result = await client.PatchAsync<object>($"/stars/{settings.Id}", new
            {
                name = settings.Name,
                catalogueId = settings.CatalogueId
            });

            AnsiConsole.MarkupLine("[green]Star updated:[/]");
            AnsiConsole.WriteLine(result?.ToString());
            return 0;
        }
    }
}