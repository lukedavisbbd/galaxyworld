using System.Data;
using Dapper;
using GalaxyWorld.Core.Models;

namespace GalaxyWorld.API.Database;

public class VectorMapper : SqlMapper.TypeHandler<Vector?>
{
    public override Vector? Parse(object value)
    {
        var array = (decimal[]?)value;
        if (array == null)
            return null;

        return new Vector {
            X = array[0],
            Y = array[1],
            Z = array[2],
        };
    }

    public override void SetValue(IDbDataParameter parameter, Vector? value)
    {
        parameter.Value = value == null ? null : new decimal[3] { value.Value.X, value.Value.Y, value.Value.Z };
    }
}
