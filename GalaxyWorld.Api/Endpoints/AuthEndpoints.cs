using GalaxyWorld.Core.Models;
using GalaxyWorld.API.Services;
using System.Security.Claims;

namespace GalaxyWorld.API.Endpoints;

public static class AuthEndpoints
{
    public static async Task<IResult> GetAuth(AuthService service, ClaimsPrincipal user)
    {
        var googleId = user.FindFirst(ClaimTypes.NameIdentifier);
        if (googleId == null)
            return Results.Unauthorized();
        var response = await service.GetAuth(googleId.Value);
        return Results.Ok(response);
    }

    public static async Task<IResult> PostAuth(AuthRequest request, AuthService service) {
        var response = await service.PostAuth(request);
        return Results.Ok(response);
    }

    public static IEndpointRouteBuilder UseAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/auth", GetAuth)
            .Produces<GetAuthResponse>()
            .RequireAuthorization();
        routes.MapPost("/auth", PostAuth)
            .Produces<AuthResponse>();
        return routes;
    }

}
