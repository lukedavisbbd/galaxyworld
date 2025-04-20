using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class GetCatalogueStarsCommand : GetStarsByParentIdCommand<CatalogueEntry, CatalogueEntrySort>
{
    protected override string SubPath => Routes.Catalogues;
}
