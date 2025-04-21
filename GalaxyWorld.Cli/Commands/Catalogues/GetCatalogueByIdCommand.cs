using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class GetCatalogueByIdCommand : GetEntityByIdCommand<Catalogue>
{
    protected override string Path => Routes.Catalogues;
}
