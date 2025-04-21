using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.Commands.Stars;

public class GetStarByIdCommand : GetEntityByIdCommand<Star>
{
    protected override string Path => Routes.Stars;
}
