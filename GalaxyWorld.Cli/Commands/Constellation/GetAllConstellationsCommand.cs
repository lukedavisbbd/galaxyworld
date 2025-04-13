using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Constellation
{
    public class GetAllConstellationsCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            var client = new ApiClient();
            var result = await client.GetAsync<List<object>>("/constellations");

            AnsiConsole.MarkupLine("[bold green]Constellations:[/]");
            foreach (var item in result ?? new())
                AnsiConsole.MarkupLine($"- {item}");

            return 0;
        }
    }
}