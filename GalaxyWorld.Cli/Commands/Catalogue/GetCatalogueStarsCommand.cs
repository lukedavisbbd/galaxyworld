using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using System.ComponentModel;
using EntryModels = GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Core.Models;

namespace GalaxyWorld.Cli.Commands.Catalogue;

public class GetCatalogueStarsCommand : AsyncCommand<GetCatalogueStarsCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int Id { get; set; }
        [Description("one of: EntryId, EntryIdDsc, EntryDesignation, EntryDesignationDsc")]
        [CommandOption("-s|--sort <sort>")]
        public EntryModels::CatalogueEntrySort Sort { get; init; }
        [CommandOption("-p|--page <page>")]
        public int Page { get; init; } = 1;
        [CommandOption("-l|--page-length <length>")]
        public int Length { get; init; } = 20;
        [Description("one or more filters of the form: '{PropertyName}.{Operation}.{Value}', where {PropertyName} is the pascal case name of the property, {Operation} is one of Eq, Neq, Gt, Lt, Gte, or Lte, and {Value} is a comparison value. E.g. EntryId.Eq.1234 for 'star with ID 1234 in the given catalogue'")]
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
            var filters = (settings.Filter ?? []).Select(filter => Filter<EntryModels::CatalogueEntry>.Parse(filter, null)).ToArray();
            var entries = await client.GetCatalogueStarEntries(settings.Id, (page - 1) * length, length, settings.Sort, filters);

            if (entries.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No entries found.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Catalogue Entries[/]").AddColumns("Star ID", "Entry ID", "Entry Designation");

            foreach (var entry in entries)
            {
                table.AddRow(entry.StarId.ToString(), entry.EntryId ?? "", entry.EntryDesignation ?? "");
            }

            AnsiConsole.Write(table);

            Console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to get catalogue."}[/]");
            return 1;
        }
    }
}