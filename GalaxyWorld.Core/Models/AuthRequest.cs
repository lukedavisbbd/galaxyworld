namespace GalaxyWorld.Core.Models;

public class AuthRequest
{
    public required string Code { get; init; }
    public required string Uri { get; init; }
}
