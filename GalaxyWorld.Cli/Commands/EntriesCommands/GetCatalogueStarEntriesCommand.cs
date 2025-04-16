using Spectre.Console.Cli;
using System.ComponentModel;
using GalaxyWorld.Cli.ApiHandler;
using Spectre.Console;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Cli.Commands.EntriesCommands;

public class GetCatalogueStarEntriesCommand : AsyncCommand<GetCatalogueStarEntriesCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<catalogue_id>")]
        public int CatalogueId { get; set; }
        [Description("one of: EntryId, EntryIdDsc, EntryDesignation, EntryDesignationDsc")]
        [CommandOption("-s|--sort <sort>")]
        public CatalogueEntrySort Sort { get; init; }
        [CommandOption("-p|--page <page>")]
        public int Page { get; init; } = 1;
        [CommandOption("-l|--page-length <length>")]
        public int Length { get; init; } = 20;
        [Description("one or more filters of the form: '{PropertyName}.{Operation}.{Value}', where {PropertyName} is the pascal case name of the property, {Operation} is one of Eq, Neq, Gt, Lt, Gte, or Lte, and {Value} is a comparison value. E.g. \"EntryDesignation.Eq.55 Can%\" for 'star with designation starting 55 Can', which is 55 Cancri")]
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
            var filters = (settings.Filter ?? []).Select(filter => Filter<CatalogueEntry>.Parse(filter, null)).ToArray();
            var constellations = await client.GetCatalogueStarEntries(settings.CatalogueId, (page - 1) * length, length, settings.Sort, filters);

            if (constellations.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No entries found.[/]");
                return 0;
            }

            var table = new Table().Title($"[bold]Catelogue Entries (Sorted by {FormatHelper.PascalToTitleCase(settings.Sort.ToString())})[/]").AddColumns("Star ID", "Entry ID", "Entry Designation");

            foreach (var constellation in constellations)
            {
                table.AddRow(
                    constellation.StarId.ToString(),
                    constellation.EntryId ?? "",
                    constellation.EntryDesignation ?? ""
                );
            }

            AnsiConsole.Write(table);

            Console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "Failed to get catalogue."}[/]");
            return 1;
        }
    }
}
