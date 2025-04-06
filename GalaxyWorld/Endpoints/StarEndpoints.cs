using System.Security.Claims;
using System.Text.Json;
using GalaxyWorld.Auth;
using GalaxyWorld.Models;
using GalaxyWorld.Models.Dbos;
using GalaxyWorld.Services;
using Npgsql;

namespace GalaxyWorld.Endpoints;

public static class StarEndpoints
{
    public static async Task<IResult> GetStars(StarService service, int start = 0, int length = 100)
    {
        return Results.Ok(await service.GetStars(new Page
        {
            Length = int.Max(length, 0),
            Start = int.Max(start, 0),
        }));
    }

    public static async Task<IResult> GetStar(StarService service, int id)
    {
        var star = await service.GetStar(id);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> PostStar(StarService service, StarInsert insert)
    {
        var star = await service.CreateStar(insert);
        return Results.Ok(star);
    }

    public static async Task<IResult> PatchStar(StarService service, int id, StarPatch patch)
    {
        var star = await service.PatchStar(id, patch);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static IEndpointRouteBuilder UseStarEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/stars", GetStars)
            .RequireAuthorization();
        
        routes.MapGet("/stars/{id:int}", GetStar)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        
        routes.MapPost("/stars", PostStar)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPatch("/stars/{id:int}", PatchStar)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        return routes;
    }
}
