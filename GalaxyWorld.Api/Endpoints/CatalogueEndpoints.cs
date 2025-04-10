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

    public static async Task<IResult> GetCatalogue(CatalogueService service, int catId)
    {
        var catalogue = await service.GetOne(catId);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static async Task<IResult> PostCatalogue(CatalogueService service, CatalogueInsert insert)
    {
        var catalogue = await service.Create(insert);
        return Results.Ok(catalogue);
    }
    
    public static async Task<IResult> PatchCatalogue(CatalogueService service, int catId, CataloguePatch patch)
    {
        var catalogue = await service.Patch(catId, patch);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static async Task<IResult> DeleteCatalogue(CatalogueService service, int catId)
    {
        var catalogue = await service.Delete(catId);
        if (catalogue == null) return Results.NotFound();
        return Results.Ok(catalogue);
    }

    public static IEndpointRouteBuilder UseCatalogueEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/catalogues", GetCatalogues)
            .Produces<IEnumerable<Catalogue>>()
            .RequireAuthorization();

        routes.MapGet("/catalogues/{catId:int}", GetCatalogue)
            .Produces<Catalogue>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        routes.MapPost("/catalogues", PostCatalogue)
            .Produces<Catalogue>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);
        
        routes.MapPatch("/catalogues/{catId:int}", PatchCatalogue)
            .Produces<Catalogue>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        routes.MapDelete("/catalogues/{catId:int}", DeleteCatalogue)
            .Produces<Catalogue>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(RoleRequirement.RoleAdmin.PolicyName);

        return routes;
    }
}
