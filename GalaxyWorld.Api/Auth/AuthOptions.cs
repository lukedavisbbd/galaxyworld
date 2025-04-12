namespace GalaxyWorld.API.Auth;

public record AuthOptions
{
    public const string Section = "Auth";
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string TokenUri { get; init; }
}
