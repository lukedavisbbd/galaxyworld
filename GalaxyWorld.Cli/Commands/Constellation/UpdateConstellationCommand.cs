using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using CoreModels = GalaxyWorld.Core.Models;
using ConstellationModels = GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;

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
            
            try
            {
                var payload = new ConstellationModels::ConstellationPatch
                {
                    ConName = new CoreModels::Optional<string>(InputHelper.Prompt<string>("Constellation Name")),
                    Genitive = new CoreModels::Optional<string>(InputHelper.Prompt<string>("Genitive Form")),
                    IauAbbr = new CoreModels::Optional<string>(InputHelper.Prompt<string>("IAU Abbreviation")),
                    Meaning = new CoreModels::Optional<string>(InputHelper.Prompt<string>("Meaning")),
                    NasaAbbr = new CoreModels::Optional<string>(InputHelper.Prompt<string>("NASA Abbreviation")),
                    Origin = new CoreModels::Optional<string>(InputHelper.Prompt<string>("Origin"))
                };

                var catalogue = await client.PatchConstellation(settings.Id, payload);

                var table = new Table().Title("[bold]Catalogue Updated[/]").AddColumns("Field", "Value");

                foreach (var prop in typeof(ConstellationModels::Constellation).GetProperties())
                {
                    var value = prop.GetValue(catalogue)?.ToString() ?? "[grey]null[/]";
                    table.AddRow(prop.Name, value);
                }

                AnsiConsole.Write(table);
                return 0;
            }
            catch (CliException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to update catalogue."}[/]");
                return 1;
            }
        }
    }
}