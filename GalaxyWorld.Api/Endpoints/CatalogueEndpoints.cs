using GalaxyWorld.API.Auth;
using GalaxyWorld.Core.Models.Catalogue;
using GalaxyWorld.API.Services;
using GalaxyWorld.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyWorld.API.Endpoints;

public static class CatalogueEndpoints
{
    public static async Task<IResult> GetCatalogues(CatalogueService service, [FromQuery] Filter<Catalogue>[] filter, int start = 0, int length = 100, CatalogueSort sort = default)
    {
        return Results.Ok(await service.Get(new Page
        {
            Length = int.Max(length, 0),
            Start = int.Max(start, 0),
        }, sort, filter));
    }

    public static async Task<IResult> GetCatalogue(CatalogueService service, int catalogueId)
    {
        var catalogue = await service.GetOne(catalogueId);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static async Task<IResult> PostCatalogue(CatalogueService service, CatalogueInsert insert)
    {
        var catalogue = await service.Create(insert);
        return Results.Ok(catalogue);
    }
    
    public static async Task<IResult> PatchCatalogue(CatalogueService service, int catalogueId, CataloguePatch patch)
    {
        var catalogue = await service.Patch(catalogueId, patch);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static async Task<IResult> DeleteCatalogue(CatalogueEntryService entryService, CatalogueService catalogueService, int catalogueId)
    {
        var entries = await entryService.GetByCatalogue(catalogueId, new Page { Start = 0, Length = 1 }, default, []);
        if (entries.Count() > 0) {
            return Results.Problem("Catalogue is not empty.", null, StatusCodes.Status404NotFound, "Bad Request");
        }

        var catalogue = await catalogueService.Delete(catalogueId);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static IEndpointRouteBuilder UseCatalogueEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/catalogues", GetCatalogues)
            .Produces<IEnumerable<Catalogue>>()
            .RequireAuthorization();

        routes.MapGet("/catalogues/{catalogueId:int}", GetCatalogue)
            .Produces<Catalogue>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        routes.MapPost("/catalogues", PostCatalogue)
            .Produces<Catalogue>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPatch("/catalogues/{catalogueId:int}", PatchCatalogue)
            .Produces<Catalogue>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/catalogues/{catalogueId:int}", DeleteCatalogue)
            .Produces<Catalogue>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        return routes;
    }
}
