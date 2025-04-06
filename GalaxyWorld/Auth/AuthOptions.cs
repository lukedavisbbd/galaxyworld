namespace GalaxyWorld.Auth;

public record AuthOptions
{
    public const string Section = "Auth";
    public required string ClientId { get; init; }
}
