using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
    public class GetCatalogueByIdCommand : AsyncCommand<GetCatalogueByIdCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var result = await client.GetAsync<object>($"/catalogues/{settings.Id}");

            AnsiConsole.MarkupLine("[bold green]Catalogue Details:[/]");
            AnsiConsole.WriteLine(result?.ToString() ?? "Not found.");
            return 0;
        }
    }
}