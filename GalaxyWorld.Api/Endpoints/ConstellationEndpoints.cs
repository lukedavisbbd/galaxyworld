using GalaxyWorld.API.Auth;
using GalaxyWorld.Core.Models.Constellation;
using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.API.Services;
using GalaxyWorld.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyWorld.API.Endpoints;

public static class ConstellationEndpoints
{
    public static async Task<IResult> GetConstellations(ConstellationService service, [FromQuery] Filter<Constellation>[] filter, int start = 0, int length = 100, ConstellationSort sort = default)
    {
        return Results.Ok(await service.Get(new Page
        {
            Length = int.Max(length, 0),
            Start = int.Max(start, 0),
        }, sort, filter));
    }

    public static async Task<IResult> GetConstellation(ConstellationService service, int conId)
    {
        var constellation = await service.GetOne(conId);
        if (constellation == null) return Results.NotFound();
        return Results.Ok(constellation);
    }

    public static async Task<IResult> GetConstellationStars(ConstellationService constellationService, StarService starService, [FromQuery] Filter<Star>[] filter, int conId, int start = 0, int length = 100, StarSort sort = default)
    {
        if (await constellationService.GetOne(conId) == null)
            return Results.NotFound();
        
        var stars = await starService.GetByConstellation(conId, new Page
        {
            Start = start,
            Length = length,
        }, sort, filter);
        return Results.Ok(stars);
    }

    public static async Task<IResult> PostConstellation(ConstellationService service, ConstellationInsert insert)
    {
        var constellation = await service.Create(insert);
        return Results.Ok(constellation);
    }
    
    public static async Task<IResult> PatchConstellation(ConstellationService service, int conId, ConstellationPatch patch)
    {
        var constellation = await service.Patch(conId, patch);
        if (constellation == null) return Results.NotFound();
        return Results.Ok(constellation);
    }

    public static async Task<IResult> DeleteConstellation(ConstellationService service, int conId)
    {
        var constellation = await service.Delete(conId);
        if (constellation == null) return Results.NotFound();
        return Results.Ok(constellation);
    }

    public static IEndpointRouteBuilder UseConstellationEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/constellations", GetConstellations)
            .Produces<IEnumerable<Constellation>>()
            .RequireAuthorization();

        routes.MapGet("/constellations/{conId:int}", GetConstellation)
            .Produces<Constellation>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        routes.MapGet("/constellations/{conId:int}/stars", GetConstellationStars)
            .Produces<IEnumerable<Star>>()
            .RequireAuthorization();

        routes.MapPost("/constellations", PostConstellation)
            .Produces<Constellation>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPatch("/constellations/{conId:int}", PatchConstellation)
            .Produces<Constellation>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/constellations/{conId:int}", DeleteConstellation)
            .Produces<Constellation>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        return routes;
    }
}
