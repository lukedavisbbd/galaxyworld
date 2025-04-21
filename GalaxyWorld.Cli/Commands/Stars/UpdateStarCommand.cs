using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Stars;

public class UpdateStarCommand : UpdateEntityCommand<StarPatch, Star>
{
    protected override string Path => Routes.Stars;

    protected override StarPatch BuildPatch()
    {
        return ModelHelper.PromptModel<StarPatch>();
    }
}
