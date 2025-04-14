using Spectre.Console;
using Spectre.Console.Cli;
using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;

namespace GalaxyWorld.Cli.Commands.Constellation;

public class GetAllConstellationsCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var client = new ApiClient();

        try
        {
            var constellations = await client.GetConstellations();

            if (constellations.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No constellations found.[/]");
                return 0;
            }

            var table = new Table().Title("[bold]Constellations[/]").AddColumns("ID", "Name", "IauAbbr", "NasaAbbr", "Genitive", "Origin", "Meaning");

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
            return 0;
        }
        catch (AppException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message ?? "No constellations found."}[/]");
            return 1;
        }
    }
}