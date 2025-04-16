using Dapper;
using GalaxyWorld.Core.Models;

namespace GalaxyWorld.API.Database;

public static class FilterExt
{
    public static string ToSql<T>(this Filter<T> filter, string columnName, int index)
    {
        switch (filter.FilterOp)
        {
            case FilterOp.Eq:
                if (filter.Value?.GetType() == typeof(string))
                    return $"{columnName} ILIKE @filter{index}";
                else
                    return $"{columnName} = @filter{index}";
            case FilterOp.Neq:
                if (filter.Value?.GetType() == typeof(string))
                    return $"{columnName} NOT ILIKE @filter{index}";
                else
                    return $"{columnName} != @filter{index}";
            case FilterOp.Gt:
                return $"{columnName} > @filter{index}";
            case FilterOp.Lt:
                return $"{columnName} < @filter{index}";
            case FilterOp.Gte:
                return $"{columnName} >= @filter{index}";
            case FilterOp.Lte:
                return $"{columnName} <= @filter{index}";
            default:
                return "";
        }
    }

    public static string ToSql<T>(this Filter<T>[] filters, FilterPrepend prepend = FilterPrepend.None)
    {
        var filterSql = filters.Select((filter, index) => {
            return filter.ToSql(DbConstants.MapPropertyName(filter.PropName), index);
        }).Where(f => !string.IsNullOrEmpty(f));

        var sqlArray = filterSql.ToArray();

        if (sqlArray.Length == 0) return "";

        var prependSql = prepend switch
        {
            FilterPrepend.And => "AND ",
            FilterPrepend.Where => "WHERE ",
            _ => "",
        };

        return prependSql + string.Join(" AND ", sqlArray);
    }

    public static DynamicParameters ToParams<T>(this Filter<T>[] filters)
    {
        var parameters = new DynamicParameters();

        for (int i = 0; i < filters.Length; i++)
            parameters.Add($"filter{i}", filters[i].Value);

        return parameters;
    }
}
