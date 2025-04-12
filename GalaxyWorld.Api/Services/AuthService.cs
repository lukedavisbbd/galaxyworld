using System.Text.Json;
using GalaxyWorld.API.Auth;
using GalaxyWorld.API.Models;
using GalaxyWorld.Core.Models;
using Microsoft.Extensions.Options;

namespace GalaxyWorld.API.Services;

public class AuthService(HttpClient client, IOptions<AuthOptions> options)
{
    private static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public async Task<AuthResponse> PostAuth(AuthRequest request)
    {
        var resp = await client.PostAsJsonAsync(options.Value.TokenUri, new GoogleAuthRequest
        {
            Code = request.Code,
            RedirectUri = request.Uri,
            ClientId = options.Value.ClientId,
            ClientSecret = options.Value.ClientSecret,
            GrantType = "authorization_code",
        }, JsonOptions);

        return (await resp.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions)) ?? throw new JsonException();
    }
}
