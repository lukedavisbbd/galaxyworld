using GalaxyWorld.API.Auth;
using GalaxyWorld.API.Services;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.API.Endpoints;

public static class CatalogueEntryEndpoints
{
    public static async Task<IResult> GetCatalogueEntries(CatalogueEntryService service, int catId, int start = 0, int length = 100, CatalogueEntrySort sort = default)
    {
        var entries = await service.GetByCatalogue(catId, new Page
        {
            Start = start,
            Length = length,
        }, sort);
        if (entries == null) return Results.NotFound();
        return Results.Ok(entries);
    }

    public static async Task<IResult> GetStarEntries(CatalogueEntryService service, int starId, int start = 0, int length = 100, CatalogueEntrySort sort = default)
    {
        var star = await service.GetByStar(starId, new Page
        {
            Start = start,
            Length = length,
        }, sort);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> PostEntry(CatalogueEntryService service, int catId, int starId, CatalogueEntryInsert insert)
    {
        var entry = await service.Create(catId, starId, insert);
        return Results.Ok(entry);
    }

    public static async Task<IResult> GetEntry(CatalogueEntryService service, int catId, int starId)
    {
        var entries = await service.GetOne(catId, starId);
        if (entries == null) return Results.NotFound();
        return Results.Ok(entries);
    }

    public static async Task<IResult> PatchEntry(CatalogueEntryService service, int catId, int starId, CatalogueEntryPatch patch)
    {
        var entry = await service.Patch(catId, starId, patch);
        if (entry == null) return Results.NotFound();
        return Results.Ok(entry);
    }

    public static async Task<IResult> DeleteEntry(CatalogueEntryService service, int catId, int starId)
    {
        var entry = await service.Delete(catId, starId);
        if (entry == null) return Results.NotFound();
        return Results.Ok(entry);
    }

    public static IEndpointRouteBuilder UseCatalogueEntryEndpoints(this IEndpointRouteBuilder routes)
    {
        const string CATALOGUE_TAG = "CatalogueEndpoints";
        const string STAR_TAG = "StarEndpoints";

        routes.MapGet("/catalogues/{catId:int}/stars", GetCatalogueEntries)
            .Produces<IEnumerable<CatalogueEntry>>()
            .RequireAuthorization()
            .WithTags(CATALOGUE_TAG);
        routes.MapGet("/stars/{starId:int}/catalogues", GetStarEntries)
            .Produces<IEnumerable<CatalogueEntry>>()
            .RequireAuthorization()
            .WithTags(STAR_TAG);

        routes.MapGet("/catalogues/{catId:int}/stars/{starId:int}", GetEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .WithTags(CATALOGUE_TAG);
        routes.MapGet("/stars/{starId:int}/catalogues/{catId:int}", GetEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .WithTags(STAR_TAG);

        routes.MapPost("/catalogues/{catId:int}/stars/{starId:int}", PostEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(CATALOGUE_TAG);
        routes.MapPost("/stars/{starId:int}/catalogues/{catId:int}", PostEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(STAR_TAG);

        routes.MapPatch("/catalogues/{catId:int}/stars/{starId:int}", PatchEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(CATALOGUE_TAG);
        routes.MapPatch("/stars/{starId:int}/catalogues/{catId:int}", PatchEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(STAR_TAG);

        routes.MapDelete("/catalogues/{catId:int}/stars/{starId:int}", DeleteEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(CATALOGUE_TAG);
        routes.MapDelete("/stars/{starId:int}/catalogues/{catId:int}", DeleteEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(STAR_TAG);

        return routes;
    }
}
