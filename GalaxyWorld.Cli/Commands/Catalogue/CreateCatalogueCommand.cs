using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModels = GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
    public class CreateCatalogueCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            var client = new ApiClient();

            var catName = InputHelper.Prompt<string>("Catalogue Name");
            var defaultSlug = string.Concat(catName.ToLower().Where(c => char.IsAsciiLetter(c) || char.IsAsciiDigit(c) || c == '_'));
            var catSlug = InputHelper.Prompt<string>("Catalogue Slug", defaultSlug);

            try
            {
                var catalogue = await client.PostCatalogue(new CatalogueModels::CatalogueInsert
                {
                    CatName = catName,
                    CatSlug = catSlug,
                });

                var table = new Table().Title("[bold]Catalogue Created[/]").AddColumns("Field", "Value");

                foreach (var prop in typeof(CatalogueModels::Catalogue).GetProperties())
                {
                    var value = prop.GetValue(catalogue)?.ToString() ?? "[grey]null[/]";
                    table.AddRow(prop.Name, value);
                }

                AnsiConsole.Write(table);
                return 0;
            }
            catch (CliException e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to create catalogue."}[/]");
                return 1;
            }
        }
    }
}