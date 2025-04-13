using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using CatalogueModel = GalaxyWorld.Core.Models.Catalogue.Catalogue;

namespace GalaxyWorld.Cli.Commands.Catalogue
{
    public class GetAllCataloguesCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            var client = new ApiClient();
            var catalogues = await client.GetAsync<List<CatalogueModel>>("/catalogues");

            if (catalogues is null || catalogues.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No catalogues found.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Catalogues[/]").AddColumns("ID", "Name", "Slug");

            foreach (var catalogue in catalogues)
            {
                table.AddRow(catalogue.CatId.ToString(), 
                catalogue.CatName, 
                catalogue.CatSlug);
            }

            AnsiConsole.Write(table);
            return 0;
        }
    }
}