using Dapper;
using GalaxyWorld.Models;
using GalaxyWorld.Models.Constellation;

namespace GalaxyWorld.Database;

public class ConstellationRepository(DbContext db)
{
    public async Task<IEnumerable<Constellation>> Fetch(Page page, ConstellationSort sort)
    {
        Console.WriteLine("SELECT * FROM constellations LIMIT @length OFFSET @start " + sort.ToSql());
        var conn = db.Connection;
        var constellations = await conn.QueryAsync<Constellation>($"SELECT * FROM constellations {sort.ToSql()} LIMIT @Length OFFSET @Start", page);
        return constellations;
    }

    public async Task<Constellation?> FetchOne(int conId)
    {
        var conn = db.Connection;
        var constellation = await conn.QueryFirstOrDefaultAsync<Constellation>("SELECT * FROM constellations WHERE con_id = @conId", new { conId });
        return constellation;
    }

    public async Task<Constellation> Insert(ConstellationInsert insert)
    {
        var conn = db.Connection;
        var constellation = await conn.QueryFirstAsync<Constellation>(
            """
            INSERT INTO constellations (
                con_name,
                iau_abbr,
                nasa_abbr,
                genitive,
                origin,
                meaning
            ) VALUES (
                @ConName,
                @IauAbbr,
                @NasaAbbr,
                @Genitive,
                @Origin,
                @Meaning
            ) RETURNING *
            """,
            insert
        );
        return constellation;
    }

    public async Task<Constellation?> Update(int conId, ConstellationPatch patch)
    {
        var conn = db.Connection;

        var changes = patch.ToSql();

        if (string.IsNullOrEmpty(changes))
            return await FetchOne(conId);

        var constellation = await conn.QueryFirstOrDefaultAsync<Constellation>(
            "UPDATE constellations SET " +
            changes + " WHERE con_id = @conId RETURNING *", new
            {
                conId,
                ConName = patch.ConName.Or(),
                IauAbbr = patch.IauAbbr.Or(),
                NasaAbbr = patch.NasaAbbr.Or(),
                Genitive = patch.Genitive.Or(),
                Origin = patch.Origin.Or(),
                Meaning = patch.Meaning.Or(),
            }
        );
        return constellation;
    }

    public async Task<Constellation?> Delete(int conId)
    {
        var conn = db.Connection;
        var constellation = await conn.QueryFirstOrDefaultAsync<Constellation>("DELETE FROM constellations WHERE con_id = @conId RETURNING *", new { conId });
        return constellation;
    }
}
