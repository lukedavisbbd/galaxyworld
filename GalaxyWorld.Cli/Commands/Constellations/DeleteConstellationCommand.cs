using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Constellations;

public class DeleteConstellationCommand : DeleteEntityByIdCommand<Constellation>
{
    protected override string Path => Routes.Constellations;
}
