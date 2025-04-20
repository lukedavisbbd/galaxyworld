using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Core.Models.Catalogue;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class GetAllCataloguesCommand : GetEntitiesWithQueryCommand<Catalogue, CatalogueSort>
{
    protected override string Path => Routes.Catalogues;
}
