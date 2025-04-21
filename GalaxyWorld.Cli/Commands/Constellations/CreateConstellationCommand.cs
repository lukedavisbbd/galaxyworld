using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Constellations;

public class CreateConstellationCommand : CreateEntityCommand<ConstellationInsert, Constellation>
{
    protected override string Path => Routes.Constellations;

    protected override ConstellationInsert BuildModel()
    {
        return ModelHelper.PromptModel<ConstellationInsert>();
    }
}
