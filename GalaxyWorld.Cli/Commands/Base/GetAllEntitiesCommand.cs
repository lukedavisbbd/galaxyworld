using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Cli.Exceptions;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GalaxyWorld.Cli.Commands.Base;

public abstract class GetEntitiesWithQueryCommand<TModel, TSort> : AsyncCommand<GetEntitiesWithQueryCommand<TModel, TSort>.Settings>
    where TModel : class
    where TSort : Enum
{
    protected readonly ApiClient _apiClient = new();
    protected abstract string Path { get; }

    protected abstract void Display(IEnumerable<TModel> items, string sortBy);

    public class Settings : CommandSettings
    {
        [Description("Sort options specific to the entity (e.g., Name, NameDsc, etc.)")]
        [CommandOption("-s|--sort <sort>")]
        public TSort Sort { get; init; }

        [CommandOption("-p|--page <page>")]
        public int Page { get; init; } = 1;

        [CommandOption("-l|--page-length <length>")]
        public int Length { get; init; } = 20;

        [Description("Filter(s): \"Property.Operation.Value\" — e.g., \"Distance.Lt.10\"")]
        [CommandOption("-f|--filter <filter>")]
        public string[]? Filter { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var page = int.Max(settings.Page, 1);
            var length = int.Max(settings.Length, 1);
            var filters = (settings.Filter ?? []).Select(filter => Filter<TModel>.Parse(filter, null)).ToArray();

            var result = await _apiClient.GetWithQueryAsync(
                $"/{Path}",
                (page - 1) * length,
                length,
                settings.Sort,
                filters
            );

            if (result == null || result.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No {typeof(TModel).Name}s found.[/]");
                return 0;
            }

            Display(result, settings.Sort.ToString());

            Console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
            return 0;
        }
        catch (AppException ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message ?? "Failed to get all "+typeof(TModel).Name}[/]");
            return 1;
        }
    }
}
