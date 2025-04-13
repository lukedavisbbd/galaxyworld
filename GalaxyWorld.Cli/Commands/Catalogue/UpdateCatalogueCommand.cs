using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModel = GalaxyWorld.Core.Models.Catalogue.Catalogue;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
    public class UpdateCatalogueCommand : AsyncCommand<UpdateCatalogueCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<id>")]
            public int Id { get; set; }

            [CommandOption("-n|--name <NAME>")]
            public string? Name { get; set; }

            [CommandOption("-d|--description <DESCRIPTION>")]
            public string? Description { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            var catalogue = await client.PatchAsync<object>($"/catalogues/{settings.Id}", new
            {
                name = settings.Name,
                description = settings.Description
            });

            if (catalogue is null)
            {
                AnsiConsole.MarkupLine("[yellow]No catalogue updated.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Catalogue Updated[/]").AddColumns("Field", "Value");

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