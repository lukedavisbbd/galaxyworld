using Dapper;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;
using GalaxyWorld.Core.Models.Star;

namespace GalaxyWorld.API.Database;

public class StarRepository(DbContext db, CatalogueStarEntryRepository catalogueEntryRepo)
{
    public async Task<IEnumerable<Star>> Fetch(Page page, StarSort sort)
    {
        var conn = db.Connection;
        var stars = await conn.QueryAsync<Star>($"SELECT * FROM stars {sort.ToSql()} LIMIT @Length OFFSET @Start", page);
        return stars;
    }

    public async Task<IEnumerable<Star>> FetchByConstellation(int constellation, Page page, StarSort sort)
    {
        var conn = db.Connection;
        var stars = await conn.QueryAsync<Star>($"SELECT * FROM stars WHERE constellation = @constellation {sort.ToSql()} LIMIT @Length OFFSET @Start", new
        {
            constellation,
            page.Start,
            page.Length,
        });
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
                    await catalogueEntryRepo.Insert(new CatalogueStarEntry
                    {
                        CatId = entry.CatId,
                        StarId = star.StarId,
                        EntryId = entry.EntryId,
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
                
                Constellation = patch.Constellation.Or(),
                ProperName = patch.ProperName.Or(),
                RightAscension = patch.RightAscension.Or(),
                Declination = patch.Declination.Or(),
                PosSrc = patch.PosSrc.Or(),
                Distance = patch.Distance.Or(),
                X0 = patch.X0.Or(),
                Y0 = patch.Y0.Or(),
                Z0 = patch.Z0.Or(),
                DistanceSrc = patch.DistanceSrc.Or(),
                Magnitude = patch.Magnitude.Or(),
                AbsoluteMagnitude = patch.AbsoluteMagnitude.Or(),
                ColourIndex = patch.ColourIndex.Or(),
                MagnitudeSrc = patch.MagnitudeSrc.Or(),
                RadialVelocity = patch.RadialVelocity.Or(),
                RadialVelocitySrc = patch.RadialVelocitySrc.Or(),
                ProperMotionRightAscension = patch.ProperMotionRightAscension.Or(),
                ProperMotionDeclination = patch.ProperMotionDeclination.Or(),
                ProperMotionSrc = patch.ProperMotionSrc.Or(),
                VelocityX = patch.VelocityX.Or(),
                VelocityY = patch.VelocityY.Or(),
                VelocityZ = patch.VelocityZ.Or(),
                SpectralType = patch.SpectralType.Or(),
                SpectralTypeSrc = patch.SpectralTypeSrc.Or(),
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
