using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class GetCatalogueStarsCommand : GetStarsByParentIdCommand<CatalogueEntry, CatalogueEntrySort>
{
    protected override string SubPath => Routes.Catalogues;

    protected override void Display(IEnumerable<CatalogueEntry> catalogues, string sortBy)
    {
        TableHelper.Print(
            items: catalogues,
            title: $"Catalogues (Sorted by {FormatHelper.PascalToTitleCase(sortBy)})",
            columns: new()
            {
                { "Star ID",         s => s.StarId.ToString() },
                { "Catalogues ID",   s => s.CatId.ToString() },
                { "Entry ID",     s => s.EntryId ?? "" },
                { "Entry Designation",        s => s.EntryDesignation?.ToString() ?? "" }
            });
    }
}
