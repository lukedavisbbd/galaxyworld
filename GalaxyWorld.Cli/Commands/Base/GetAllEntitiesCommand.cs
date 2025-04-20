using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Cli.Exceptions;
using System.ComponentModel;

namespace GalaxyWorld.Cli.Commands.Base;

public abstract class GetEntitiesWithQueryCommand<TModel, TSort> : Command<GetEntitiesWithQueryCommand<TModel, TSort>.Settings>
    where TModel : class
    where TSort : Enum
{
    protected readonly ApiClient _apiClient = new();
    protected abstract string Path { get; }

    public class Settings : CommandSettings
    {
        [Description("Sort options specific to the entity (e.g., Name, NameDsc, etc.)")]
        [CommandOption("-s|--sort <sort>")]
        public TSort Sort { get; init; }

        [CommandOption("-p|--page <page>")]
        public int Page { get; init; } = 1;

        [CommandOption("-l|--page-length <length>")]
        public int Length { get; init; } = 20;

        [Description("Filter(s): Property.Operation.Value — e.g., Distance.Lt.10")]
        [CommandOption("-f|--filter <filter>")]
        public string[]? Filter { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var page = int.Max(settings.Page, 1);
            var length = int.Max(settings.Length, 1);
            var filters = (settings.Filter ?? []).Select(filter => Filter<TModel>.Parse(filter, null)).ToArray();

            var results = _apiClient.GetWithQueryAsync(
                $"/{Path}",
                (page - 1) * length,
                length,
                settings.Sort,
                filters
            ).Result;

            if (results == null || results.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No {typeof(TModel).Name}s found.[/]");
                return 0;
            }

            AnsiConsole.MarkupLine($"[green]{typeof(TModel).Name}s:[/]");
            foreach (var result in results)
                AnsiConsole.MarkupLine(result.ToString());

            // var table = new Table().Title($"[bold]Catalogues (Sorted by {FormatHelper.PascalToTitleCase(settings.Sort.ToString())})[/]").AddColumns("ID", "Name", "Slug");

            // foreach (var result in results)
            // {
            //     table.AddRow(catalogue.CatId.ToString(), 
            //     catalogue.CatName, 
            //     catalogue.CatSlug);
            // }

            // AnsiConsole.Write(table);
            
            Console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
            return 0;
        }
        catch (AppException ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message ?? "Failed to get all "+typeof(TModel).Name}");
            return 1;
        }
    }
}
