using GalaxyWorld.API.Auth;
using GalaxyWorld.Core.Models.Star;
using GalaxyWorld.API.Services;
using GalaxyWorld.Core.Models;
using Npgsql;
using GalaxyWorld.API.Database;
using Microsoft.AspNetCore.Mvc;
using GalaxyWorld.Core.Models.Planets;

namespace GalaxyWorld.API.Endpoints;

public static class StarEndpoints
{
    public static async Task<IResult> GetStars(StarService service, [FromQuery] Filter<Star>[] filter, int start = 0, int length = 100, StarSort sort = default)
    {
        return Results.Ok(await service.Get(new Page
        {
            Length = int.Max(length, 0),
            Start = int.Max(start, 0),
        }, sort, filter));
    }

    public static async Task<IResult> GetStar(StarService service, int starId)
    {
        var star = await service.GetOne(starId);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> GetStarPlanets(PlanetService planetService, int starId)
    {
        var planets = await planetService.GetPlanetarySystem(starId);
        if (planets == null) return Results.NotFound();
        return Results.Ok(planets);
    }

    public static async Task<IResult> PostStar(StarService service, StarInsert insert)
    {
        var star = await service.Create(insert);
        return Results.Ok(star);
    }

    public static async Task<IResult> PostStarsBulk(StarService service, List<StarInsert> inserts, ILogger<Star> logger)
    {
        logger.LogDebug($"received bulk insert request for {inserts.Count} stars");
        var stars = new List<StarBulkResponse>();
        for (var i = 0; i < inserts.Count; i++)
        {
            var insert = inserts[i];
            try
            {
                var star = await service.Create(insert);
                stars.Add(new StarBulkResponse
                {
                    Status = Status.Success,
                    Star = star,
                });
            }
            catch (PostgresException e) when (e.ConstraintName != null)
            {
                stars.Add(new StarBulkResponse
                {
                    Status = Status.Failure,
                    Error = DbConstants.MapConstraintName(e.ConstraintName),
                });
                logger.LogTrace($"star {i}/{inserts.Count} failed constraint {e.ConstraintName}: {e}");
            }
        }
        return Results.Json(stars, statusCode: StatusCodes.Status207MultiStatus);
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

        routes.MapGet("/stars/{starId:int}/planets", GetStarPlanets)
            .Produces<IEnumerable<Planet>>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        routes.MapPost("/stars", PostStar)
            .Produces<Star>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPost("/stars/bulk", PostStarsBulk)
            .Produces<StarBulkResponse>(StatusCodes.Status207MultiStatus)
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
