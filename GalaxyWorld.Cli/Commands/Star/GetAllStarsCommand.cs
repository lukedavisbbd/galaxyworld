using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Core.Models;
using StarModels = GalaxyWorld.Core.Models.Star;
using System.ComponentModel;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Star;

public class GetAllStarsCommand : AsyncCommand<GetAllStarsCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("one of: ProperName, ProperNameDsc, Distance, DistanceDsc, Magnitude, MagnitudeDsc, AbsoluteMagnitude, AbsoluteMagnitudeDsc")]
        [CommandOption("-s|--sort <sort>")]
        public StarSort Sort { get; init; }
        [CommandOption("-p|--page <page>")]
        public int Page { get; init; } = 1;
        [CommandOption("-l|--page-length <length>")]
        public int Length { get; init; } = 20;
        [Description("one or more filters of the form: '{PropertyName}.{Operation}.{Value}', where {PropertyName} is the pascal case name of the property, {Operation} is one of Eq, Neq, Gt, Lt, Gte, or Lte, and {Value} is a comparison value. E.g. Distance.Lt.10 for 'distance less than 10 parsecs'")]
        [CommandOption("-f|--filter <length>")]
        public required string[] Filter { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var client = new ApiClient();

        try
        {
            var page = int.Max(settings.Page, 1);
            var length = int.Max(settings.Length, 1);
            var filters = (settings.Filter ?? []).Select(filter => Filter<StarModels::Star>.Parse(filter, null)).ToArray();
            var stars = await client.GetStars((page - 1) * length, length, settings.Sort, filters);

            if (stars is null || stars.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No stars found.[/]");
                return 0;
            }

            var table = new Table().Title($"[bold]Stars[/] (Sorted by {FormatHelper.PascalToTitleCase(settings.Sort.ToString())})").AddColumns(
                "Star ID",
                "Constellation ID",
                "Proper Name",
                "Distance",
                "Magnitude",
                "Spectral Type"
            );

            foreach (var star in stars)
            {
                table.AddRow(
                    star.StarId.ToString(),
                    star.ConstellationId.ToString() ?? "",
                    star.ProperName ?? "",
                    star.Distance.ToString() ?? "",
                    star.Magnitude.ToString(),
                    star.SpectralType ?? ""
                );
            }

            AnsiConsole.Write(table);
            
            Console.WriteLine($"Page #{page} ({(page - 1) * length} - {page * length - 1})");
            return 0;
        }
        catch (AppException e) 
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message ?? "Failed to get stars list."}[/]");
            return 1;
        }
    }
}