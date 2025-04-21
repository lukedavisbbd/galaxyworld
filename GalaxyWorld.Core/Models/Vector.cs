using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace GalaxyWorld.Core.Models;

public struct Vector : IParsable<Vector>
{
    public required decimal X { get; set; }
    public required decimal Y { get; set; }
    public required decimal Z { get; set; }

    public static Vector Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var result))
            throw new FormatException();
        return result;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Vector result)
    {
        if (s == null)
        {
            result = default;
            return false;
        }
        var splits = s.Split(",");
        if (splits.Length != 3)
        {
            result = default;
            return false;
        }

        if (!decimal.TryParse(splits[0].Trim(), provider, out decimal x))
        {
            result = default;
            return false;
        }
        if (!decimal.TryParse(splits[1].Trim(), provider, out decimal y))
        {
            result = default;
            return false;
        }
        if (!decimal.TryParse(splits[2].Trim(), provider, out decimal z))
        {
            result = default;
            return false;
        }

        result = new Vector
        {
            X = x,
            Y = y,
            Z = z,
        };
        return true;
    }

    public override string ToString()
    {
        return $"{X.ToString(CultureInfo.InvariantCulture)}, {Y.ToString(CultureInfo.InvariantCulture)}, {Z.ToString(CultureInfo.InvariantCulture)}";
    }
}
