using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;

namespace GalaxyWorld.Cli.Commands.Star
{
    public class GetAllStarsCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            var client = new ApiClient();
            var result = await client.GetAsync<List<object>>("/stars");

            AnsiConsole.MarkupLine("[bold green]Stars:[/]");
            foreach (var item in result ?? new())
                AnsiConsole.MarkupLine($"- {item}");

            return 0;
        }
    }
}