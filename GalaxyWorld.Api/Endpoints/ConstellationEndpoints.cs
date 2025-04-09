using GalaxyWorld.API.Auth;
using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.API.Services;
using GalaxyWorld.Core.Models;

namespace GalaxyWorld.API.Endpoints;

public static class ConstellationEndpoints
{
    public static async Task<IResult> GetConstellations(ConstellationService service, int start = 0, int length = 100, ConstellationSort sort = default)
    {
        return Results.Ok(await service.Get(new Page
        {
            Length = int.Max(length, 0),
            Start = int.Max(start, 0),
        }, sort));
    }

    public static async Task<IResult> GetConstellation(ConstellationService service, int id)
    {
        var constellation = await service.GetOne(id);
        if (constellation == null) return Results.NotFound();
        return Results.Ok(constellation);
    }

    public static async Task<IResult> GetConstellationStars(StarService service, int id, int start = 0, int length = 100, StarSort sort = default)
    {
        var stars = await service.GetByConstellation(id, new Page
        {
            Start = start,
            Length = length,
        }, sort);
        return Results.Ok(stars);
    }

    public static async Task<IResult> PostConstellation(ConstellationService service, ConstellationInsert insert)
    {
        var constellation = await service.Create(insert);
        return Results.Ok(constellation);
    }
    
    public static async Task<IResult> PatchConstellation(ConstellationService service, int id, ConstellationPatch patch)
    {
        var constellation = await service.Patch(id, patch);
        if (constellation == null) return Results.NotFound();
        return Results.Ok(constellation);
    }

    public static async Task<IResult> DeleteConstellation(ConstellationService service, int id)
    {
        var constellation = await service.Delete(id);
        if (constellation == null) return Results.NotFound();
        return Results.Ok(constellation);
    }

    public static IEndpointRouteBuilder UseConstellationEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/constellations", GetConstellations)
            .RequireAuthorization();

        routes.MapGet("/constellations/{id:int}", GetConstellation)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        routes.MapGet("/constellations/{id:int}/stars", GetConstellationStars)
            .RequireAuthorization();

        routes.MapPost("/constellations", PostConstellation)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPatch("/constellations/{id:int}", PatchConstellation)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/constellations/{id:int}", DeleteConstellation)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        return routes;
    }
}
