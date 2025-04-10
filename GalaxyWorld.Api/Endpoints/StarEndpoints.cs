using GalaxyWorld.API.Auth;
using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.API.Services;
using GalaxyWorld.Core.Models;

namespace GalaxyWorld.API.Endpoints;

public static class StarEndpoints
{
    public static async Task<IResult> GetStars(StarService service, int start = 0, int length = 100, StarSort sort = default)
    {
        return Results.Ok(await service.Get(new Page
        {
            Length = int.Max(length, 0),
            Start = int.Max(start, 0),
        }, sort));
    }

    public static async Task<IResult> GetStar(StarService service, int starId)
    {
        var star = await service.GetOne(starId);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> PostStar(StarService service, StarInsert insert)
    {
        var star = await service.Create(insert);
        return Results.Ok(star);
    }

    public static async Task<IResult> PostStars(StarService service, List<StarInsert> inserts)
    {
        var stars = new List<Star>();
        foreach (var insert in inserts)
        {
            var star = await service.Create(insert);
            stars.Add(star);
        }
        return Results.Ok(stars);
    }

    public static async Task<IResult> PatchStar(StarService service, int starId, StarPatch patch)
    {
        var star = await service.Patch(starId, patch);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> DeleteStar(StarService service, int starId)
    {
        var star = await service.Delete(starId);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static IEndpointRouteBuilder UseStarEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/stars", GetStars)
            .Produces<IEnumerable<Star>>()
            .RequireAuthorization();

        routes.MapGet("/stars/{starId:int}", GetStar)
            .Produces<Star>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        routes.MapPost("/stars", PostStar)
            .Produces<Star>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPost("/stars/bulk", PostStars)
            .Produces<IEnumerable<Star>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapPatch("/stars/{starId:int}", PatchStar)
            .Produces<Star>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/stars/{starId:int}", DeleteStar)
            .Produces<Star>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        return routes;
    }
}
