using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModel = GalaxyWorld.Core.Models.Catalogue.Catalogue;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
    public class CreateCatalogueCommand : AsyncCommand<CreateCatalogueCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            public string Name { get; set; } = "";

            [CommandOption("-d|--description <DESCRIPTION>")]
            public string Description { get; set; } = "";
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var catalogue = await client.PostAsync<object>("/catalogues", new
            {
                name = settings.Name,
                description = settings.Description
            });

            if (catalogue is null)
            {
                AnsiConsole.MarkupLine("[yellow]No catalogue created.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Catalogue Created[/]").AddColumns("Field", "Value");

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