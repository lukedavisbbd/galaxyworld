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
        var stars = await conn.QueryAsync<Star>($"SELECT * FROM stars WHERE constellation = @constellation {filters.ToSql(FilterPrepend.And)} {sort.ToSql()} LIMIT @Length OFFSET @Start", parameters);
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
            var star = await conn.QueryFirstAsync<Star>(
                """
                INSERT INTO stars(
                    constellation,
                    proper_name,
                    right_ascension,
                    declination,
                    pos_src,
                    distance,
                    x0,
                    y0,
                    z0,
                    distance_src,
                    magnitude,
                    absolute_magnitude,
                    colour_index,
                    magnitude_src,
                    radial_velocity,
                    radial_velocity_src,
                    proper_motion_right_ascension,
                    proper_motion_declination,
                    proper_motion_src,
                    velocity_x,
                    velocity_y,
                    velocity_z,
                    spectral_type,
                    spectral_type_src
                ) VALUES(
                    @Constellation,
                    @ProperName,
                    @RightAscension,
                    @Declination,
                    @PosSrc,
                    @Distance,
                    @X0,
                    @Y0,
                    @Z0,
                    @DistanceSrc,
                    @Magnitude,
                    @AbsoluteMagnitude,
                    @ColourIndex,
                    @MagnitudeSrc,
                    @RadialVelocity,
                    @RadialVelocitySrc,
                    @ProperMotionRightAscension,
                    @ProperMotionDeclination,
                    @ProperMotionSrc,
                    @VelocityX,
                    @VelocityY,
                    @VelocityZ,
                    @SpectralType,
                    @SpectralTypeSrc
                ) RETURNING *
                """,
                insert
            );

            if (insert.CatalogueEntries != null)
            {
                foreach (var entry in insert.CatalogueEntries)
                {
                    await catalogueEntryRepo.Insert(entry.CatId, star.StarId, new CatalogueEntryInsert
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
                
                Constellation = patch.Constellation.OrDefault(),
                ProperName = patch.ProperName.OrDefault(),
                RightAscension = patch.RightAscension.OrDefault(),
                Declination = patch.Declination.OrDefault(),
                PosSrc = patch.PosSrc.OrDefault(),
                Distance = patch.Distance.OrDefault(),
                X0 = patch.X0.OrDefault(),
                Y0 = patch.Y0.OrDefault(),
                Z0 = patch.Z0.OrDefault(),
                DistanceSrc = patch.DistanceSrc.OrDefault(),
                Magnitude = patch.Magnitude.OrDefault(),
                AbsoluteMagnitude = patch.AbsoluteMagnitude.OrDefault(),
                ColourIndex = patch.ColourIndex.OrDefault(),
                MagnitudeSrc = patch.MagnitudeSrc.OrDefault(),
                RadialVelocity = patch.RadialVelocity.OrDefault(),
                RadialVelocitySrc = patch.RadialVelocitySrc.OrDefault(),
                ProperMotionRightAscension = patch.ProperMotionRightAscension.OrDefault(),
                ProperMotionDeclination = patch.ProperMotionDeclination.OrDefault(),
                ProperMotionSrc = patch.ProperMotionSrc.OrDefault(),
                VelocityX = patch.VelocityX.OrDefault(),
                VelocityY = patch.VelocityY.OrDefault(),
                VelocityZ = patch.VelocityZ.OrDefault(),
                SpectralType = patch.SpectralType.OrDefault(),
                SpectralTypeSrc = patch.SpectralTypeSrc.OrDefault(),
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
