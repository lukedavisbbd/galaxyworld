using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using ConstellationModel = GalaxyWorld.Core.Models.Constellation.Constellation;

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
            var constellation = await client.PatchAsync<object>($"/constellations/{settings.Id}", new
            {
                name = settings.Name
            });

            if (constellation is null)
            {
                AnsiConsole.MarkupLine("[yellow]No constellation updated.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Constellation Updated[/]").AddColumns("Field", "Value");

            foreach (var prop in typeof(ConstellationModel).GetProperties())
            {
                var value = prop.GetValue(constellation)?.ToString() ?? "[grey]null[/]";
                table.AddRow(prop.Name, value);
            }

            AnsiConsole.Write(table);
            return 0;
        }
    }
}