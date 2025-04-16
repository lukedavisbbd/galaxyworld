using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using ConModels = GalaxyWorld.Core.Models.Constellation;
using System.ComponentModel;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Constellation;

public class GetAllConstellationsCommand : AsyncCommand<GetAllConstellationsCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("one of: ConName, ConNameDsc, IauAbbr, IauAbbrDsc, NasaAbbr, NasaAbbrDsc, Genitive, GenitiveDsc")]
        [CommandOption("-s|--sort <sort>")]
        public ConstellationSort Sort { get; init; }
        [CommandOption("-p|--page <page>")]
        public int Page { get; init; } = 1;
        [CommandOption("-l|--page-length <length>")]
        public int Length { get; init; } = 20;
        [Description("one or more filters of the form: '{PropertyName}.{Operation}.{Value}', where {PropertyName} is the pascal case name of the property, {Operation} is one of Eq, Neq, Gt, Lt, Gte, or Lte, and {Value} is a comparison value. E.g. Origin.Eq.%Ptolemy% for 'constellations with origin containing Ptolemy'")]
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
            var filters = (settings.Filter ?? []).Select(filter => Filter<ConModels::Constellation>.Parse(filter, null)).ToArray();
            var constellations = await client.GetConstellations((page - 1) * length, length, settings.Sort, filters);

            if (constellations.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No constellations found.[/]");
                return 0;
            }

            var table = new Table().Title($"[bold]Constellations (Sorted by {FormatHelper.PascalToTitleCase(settings.Sort.ToString())})[/]").AddColumns("ID", "Name", "IAU Abbr.", "NASA Abbr.", "Genitive", "Origin", "Meaning");

            foreach (var constellation in constellations)
            {
                table.AddRow(constellation.ConId.ToString(), 
                constellation.ConName, 
                constellation.IauAbbr, 
                constellation.NasaAbbr, 
                constellation.Genitive, 
                constellation.Origin, 
                constellation.Meaning);
            }

            AnsiConsole.Write(table);
            
            Console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "No constellations found."}[/]");
            return 1;
        }
    }
}