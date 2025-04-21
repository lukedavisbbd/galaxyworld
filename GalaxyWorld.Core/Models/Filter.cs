using System.Diagnostics.CodeAnalysis;

namespace GalaxyWorld.Core.Models;

/// <summary>
/// Filter must be in the format "{PropName}.{Op}.{Value}", where PropName is a pascal case property name of
/// the response object, Op is Eq, Neq, Gt, Lt, Gte, or Lte, and Value is some value that can be parsed to the 
/// type of the property.
/// 
/// Eq and Neq map to LIKE and NOT LIKE respectively if the property type is string. So selections like
/// ConstellationName.Eq.An%25 will select all constellations starting with "An". '%25' maps to '%', to match a single
/// character use '_'.
/// 
/// Multiple filters are allowed and all must be true for an object to be included in the response.
/// 
/// Examples:
/// 
/// GET /stars?filter=ProperName.Eq.So_
/// 
/// GET /stars?filter=StarId.Gt.10&filter=StarId.Lt.200
/// </summary>
/// <typeparam name="T">Type of the response object.</typeparam>
public class Filter<T> : IParsable<Filter<T>>
{
    public required string PropName { get; init; }
    public required FilterOp FilterOp { get; init; }
    public required object? Value { get; init; }

    public override string ToString() {
        return $"{PropName}.{FilterOp}.{Value}";
    }

    public static Filter<T> Parse(string s, IFormatProvider? provider)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));
        if (TryParse(s, provider, out var result))
            return result;
        throw new FormatException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Filter<T> result)
    {
        if (s == null)
        {
            result = null;
            return false;
        }

        var thisType = typeof(Filter<T>);
        var filterType = typeof(T);

        // attempt to match filter parameters to dbo properties
        foreach (var filterProp in filterType.GetProperties())
        {
            if (s.StartsWith(filterProp.Name + '.'))
            {
                s = s.Substring(filterProp.Name.Length + 1);

                var i = s.IndexOf('.');
                if (i == -1) break;

                var opStr = s.Substring(0, i);
                FilterOp op;
                if (!Enum.TryParse(opStr, out op)) break;

                s = s.Substring(opStr.Length + 1);

                object? value = null;

                if (filterProp.PropertyType == typeof(string))
                {
                    value = s;
                }
                else
                {
                    var propType = filterProp.PropertyType;
                    // if nullable, get inner type
                    if (filterProp.PropertyType.IsGenericType && filterProp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        propType = propType.GenericTypeArguments[0];

                    var tryParse = propType.GetMethod("TryParse", [typeof(string), typeof(IFormatProvider), propType.MakeByRefType()]);

                    if (tryParse == null) break;

                    // params: string, IFormatProvider?, out PropType
                    var parameters = new object?[] { s, provider, null };
                    var success = tryParse.Invoke(null, parameters);

                    if (success is not true) break;

                    value = parameters[2];
                }

                result = new Filter<T> { 
                    PropName = filterProp.Name,
                    FilterOp = op,
                    Value = value,
                };
                return true;
            }
        }

        result = null;
        return false;
    }
}

public enum FilterOp
{
    Eq,
    Neq,
    Gt,
    Lt,
    Gte,
    Lte,
}
