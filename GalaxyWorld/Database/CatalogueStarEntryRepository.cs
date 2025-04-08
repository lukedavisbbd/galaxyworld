using Dapper;
using GalaxyWorld.Models;
using GalaxyWorld.Models.CatalogueEntry;

namespace GalaxyWorld.Database;

public class CatalogueStarEntryRepository(DbContext db)
{
    public async Task<IEnumerable<CatalogueStarEntry>> FetchByStar(int starId, Page page, CatalogueStarEntrySort sort)
    {
        var conn = db.Connection;
        var starCatalogues = await conn.QueryAsync<CatalogueStarEntry>($"SELECT * FROM catalogue_star_entries WHERE star_id = @starId {sort.ToSql()} LIMIT @Length OFFSET @Start", new
        {
            starId,
            page.Start,
            page.Length,
        });
        return starCatalogues;
    }

    public async Task<IEnumerable<CatalogueStarEntry>> FetchByCatalogue(int catId, Page page, CatalogueStarEntrySort sort)
    {
        var conn = db.Connection;
        var starCatalogues = await conn.QueryAsync<CatalogueStarEntry>($"SELECT * FROM catalogue_star_entries WHERE cat_id = @catId {sort.ToSql()} LIMIT @Length OFFSET @Start", new
        {
            catId,
            page.Start,
            page.Length,
        });
        return starCatalogues;
    }

    public async Task<CatalogueStarEntry?> FetchOneByEntryId(int catId, string entryId)
    {
        var conn = db.Connection;
        var starCatalogue = await conn.QueryFirstOrDefaultAsync<CatalogueStarEntry>($"SELECT * FROM catalogue_star_entries WHERE cat_id = @catId AND entry_id = @entryId", new
        {
            catId,
            entryId,
        });
        return starCatalogue;
    }

    public async Task<CatalogueStarEntry> Insert(CatalogueStarEntry insert)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstAsync<CatalogueStarEntry>(
            """
            INSERT INTO catalogue_star_entries (
                cat_id,
                star_id,
                entry_id
            ) VALUES(
                @CatId,
                @StarId,
                @EntryId
            ) RETURNING *
            """,
            insert
        );
        return catalogue;
    }

    public async Task<CatalogueStarEntry?> Update(CatalogueStarEntry patch)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<CatalogueStarEntry>(
            "UPDATE catalogue_star_entries SET entry_id = @EntryId WHERE cat_id = @CatId AND star_id = @StarId RETURNING *",
            patch
        );
        return catalogue;
    }

    public async Task<CatalogueStarEntry?> DeleteByCatelogue(int catId, string entryId)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<CatalogueStarEntry>("DELETE FROM catalogue_star_entries WHERE cat_id = @catId AND entry_id = @entryId RETURNING *", new
        {
            catId,
            entryId,
        });
        return catalogue;
    }

    public async Task<CatalogueStarEntry?> DeleteByStar(int starId, int catId)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<CatalogueStarEntry>("DELETE FROM catalogue_star_entries WHERE star_id = @starId AND cat_id = @catId RETURNING *", new
        {
            starId,
            catId,
        });
        return catalogue;
    }
}
