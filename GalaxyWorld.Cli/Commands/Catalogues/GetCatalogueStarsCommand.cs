using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class GetCatalogueStarsCommand : GetStarsByParentIdCommand<StarSort>
{
    protected override string SubPath => Routes.Catalogues;
}
