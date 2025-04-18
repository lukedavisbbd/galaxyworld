﻿using Dapper;
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

    public async Task<IEnumerable<CatalogueEntry>> FetchByCatalogue(int catId, Page page, CatalogueEntrySort sort, Filter<CatalogueEntry>[] filters)
    {
        var parameters = new DynamicParameters(page);
        parameters.Add("catId", catId);
        parameters.AddDynamicParams(filters.ToParams());

        var conn = db.Connection;
        var starCatalogues = await conn.QueryAsync<CatalogueEntry>($"SELECT * FROM catalogue_entries WHERE cat_id = @catId {filters.ToSql(FilterPrepend.And)} {sort.ToSql()} LIMIT @Length OFFSET @Start", parameters);
        return starCatalogues;
    }

    public async Task<CatalogueEntry?> FetchOne(int catId, int starId)
    {
        var conn = db.Connection;
        var starCatalogue = await conn.QueryFirstOrDefaultAsync<CatalogueEntry>($"SELECT * FROM catalogue_entries WHERE cat_id = @catId AND star_id = @starId", new
        {
            catId,
            starId,
        });
        return starCatalogue;
    }

    public async Task<CatalogueEntry> Insert(int catId, int starId, CatalogueEntryInsert insert)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstAsync<CatalogueEntry>(
            """
            INSERT INTO catalogue_entries (
                cat_id,
                star_id,
                entry_id,
                entry_designation
            ) VALUES(
                @catId,
                @starId,
                @EntryId,
                @EntryDesignation
            ) RETURNING *
            """,
            new
            {
                catId,
                starId,
                insert.EntryId,
                insert.EntryDesignation,
            }
        );
        return catalogue;
    }

    public async Task<CatalogueEntry?> Update(int catId, int starId, CatalogueEntryPatch patch)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<CatalogueEntry>(
            $"UPDATE catalogue_entries SET {patch.ToSql()} WHERE cat_id = @catId AND star_id = @starId RETURNING *",
            new
            {
                catId,
                starId,
                EntryId = patch.EntryId.OrDefault(),
                EntryDesignation = patch.EntryDesignation.OrDefault(),
            }
        );
        return catalogue;
    }

    public async Task<CatalogueEntry?> Delete(int catId, int starId)
    {
        var conn = db.Connection;
        var catalogue = await conn.QueryFirstOrDefaultAsync<CatalogueEntry>("DELETE FROM catalogue_entries WHERE cat_id = @catId AND star_id = @starId RETURNING *", new
        {
            starId,
            catId,
        });
        return catalogue;
    }
}
