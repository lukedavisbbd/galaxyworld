using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModel = GalaxyWorld.Core.Models.Catalogue.Catalogue;

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
            var catalogue = await client.GetAsync<CatalogueModel>($"/catalogues/{settings.Id}");

            if (catalogue is null)
            {
                AnsiConsole.MarkupLine("[yellow]No catalogue found.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Catalogue Details[/]").AddColumns("Field", "Value");

            foreach (var prop in typeof(CatalogueModel).GetProperties())
            {
                var value = prop.GetValue(catalogue)?.ToString() ?? "[grey]null[/]";
                table.AddRow(prop.Name, value);
            }

            AnsiConsole.Write(table);
            return 0;
        }
    }
}