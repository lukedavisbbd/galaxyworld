using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Constellations;

public class UpdateConstellationCommand : UpdateEntityCommand<ConstellationPatch, Constellation>
{
    protected override string Path => Routes.Constellations;

    protected override ConstellationPatch BuildPatch()
    {
        return ModelHelper.PromptModel<ConstellationPatch>();
    }
}
