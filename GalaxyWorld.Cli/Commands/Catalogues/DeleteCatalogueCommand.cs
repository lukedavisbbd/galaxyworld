using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class DeleteCatalogueCommand : DeleteEntityByIdCommand<Catalogue>
{
    protected override string Path => Routes.Catalogues;
}
