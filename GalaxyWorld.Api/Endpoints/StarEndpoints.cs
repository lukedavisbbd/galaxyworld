using GalaxyWorld.API.Auth;
using GalaxyWorld.Core.Models.CatalogueEntry;
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

    public static async Task<IResult> GetStar(StarService service, int id)
    {
        var star = await service.GetOne(id);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> GetStarCatalogueEntries(CatalogueStarEntryService service, int id, int start = 0, int length = 100, CatalogueStarEntrySort sort = default)
    {
        var star = await service.GetByStar(id, new Page
        {
            Start = start,
            Length = length,
        }, sort);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> PostStarCatalogueEntry(CatalogueStarEntryService service, int id, CatalogueStarEntryStar insert)
    {
        var entry = await service.Create(new CatalogueStarEntry
        {
            CatId = insert.CatId,
            StarId = id,
            EntryId = insert.EntryId,
        });
        return Results.Ok(entry);
    }

    public static async Task<IResult> PatchStarCatalogueEntry(CatalogueStarEntryService service, int id, CatalogueStarEntryStar patch)
    {
        var entry = await service.Patch(new CatalogueStarEntry
        {
            CatId = patch.CatId,
            StarId = id,
            EntryId = patch.EntryId,
        });
        if (entry == null) return Results.NotFound();
        return Results.Ok(entry);
    }

    public static async Task<IResult> DeleteStarCatalogueEntry(CatalogueStarEntryService service, int id, int catId)
    {
        var entry = await service.DeleteByStar(id, catId);
        if (entry == null) return Results.NotFound();
        return Results.Ok(entry);
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

    public static async Task<IResult> PatchStar(StarService service, int id, StarPatch patch)
    {
        var star = await service.Patch(id, patch);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> DeleteStar(StarService service, int id)
    {
        var star = await service.Delete(id);
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

        routes.MapGet("/stars/{id:int}/catentries", GetStarCatalogueEntries)
            .RequireAuthorization();

        routes.MapPost("/stars/{id:int}/catentries", PostStarCatalogueEntry)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapPatch("/stars/{id:int}/catentries", PatchStarCatalogueEntry)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/stars/{id:int}/catentries", DeleteStarCatalogueEntry)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapPost("/stars", PostStar)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPost("/stars/bulk", PostStars)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapPatch("/stars/{id:int}", PatchStar)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/stars/{id:int}", DeleteStar)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        return routes;
    }
}
