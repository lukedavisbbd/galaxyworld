namespace GalaxyWorld.API.Endpoints;

public static class EndpointConfiguration
{
    public static IEndpointRouteBuilder ConfigureEndpoints(this IEndpointRouteBuilder routes)
    {
        return routes
            .UseStarEndpoints()
            .UseCatalogueEndpoints()
            .UseConstellationEndpoints();
    }
}
