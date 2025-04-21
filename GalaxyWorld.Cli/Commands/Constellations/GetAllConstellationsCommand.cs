using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Core.Models.Constellation;

namespace GalaxyWorld.Cli.Commands.Constellations;

public class GetAllConstellationsCommand : GetEntitiesWithQueryCommand<Constellation, ConstellationSort>
{
    protected override string Path => Routes.Constellations;

    protected override void Display(IEnumerable<Constellation> constellations, string sortBy)
    {
        TableHelper.Print(
            items: constellations,
            title: $"Constellations (Sorted by {FormatHelper.PascalToTitleCase(sortBy)})",
            columns: new()
            {
                { "ID",       c => c.ConId.ToString() },
                { "Name",     c => c.ConName ?? "" },
                { "IauAbbr",  c => c.IauAbbr ?? "" },
                { "NasaAbbr", c => c.NasaAbbr ?? "" },
                { "Genitive", c => c.Genitive ?? "" },
                { "Origin",   c => c.Origin ?? "" }
            });
    }
}
