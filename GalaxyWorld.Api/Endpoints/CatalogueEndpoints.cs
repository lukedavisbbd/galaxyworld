using GalaxyWorld.API.Auth;
using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.API.Services;
using GalaxyWorld.Core.Models;

namespace GalaxyWorld.API.Endpoints;

public static class CatalogueEndpoints
{
    public static async Task<IResult> GetCatalogues(CatalogueService service, int start = 0, int length = 100, CatalogueSort sort = default)
    {
        return Results.Ok(await service.Get(new Page
        {
            Length = int.Max(length, 0),
            Start = int.Max(start, 0),
        }, sort));
    }

    public static async Task<IResult> GetCatalogue(CatalogueService service, int id)
    {
        var catalogue = await service.GetOne(id);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static async Task<IResult> GetCatalogueStars(CatalogueStarEntryService service, int id, int start = 0, int length = 100, CatalogueStarEntrySort sort = default)
    {
        var entries = await service.GetByCatalogue(id, new Page
        {
            Start = start,
            Length = length,
        }, sort);
        if (entries == null) return Results.NotFound();
        return Results.Ok(entries);
    }

    public static async Task<IResult> PostCatalogueStar(CatalogueStarEntryService service, int id, CatalogueStarEntryCatalogue insert)
    {
        var entry = await service.Create(new CatalogueStarEntry
        {
            CatId = id,
            StarId = insert.StarId,
            EntryId = insert.EntryId,
        });
        return Results.Ok(entry);
    }

    public static async Task<IResult> PatchCatalogueStar(CatalogueStarEntryService service, int id, CatalogueStarEntryCatalogue patch)
    {
        var entry = await service.Patch(new CatalogueStarEntry
        {
            CatId = id,
            StarId = patch.StarId,
            EntryId = patch.EntryId,
        });
        if (entry == null) return Results.NotFound();
        return Results.Ok(entry);
    }

    public static async Task<IResult> DeleteCatalogueStar(CatalogueStarEntryService service, int id, string entryId)
    {
        var entry = await service.DeleteByCatelogue(id, entryId);
        if (entry == null) return Results.NotFound();
        return Results.Ok(entry);
    }

    public static async Task<IResult> PostCatalogue(CatalogueService service, CatalogueInsert insert)
    {
        var catalogue = await service.Create(insert);
        return Results.Ok(catalogue);
    }
    
    public static async Task<IResult> PatchCatalogue(CatalogueService service, int id, CataloguePatch patch)
    {
        var catalogue = await service.Patch(id, patch);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static async Task<IResult> DeleteCatalogue(CatalogueService service, int id)
    {
        var catalogue = await service.Delete(id);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static IEndpointRouteBuilder UseCatalogueEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/catalogues", GetCatalogues)
            .RequireAuthorization();

        routes.MapGet("/catalogues/{id:int}", GetCatalogue)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        routes.MapGet("/catalogues/{id:int}/stars", GetCatalogueStars)
            .RequireAuthorization();

        routes.MapPost("/catalogues/{id:int}/stars", PostCatalogueStar)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapPatch("/catalogues/{id:int}/stars", PatchCatalogueStar)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/catalogues/{id:int}/stars", DeleteCatalogueStar)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapPost("/catalogues", PostCatalogue)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPatch("/catalogues/{id:int}", PatchCatalogue)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/catalogues/{id:int}", DeleteCatalogue)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        return routes;
    }
}
