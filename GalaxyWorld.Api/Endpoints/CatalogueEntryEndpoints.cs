using GalaxyWorld.API.Auth;
using GalaxyWorld.API.Services;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyWorld.API.Endpoints;

public static class CatalogueEntryEndpoints
{
    public static async Task<IResult> GetCatalogueEntries(CatalogueService catalogueService, CatalogueEntryService entryService, int catalogueId, [FromQuery] Filter<CatalogueEntry>[] filter, int start = 0, int length = 100, CatalogueEntrySort sort = default)
    {
        if (await catalogueService.GetOne(catalogueId) == null)
            return Results.NotFound();
        
        var entries = await entryService.GetByCatalogue(catalogueId, new Page
        {
            Start = start,
            Length = length,
        }, sort, filter);
        
        if (entries == null) return Results.NotFound();
        
        return Results.Ok(entries);
    }

    public static async Task<IResult> GetStarEntries(StarService starService, CatalogueEntryService entryService, int starId, [FromQuery] Filter<CatalogueEntry>[] filter, int start = 0, int length = 100, CatalogueEntrySort sort = default)
    {
        if (await starService.GetOne(starId) == null)
            return Results.NotFound();
        
        var star = await entryService.GetByStar(starId, new Page
        {
            Start = start,
            Length = length,
        }, sort, filter);
        if (star == null) return Results.NotFound();
        return Results.Ok(star);
    }

    public static async Task<IResult> PostEntry(CatalogueService catalogueService, StarService starService, CatalogueEntryService entryService, int catalogueId, int starId, CatalogueEntryInsert insert)
    {
        if (await catalogueService.GetOne(catalogueId) == null)
            return Results.NotFound();
        if (await starService.GetOne(starId) == null)
            return Results.NotFound();
        
        var entry = await entryService.Create(catalogueId, starId, insert);
        return Results.Ok(entry);
    }

    public static async Task<IResult> GetEntry(CatalogueEntryService service, int catalogueId, int starId)
    {
        var entries = await service.GetOne(catalogueId, starId);
        if (entries == null) return Results.NotFound();
        return Results.Ok(entries);
    }

    public static async Task<IResult> PatchEntry(CatalogueEntryService service, int catalogueId, int starId, CatalogueEntryPatch patch)
    {
        var entry = await service.Patch(catalogueId, starId, patch);
        if (entry == null) return Results.NotFound();
        return Results.Ok(entry);
    }

    public static async Task<IResult> DeleteEntry(CatalogueEntryService service, int catalogueId, int starId)
    {
        var entry = await service.Delete(catalogueId, starId);
        if (entry == null) return Results.NotFound();
        return Results.Ok(entry);
    }

    public static IEndpointRouteBuilder UseCatalogueEntryEndpoints(this IEndpointRouteBuilder routes)
    {
        const string CATALOGUE_TAG = "CatalogueEndpoints";
        const string STAR_TAG = "StarEndpoints";

        routes.MapGet("/catalogues/{catalogueId:int}/stars", GetCatalogueEntries)
            .Produces<IEnumerable<CatalogueEntry>>()
            .RequireAuthorization()
            .WithTags(CATALOGUE_TAG);
        routes.MapGet("/stars/{starId:int}/catalogues", GetStarEntries)
            .Produces<IEnumerable<CatalogueEntry>>()
            .RequireAuthorization()
            .WithTags(STAR_TAG);

        routes.MapGet("/catalogues/{catalogueId:int}/stars/{starId:int}", GetEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .WithTags(CATALOGUE_TAG);
        routes.MapGet("/stars/{starId:int}/catalogues/{catalogueId:int}", GetEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .WithTags(STAR_TAG);

        routes.MapPost("/catalogues/{catalogueId:int}/stars/{starId:int}", PostEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(CATALOGUE_TAG);
        routes.MapPost("/stars/{starId:int}/catalogues/{catalogueId:int}", PostEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(STAR_TAG);

        routes.MapPatch("/catalogues/{catalogueId:int}/stars/{starId:int}", PatchEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(CATALOGUE_TAG);
        routes.MapPatch("/stars/{starId:int}/catalogues/{catalogueId:int}", PatchEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(STAR_TAG);

        routes.MapDelete("/catalogues/{catalogueId:int}/stars/{starId:int}", DeleteEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(CATALOGUE_TAG);
        routes.MapDelete("/stars/{starId:int}/catalogues/{catalogueId:int}", DeleteEntry)
            .Produces<CatalogueEntry>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName)
            .WithTags(STAR_TAG);

        return routes;
    }
}
