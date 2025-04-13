using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using ConstellationModel = GalaxyWorld.Core.Models.Constellation.Constellation;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Constellation
{
    public class GetConstellationByIdCommand : AsyncCommand<GetConstellationByIdCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            
            try
            {
                var constellation = await client.GetConstellation(settings.Id);

                var table = new Table().Title("[bold]Constellation Details[/]").AddColumns("Field", "Value");

                foreach (var prop in typeof(ConstellationModel).GetProperties())
                {
                    var value = prop.GetValue(constellation)?.ToString() ?? "[grey]null[/]";
                    table.AddRow(prop.Name, value);
                }

                AnsiConsole.Write(table);
                return 0;
            }
            catch (CliException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get constellation."}[/]");
                return 1;
            }
        }
    }
}