using System.Text.Json;
using Dapper;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.API.Database;

public class StarRepository(DbContext db, CatalogueEntryRepository catalogueEntryRepo)
{
    public async Task<IEnumerable<Star>> Fetch(Page page, StarSort sort, Filter<Star>[] filters)
    {
        var parameters = new DynamicParameters(page);
        parameters.AddDynamicParams(filters.ToParams());
        
        var conn = db.Connection;
        var stars = await conn.QueryAsync<Star>($"SELECT * FROM stars {filters.ToSql(FilterPrepend.Where)} {sort.ToSql()} LIMIT @Length OFFSET @Start", parameters);
        
        return stars;
    }

    public async Task<IEnumerable<Star>> FetchByConstellation(int constellation, Page page, StarSort sort, Filter<Star>[] filters)
    {
        var parameters = new DynamicParameters(page);
        parameters.Add("constellation", constellation);
        parameters.AddDynamicParams(filters.ToParams());

        var conn = db.Connection;
        var stars = await conn.QueryAsync<Star>($"SELECT * FROM stars WHERE constellation_id = @constellation {filters.ToSql(FilterPrepend.And)} {sort.ToSql()} LIMIT @Length OFFSET @Start", parameters);
        return stars;
    }

    public async Task<Star?> FetchOne(int id)
    {
        var conn = db.Connection;
        var star = await conn.QueryFirstOrDefaultAsync<Star>("SELECT * FROM stars WHERE star_id = @id", new { id });
        return star;
    }

    public async Task<Star?> FetchStarCatalogues(int id)
    {
        var conn = db.Connection;
        var star = await conn.QueryFirstOrDefaultAsync<Star>("SELECT * FROM stars WHERE star_id = @id", new { id });
        return star;
    }

    public async Task<Star> Insert(StarInsert insert)
    {
        var conn = db.Connection;
        var tx = conn.BeginTransaction();

        try
        {
            var positionCartesianParams = insert.PositionCartesian == null
                ? "null" :
                "ARRAY [@PositionCartesianX, @PositionCartesianY, @PositionCartesianZ]";
            var velocityCartesianParams = insert.VelocityCartesian == null
                ? "null" :
                "ARRAY [@VelocityCartesianX, @VelocityCartesianY, @VelocityCartesianZ]";
            var velocityCircularParams = insert.VelocityCircular == null
                ? "null" :
                "ARRAY [@VelocityCircularRa, @VelocityCircularDecl, @VelocityCircularDistance]";

            var star = await conn.QueryFirstAsync<Star>(
                $"""
                INSERT INTO stars(
                    constellation_id,
                    proper_name,
                    right_ascension,
                    declination,
                    distance,
                    position_cartesian,
                    magnitude,
                    velocity_circular,
                    velocity_cartesian,
                    colour_index,
                    spectral_type
                ) VALUES(
                    @ConstellationId,
                    @ProperName,
                    @RightAscension,
                    @Declination,
                    @Distance,
                    {positionCartesianParams},
                    @Magnitude,
                    {velocityCircularParams},
                    {velocityCartesianParams},
                    @ColourIndex,
                    @SpectralType
                ) RETURNING *
                """,
                new {
                    insert.ConstellationId,
                    insert.ProperName,
                    insert.RightAscension,
                    insert.Declination,
                    insert.Distance,
                    insert.Magnitude,
                    insert.ColourIndex,
                    insert.SpectralType,
                    insert.CatalogueEntries,
                    PositionCartesianX = insert.PositionCartesian?.X,
                    PositionCartesianY = insert.PositionCartesian?.Y,
                    PositionCartesianZ = insert.PositionCartesian?.Z,
                    VelocityCircularRa = insert.VelocityCircular?.X,
                    VelocityCircularDecl = insert.VelocityCircular?.Y,
                    VelocityCircularDistance = insert.VelocityCircular?.Z,
                    VelocityCartesianX = insert.VelocityCartesian?.X,
                    VelocityCartesianY = insert.VelocityCartesian?.Y,
                    VelocityCartesianZ = insert.VelocityCartesian?.Z,
                }
            );

            if (insert.CatalogueEntries != null)
            {
                foreach (var entry in insert.CatalogueEntries)
                {
                    await catalogueEntryRepo.Insert(entry.CatalogueId, star.StarId, new CatalogueEntryInsert
                    {
                        EntryId = entry.EntryId,
                        EntryDesignation = entry.EntryDesignation,
                    });
                }
            }

            tx.Commit();
            return star;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<Star?> Update(int starId, StarPatch patch)
    {
        var conn = db.Connection;

        var changes = patch.ToSql();

        if (string.IsNullOrEmpty(changes))
            return await FetchOne(starId);

        changes += ", updated_at = @updatedAt";

        var star = await conn.QueryFirstOrDefaultAsync<Star>(
            "UPDATE stars SET " +
            changes + " WHERE star_id = @starId RETURNING *", new
            {
                starId,
                updatedAt = DateTime.Now,
                
                ConstellationId = patch.ConstellationId.OrDefault(),
                ProperName = patch.ProperName.OrDefault(),
                RightAscension = patch.RightAscension.OrDefault(),
                Declination = patch.Declination.OrDefault(),
                Distance = patch.Distance.OrDefault(),
                PositionCartesian = patch.PositionCartesian.OrDefault(),
                Magnitude = patch.Magnitude.OrDefault(),
                ColourIndex = patch.ColourIndex.OrDefault(),
                VelocityCartesian = patch.VelocityCartesian.OrDefault(),
                VelocityCircular = patch.VelocityCircular.OrDefault(),
                SpectralType = patch.SpectralType.OrDefault(),
            }
        );
        return star;
    }

    public async Task<Star?> Delete(int id)
    {
        var conn = db.Connection;
        var star = await conn.QueryFirstOrDefaultAsync<Star>("DELETE FROM stars WHERE star_id = @id RETURNING *", new { id });
        return star;
    }
}
