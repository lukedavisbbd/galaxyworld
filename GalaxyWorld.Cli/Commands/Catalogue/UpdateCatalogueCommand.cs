using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using CoreModels = GalaxyWorld.Core.Models;
using CatalogueModels = GalaxyWorld.Core.Models.Catalogue;

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

            [CommandOption("-d|--slug <SLUG>")]
            public string? Slug { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var client = new ApiClient();
            
            try
            {
                var catNameOpt = string.IsNullOrWhiteSpace(settings.Name) ?
                    new CoreModels::Optional() :
                    new CoreModels::Optional<string>(catName);
                var catSlugOpt = string.IsNullOrWhiteSpace(settings.Slug) ?
                    new CoreModels::Optional() :
                    new CoreModels::Optional<string>(catSlug);
                var catalogue = await client.PatchCatalogue(new CatalogueModels::CataloguePatch
                {
                    CatName = catNameOpt,
                    CatSlug = catSlugOpt,
                });

                var table = new Table().Title("[bold]Catalogue Updated[/]").AddColumns("Field", "Value");

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
                AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to update catalogue."}[/]");
                return 1;
            }
        }
    }
}