using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Constellations;

public class GetConstellationStarsCommand : GetStarsByParentIdCommand<StarSort>
{
    protected override string SubPath => Routes.Constellations;
}
