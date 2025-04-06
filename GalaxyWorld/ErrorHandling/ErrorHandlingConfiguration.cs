using System.Text.Json;
using GalaxyWorld.Database;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace GalaxyWorld.ErrorHandling;

public static class ErrorHandlingConfiguration
{
    public static IServiceCollection ConfigureErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.Map<PostgresException>(ex =>
            {
                if (ex.ConstraintName != null)
                {
                    return new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Detail = DbConstants.MapConstraintName(ex.ConstraintName),
                    };
                }
                return new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                };
            });
        });
        return services;
    }
}
