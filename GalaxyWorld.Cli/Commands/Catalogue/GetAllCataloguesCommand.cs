using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
    public class GetAllCataloguesCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            var client = new ApiClient();
            var result = await client.GetAsync<List<object>>("/catalogues");

            AnsiConsole.Write(new Markup("[bold green]Catalogues:[/]\n"));
            foreach (var item in result ?? new())
                AnsiConsole.MarkupLine($"- {item}");

            return 0;
        }
    }
}