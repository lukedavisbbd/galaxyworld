using Dapper;
using GalaxyWorld.Models;
using GalaxyWorld.Models.Catalogue;

namespace GalaxyWorld.Database;

public class CatalogueRepository(DbContext db)
{
    public async Task<IEnumerable<Catalogue>> Fetch(Page page, CatalogueSort sort)
    {
        var conn = db.Connection;
        var catalogues = await conn.QueryAsync<Catalogue>($"SELECT * FROM catalogues {sort.ToSql()} LIMIT @Length OFFSET @Start", page);
        return catalogues;
    }

    public async Task<Catalogue?> FetchOne(int catId)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<Catalogue>("SELECT * FROM catalogues WHERE cat_id = @catId", new { catId });
        return catalogue;
    }

    public async Task<Catalogue> Insert(CatalogueInsert insert)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstAsync<Catalogue>(
            """
            INSERT INTO catalogues(
                cat_name,
                cat_slug
            ) VALUES(
                @CatName,
                @CatSlug
            ) RETURNING *
            """,
            insert
        );
        return catalogue;
    }

    public async Task<Catalogue?> Update(int catId, CataloguePatch patch)
    {
        var conn = db.Connection;

        var changes = patch.ToSql();

        if (string.IsNullOrEmpty(changes))
            return await FetchOne(catId);

        var catalogue = await conn.QueryFirstOrDefaultAsync<Catalogue>(
            "UPDATE catalogues SET " +
            changes + " WHERE cat_id = @catId RETURNING *", new
            {
                catId,
                CatName = patch.CatName.Or(),
                CatSlug = patch.CatSlug.Or(),
            }
        );
        return catalogue;
    }

    public async Task<Catalogue?> Delete(int catId)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<Catalogue>("DELETE FROM catalogues WHERE cat_id = @catId RETURNING *", new { catId });
        return catalogue;
    }
}
