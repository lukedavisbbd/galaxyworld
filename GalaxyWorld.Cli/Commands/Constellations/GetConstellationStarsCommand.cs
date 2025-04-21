using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Cli.Helper;

namespace GalaxyWorld.Cli.Commands.Constellations;

public class GetConstellationStarsCommand : GetStarsByParentIdCommand<Star, StarSort>
{
    protected override string SubPath => Routes.Constellations;

    protected override void Display(IEnumerable<Star> stars, string sortBy)
    {
        TableHelper.Print(
            items: stars,
            title: $"Stars (Sorted by {FormatHelper.PascalToTitleCase(sortBy)})",
            columns: new()
            {
                { "Star ID",         s => s.StarId.ToString() },
                { "Constellation",   s => s.Constellation?.ToString() ?? "" },
                { "Proper Name",     s => s.ProperName ?? "" },
                { "Distance",        s => s.Distance?.ToString() ?? "" },
                { "Magnitude",       s => s.Magnitude.ToString() ?? "" },
                { "Spectral Type",   s => s.SpectralType ?? "" }
            });
    }
}
