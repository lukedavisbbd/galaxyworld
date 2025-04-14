using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModel = GalaxyWorld.Core.Models.Catalogue.Catalogue;
using GalaxyWorld.Cli.Exceptions;

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

            try
            {
                var catalogue = await client.GetCatalogue(settings.Id);

                var table = new Table().Title("[bold]Catalogue Details[/]").AddColumns("Field", "Value");

                foreach (var prop in typeof(CatalogueModel).GetProperties())
                {
                    var value = prop.GetValue(catalogue)?.ToString() ?? "[grey]null[/]";
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