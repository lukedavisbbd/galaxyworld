using Dapper;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.API.Database;

public class CatalogueEntryRepository(DbContext db)
{
    public async Task<IEnumerable<CatalogueEntry>> FetchByStar(int starId, Page page, CatalogueEntrySort sort, Filter<CatalogueEntry>[] filters)
    {
        var parameters = new DynamicParameters(page);
        parameters.Add("starId", starId);
        parameters.AddDynamicParams(filters.ToParams());

        var conn = db.Connection;
        var starCatalogues = await conn.QueryAsync<CatalogueEntry>($"SELECT * FROM catalogue_entries WHERE star_id = @starId {filters.ToSql(FilterPrepend.And)} {sort.ToSql()} LIMIT @Length OFFSET @Start", parameters);
        return starCatalogues;
    }

    public async Task<IEnumerable<CatalogueEntry>> FetchByCatalogue(int catalogueId, Page page, CatalogueEntrySort sort, Filter<CatalogueEntry>[] filters)
    {
        var parameters = new DynamicParameters(page);
        parameters.Add("catalogueId", catalogueId);
        parameters.AddDynamicParams(filters.ToParams());

        var conn = db.Connection;
        var starCatalogues = await conn.QueryAsync<CatalogueEntry>($"SELECT * FROM catalogue_entries WHERE catalogue_id = @catalogueId {filters.ToSql(FilterPrepend.And)} {sort.ToSql()} LIMIT @Length OFFSET @Start", parameters);
        return starCatalogues;
    }

    public async Task<CatalogueEntry?> FetchOne(int catalogueId, int starId)
    {
        var conn = db.Connection;
        var starCatalogue = await conn.QueryFirstOrDefaultAsync<CatalogueEntry>($"SELECT * FROM catalogue_entries WHERE catalogue_id = @catalogueId AND star_id = @starId", new
        {
            catalogueId,
            starId,
        });
        return starCatalogue;
    }

    public async Task<CatalogueEntry> Insert(int catalogueId, int starId, CatalogueEntryInsert insert)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstAsync<CatalogueEntry>(
            """
            INSERT INTO catalogue_entries (
                catalogue_id,
                star_id,
                entry_id,
                entry_designation
            ) VALUES(
                @catalogueId,
                @starId,
                @EntryId,
                @EntryDesignation
            ) RETURNING *
            """,
            new
            {
                catalogueId,
                starId,
                insert.EntryId,
                insert.EntryDesignation,
            }
        );
        return catalogue;
    }

    public async Task<CatalogueEntry?> Update(int catalogueId, int starId, CatalogueEntryPatch patch)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<CatalogueEntry>(
            $"UPDATE catalogue_entries SET {patch.ToSql()} WHERE catalogue_id = @catalogueId AND star_id = @starId RETURNING *",
            new
            {
                catalogueId,
                starId,
                EntryId = patch.EntryId.OrDefault(),
                EntryDesignation = patch.EntryDesignation.OrDefault(),
            }
        );
        return catalogue;
    }

    public async Task<CatalogueEntry?> Delete(int catalogueId, int starId)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<CatalogueEntry>("DELETE FROM catalogue_entries WHERE catalogue_id = @catalogueId AND star_id = @starId RETURNING *", new
        {
            starId,
            catalogueId,
        });
        return catalogue;
    }
}
