using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Helper;
using ConstellationModel = GalaxyWorld.Core.Models.Constellation.Constellation;

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

            // var payload = new ConstellationModel::ConstellationInsert 

            var constellation = await client.PostAsync<object>("/constellations", new
            {
                name = settings.Name
            });

            if (constellation is null)
            {
                AnsiConsole.MarkupLine("[yellow]No constellation created.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Constellation Created[/]").AddColumns("Field", "Value");

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