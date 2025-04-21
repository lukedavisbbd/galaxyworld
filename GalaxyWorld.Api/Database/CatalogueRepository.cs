using Dapper;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.Catalogue;

namespace GalaxyWorld.API.Database;

public class CatalogueRepository(DbContext db)
{
    public async Task<IEnumerable<Catalogue>> Fetch(Page page, CatalogueSort sort, Filter<Catalogue>[] filters)
    {
        var parameters = new DynamicParameters(page);
        parameters.AddDynamicParams(filters.ToParams());
        
        var conn = db.Connection;
        var catalogues = await conn.QueryAsync<Catalogue>($"SELECT * FROM catalogues {filters.ToSql(FilterPrepend.Where)} {sort.ToSql()} LIMIT @Length OFFSET @Start", parameters);
        
        return catalogues;
    }

    public async Task<Catalogue?> FetchOne(int catalogueId)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<Catalogue>("SELECT * FROM catalogues WHERE catalogue_id = @catalogueId", new { catalogueId });
        return catalogue;
    }

    public async Task<Catalogue> Insert(CatalogueInsert insert)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstAsync<Catalogue>(
            """
            INSERT INTO catalogues(
                catalogue_name,
                catalogue_slug
            ) VALUES(
                @CatalogueName,
                @CatalogueSlug
            ) RETURNING *
            """,
            insert
        );
        return catalogue;
    }

    public async Task<Catalogue?> Update(int catalogueId, CataloguePatch patch)
    {
        var conn = db.Connection;

        var changes = patch.ToSql();

        if (string.IsNullOrEmpty(changes))
            return await FetchOne(catalogueId);

        var catalogue = await conn.QueryFirstOrDefaultAsync<Catalogue>(
            "UPDATE catalogues SET " +
            changes + " WHERE catalogue_id = @catalogueId RETURNING *", new
            {
                catalogueId,
                CatalogueName = patch.CatalogueName.OrDefault(),
                CatalogueSlug = patch.CatalogueSlug.OrDefault(),
            }
        );
        return catalogue;
    }

    public async Task<Catalogue?> Delete(int catalogueId)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<Catalogue>("DELETE FROM catalogues WHERE catalogue_id = @catalogueId RETURNING *", new { catalogueId });
        return catalogue;
    }
}
