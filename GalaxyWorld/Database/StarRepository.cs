using Dapper;
using GalaxyWorld.Models;
using GalaxyWorld.Models.Dbos;

namespace GalaxyWorld.Database;

public class StarRepository(DbContext db)
{
    public async Task<IEnumerable<Star>> FetchStars(Page? page = default)
    {
        page ??= new Page
        {
            Start = 0,
            Length = 100,
        };

        var conn = db.Connection;
        var stars = await conn.QueryAsync<Star>("SELECT * FROM stars LIMIT @length OFFSET @start", new {
            start = page.Start,
            length = page.Length,
        });
        return stars;
    }

    public async Task<Star?> FetchStarById(int id)
    {
        var conn = db.Connection;
        var star = await conn.QueryFirstOrDefaultAsync<Star>("SELECT * FROM stars WHERE star_id = @id", new { id });
        return star;
    }

    public async Task<Star> InsertStar(StarInsert insert)
    {
        var conn = db.Connection;
        var star = await conn.QueryFirstAsync<Star>(
            "INSERT INTO stars (" +
                "athyg_id," +
                "tycho2_id," +
                "gaia_dr3_id," +
                "hyg_v3_id," +
                "hipparcos_id," +
                "henry_draper_id," +
                "harvard_yale_id," +
                "gliese_id," +
                "bayer_id," +
                "flamsteed_id," +
                "constellation," +
                "proper_name," +
                "right_ascension," +
                "declination," +
                "pos_src," +
                "distance," +
                "x0," +
                "y0," +
                "z0," +
                "distance_src," +
                "magnitude," +
                "absolute_magnitude," +
                "colour_index," +
                "magnitude_src," +
                "radial_velocity," +
                "radial_velocity_src," +
                "proper_motion_right_ascension," +
                "proper_motion_declination," +
                "proper_motion_src," +
                "velocity_x," +
                "velocity_y," +
                "velocity_z," +
                "spectral_type," +
                "spectral_type_src" +
            ") VALUES (" +
                "@AthygId," +
                "@Tycho2Id," +
                "@GaiaDr3Id," +
                "@HygV3Id," +
                "@HipparcosId," +
                "@HenryDraperId," +
                "@HarvardYaleId," +
                "@GlieseId," +
                "@BayerId," +
                "@FlamsteedId," +
                "@Constellation," +
                "@ProperName," +
                "@RightAscension," +
                "@Declination," +
                "@PosSrc," +
                "@Distance," +
                "@X0," +
                "@Y0," +
                "@Z0," +
                "@DistanceSrc," +
                "@Magnitude," +
                "@AbsoluteMagnitude," +
                "@ColourIndex," +
                "@MagnitudeSrc," +
                "@RadialVelocity," +
                "@RadialVelocitySrc," +
                "@ProperMotionRightAscension," +
                "@ProperMotionDeclination," +
                "@ProperMotionSrc," +
                "@VelocityX," +
                "@VelocityY," +
                "@VelocityZ," +
                "@SpectralType," +
                "@SpectralTypeSrc" +
            ") RETURNING *", new {
                insert.AthygId,
                insert.Tycho2Id,
                insert.GaiaDr3Id,
                insert.HygV3Id,
                insert.HipparcosId,
                insert.HenryDraperId,
                insert.HarvardYaleId,
                insert.GlieseId,
                insert.BayerId,
                insert.FlamsteedId,
                insert.Constellation,
                insert.ProperName,
                insert.RightAscension,
                insert.Declination,
                insert.PosSrc,
                insert.Distance,
                insert.X0,
                insert.Y0,
                insert.Z0,
                insert.DistanceSrc,
                insert.Magnitude,
                insert.AbsoluteMagnitude,
                insert.ColourIndex,
                insert.MagnitudeSrc,
                insert.RadialVelocity,
                insert.RadialVelocitySrc,
                insert.ProperMotionRightAscension,
                insert.ProperMotionDeclination,
                insert.ProperMotionSrc,
                insert.VelocityX,
                insert.VelocityY,
                insert.VelocityZ,
                insert.SpectralType,
                insert.SpectralTypeSrc,
            }
        );
        return star;
    }

    internal async Task<Star?> UpdateStar(int id, StarPatch patch)
    {
        var conn = db.Connection;

        var changes = patch.ToSql();

        Console.WriteLine("A"+changes+"B");

        if (string.IsNullOrEmpty(changes))
            return await FetchStarById(id);

        changes += ", updated_at = @updatedAt";

        var star = await conn.QueryFirstOrDefaultAsync<Star>(
            "UPDATE stars SET " +
            changes + " WHERE star_id = @id RETURNING *", new
            {
                id,
                updatedAt = DateTime.Now,
                
                AthygId = patch.AthygId.ValueOrDefault,
                Tycho2Id = patch.Tycho2Id.ValueOrDefault,
                GaiaDr3Id = patch.GaiaDr3Id.ValueOrDefault,
                HygV3Id = patch.HygV3Id.ValueOrDefault,
                HipparcosId = patch.HipparcosId.ValueOrDefault,
                HenryDraperId = patch.HenryDraperId.ValueOrDefault,
                HarvardYaleId = patch.HarvardYaleId.ValueOrDefault,
                GlieseId = patch.GlieseId.ValueOrDefault,
                BayerId = patch.BayerId.ValueOrDefault,
                FlamsteedId = patch.FlamsteedId.ValueOrDefault,
                Constellation = patch.Constellation.ValueOrDefault,
                ProperName = patch.ProperName.ValueOrDefault,
                RightAscension = patch.RightAscension.ValueOrDefault,
                Declination = patch.Declination.ValueOrDefault,
                PosSrc = patch.PosSrc.ValueOrDefault,
                Distance = patch.Distance.ValueOrDefault,
                X0 = patch.X0.ValueOrDefault,
                Y0 = patch.Y0.ValueOrDefault,
                Z0 = patch.Z0.ValueOrDefault,
                DistanceSrc = patch.DistanceSrc.ValueOrDefault,
                Magnitude = patch.Magnitude.ValueOrDefault,
                AbsoluteMagnitude = patch.AbsoluteMagnitude.ValueOrDefault,
                ColourIndex = patch.ColourIndex.ValueOrDefault,
                MagnitudeSrc = patch.MagnitudeSrc.ValueOrDefault,
                RadialVelocity = patch.RadialVelocity.ValueOrDefault,
                RadialVelocitySrc = patch.RadialVelocitySrc.ValueOrDefault,
                ProperMotionRightAscension = patch.ProperMotionRightAscension.ValueOrDefault,
                ProperMotionDeclination = patch.ProperMotionDeclination.ValueOrDefault,
                ProperMotionSrc = patch.ProperMotionSrc.ValueOrDefault,
                VelocityX = patch.VelocityX.ValueOrDefault,
                VelocityY = patch.VelocityY.ValueOrDefault,
                VelocityZ = patch.VelocityZ.ValueOrDefault,
                SpectralType = patch.SpectralType.ValueOrDefault,
                SpectralTypeSrc = patch.SpectralTypeSrc.ValueOrDefault,
            }
        );
        return star;
    }
}
