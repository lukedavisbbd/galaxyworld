using GalaxyWorld.API.Database;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace GalaxyWorld.API.ErrorHandling;

public static class ErrorHandlingConfiguration
{
    private static ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

    public static IServiceCollection ConfigureErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.Map<PostgresException>(ex =>
            {
                return MapDbError(ex);
            });
            options.Map<BadHttpRequestException>(ex =>
            {
                return new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                };
            });
        });
        return services;
    }

    private static ProblemDetails MapDbError(PostgresException ex)
    {
        if (ex.SqlState == "23502")
        {
            var detail = string.IsNullOrEmpty(ex.ColumnName) ? "Unexpected null value." : DbConstants.MapConstraintName($"{ex.ColumnName}_null");
            return new ProblemDetails
            {
                Title = "Null Constraint Failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = detail,
            };
        }

        if (ex.ConstraintName != null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Constraint Failed",
                Detail = DbConstants.MapConstraintName(ex.ConstraintName),
            };
        }

        if (ex.SqlState.Substring(0, 2) == "23")
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Constraint Failed",
                Detail = "Operation would leave database in an inconsistent state.",
            };
        }

        if (ex.SqlState == "22001") {
            var detail = string.IsNullOrEmpty(ex.ColumnName) ? "String too long." : DbConstants.MapConstraintName($"{ex.ColumnName}_length");
            return new ProblemDetails
            {
                Title = "Invalid Data",
                Status = StatusCodes.Status400BadRequest,
                Detail = detail,
            };
        }

        if (ex.SqlState.Substring(0, 2) == "22")
        {
            var detail = string.IsNullOrEmpty(ex.ColumnName) ? "Invalid data." : DbConstants.MapConstraintName($"{ex.ColumnName}_invalid");
            return new ProblemDetails
            {
                Title = "Invalid Data",
                Status = StatusCodes.Status400BadRequest,
                Detail = detail,
            };
        }

        return new ProblemDetails
        {
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
        };
    }
}
