using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using StarModel = GalaxyWorld.Core.Models.Star.Star;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Star
{
    public class GetStarByIdCommand : AsyncCommand<GetStarByIdCommand.Settings>
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
                var star = await client.GetStar(settings.Id);

                var table = new Table().Title("[bold]Star Details[/]").AddColumns("Field", "Value");

                foreach (var prop in typeof(StarModel).GetProperties())
                {
                    var value = prop.GetValue(star)?.ToString() ?? "[grey]null[/]";
                    table.AddRow(prop.Name, value);
                }

                AnsiConsole.Write(table);
                return 0;
            }
            catch (CliException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get catalogue."}[/]");
                return 1;
            }
        }
    }
}