using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Helper;
using ConstellationModels = GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Exceptions;

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

            try
            {
                var payload = new ConstellationModels::ConstellationInsert
                {
                    ConName = InputHelper.Prompt<string>("Constellation Name"),
                    Genitive = InputHelper.Prompt<string>("Genitive Form"),
                    IauAbbr = InputHelper.Prompt<string>("IAU Abbreviation"),
                    Meaning = InputHelper.Prompt<string>("Meaning"),
                    NasaAbbr = InputHelper.Prompt<string>("NASA Abbreviation"),
                    Origin = InputHelper.Prompt<string>("Origin")
                };

                var constellation = await client.PostConstellation(payload);

                var table = new Table().Title("[bold]Constellation Created[/]").AddColumns("Field", "Value");

                foreach (var prop in typeof(ConstellationModels::Constellation).GetProperties())
                {
                    var value = prop.GetValue(constellation)?.ToString() ?? "[grey]null[/]";
                    table.AddRow(prop.Name, value);
                }

                AnsiConsole.Write(table);
                return 0;
            }
            catch (CliException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create constellation."}[/]");
                return 1;
            }
        }
    }
}