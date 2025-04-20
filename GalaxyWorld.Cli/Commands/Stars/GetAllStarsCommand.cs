using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.Cli.Commands.Stars;

public class GetAllStarsCommand : GetEntitiesWithQueryCommand<Star, StarSort>
{
    protected override string Path => Routes.Stars;
}
