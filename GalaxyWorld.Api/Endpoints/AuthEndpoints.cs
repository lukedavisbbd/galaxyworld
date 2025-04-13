using GalaxyWorld.Core.Models;
using GalaxyWorld.API.Services;

namespace GalaxyWorld.API.Endpoints;

public static class AuthEndpoints
{
    public static async Task<IResult> PostAuth(AuthRequest request, AuthService service) {
        var response = await service.PostAuth(request);
        return Results.Ok(response);
    }

    public static IEndpointRouteBuilder UseAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/auth", PostAuth)
            .Produces<AuthResponse>();
        return routes;
    }

}
