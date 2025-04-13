namespace GalaxyWorld.API.Models;

public class GoogleAuthRequest
{
    public required string Code { get; init; }
    public required string RedirectUri { get; init; }
    public required string GrantType { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}
