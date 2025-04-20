using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class UpdateCatalogueCommand : UpdateEntityCommand<CataloguePatch, Catalogue>
{
    protected override string Path => Routes.Catalogues;

    protected override CataloguePatch BuildPatch()
    {
        return ModelHelper.PromptModel<CataloguePatch>();
    }
}
