using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Stars;

public class CreateStarCommand : CreateEntityCommand<StarInsert, Star>
{
    protected override string Path => Routes.Stars;

    protected override StarInsert BuildModel()
    {
        return ModelHelper.PromptModel<StarInsert>([nameof(StarInsert.CatalogueEntries)]);
    }
}
