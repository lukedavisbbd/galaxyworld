using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using CatModels = GalaxyWorld.Core.Models.Catalogue;
using System.ComponentModel;
using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Catalogue;

public class GetAllCataloguesCommand : AsyncCommand<GetAllCataloguesCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("one of: CatName, CatNameDsc, CatSlug, CatSlugDsc")]
        [CommandOption("-s|--sort <sort>")]
        public CatalogueSort Sort { get; init; }
        [CommandOption("-p|--page <page>")]
        public int Page { get; init; } = 1;
        [CommandOption("-l|--page-length <length>")]
        public int Length { get; init; } = 20;
        [Description("one or more filters of the form: '{PropertyName}.{Operation}.{Value}', where {PropertyName} is the pascal case name of the property, {Operation} is one of Eq, Neq, Gt, Lt, Gte, or Lte, and {Value} is a comparison value. E.g. CatSlug.Eq.hd")]
        [CommandOption("-f|--filter <length>")]
        public required string[]? Filter { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var page = int.Max(settings.Page, 1);
            var length = int.Max(settings.Length, 1);
            var filters = (settings.Filter ?? []).Select(filter => Filter<CatModels::Catalogue>.Parse(filter, null)).ToArray();
            var catalogues = await client.GetCatalogues((page - 1) * length, length, settings.Sort, filters);

            if (catalogues.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No catalogues found.[/]");
                return 0;
            }

            var table = new Table().Title($"[bold]Catalogues (Sorted by {FormatHelper.PascalToTitleCase(settings.Sort.ToString())})[/]").AddColumns("ID", "Name", "Slug");

            foreach (var catalogue in catalogues)
            {
                table.AddRow(catalogue.CatId.ToString(), 
                catalogue.CatName, 
                catalogue.CatSlug);
            }

            AnsiConsole.Write(table);
            
            Console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "No catalogues found."}[/]");
            return 1;
        }
    }
}