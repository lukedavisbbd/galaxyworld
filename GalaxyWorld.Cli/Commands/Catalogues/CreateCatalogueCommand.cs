using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class CreateCatalogueCommand : CreateEntityCommand<CatalogueInsert, Catalogue>
{
    protected override string Path => Routes.Catalogues;

    protected override CatalogueInsert BuildModel()
    {
        return ModelHelper.PromptModel<CatalogueInsert>();
    }
}
