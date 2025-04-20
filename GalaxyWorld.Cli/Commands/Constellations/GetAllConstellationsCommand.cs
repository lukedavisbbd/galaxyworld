using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Core.Models.Constellation;

namespace GalaxyWorld.Cli.Commands.Constellations;

public class GetAllConstellationsCommand : GetEntitiesWithQueryCommand<Constellation, ConstellationSort>
{
    protected override string Path => Routes.Constellations;
}
