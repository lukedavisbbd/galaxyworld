using GalaxyWorld.Cli.Commands.Base;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Cli.Models;
using GalaxyWorld.Core.Models.Catalogue;

namespace GalaxyWorld.Cli.Commands.Catalogues;

public class GetAllCataloguesCommand : GetEntitiesWithQueryCommand<Catalogue, CatalogueSort>
{
    protected override string Path => Routes.Catalogues;

    protected override void Display(IEnumerable<Catalogue> catalogues, string sortBy)
    {
        TableHelper.Print(
            items: catalogues,
            title: $"Catalogues (Sorted by {FormatHelper.PascalToTitleCase(sortBy)})",
            columns: new()
            {
                { "ID",     c => c.CatId.ToString() },
                { "Name",   c => c.CatName ?? "" },
                { "Slug",   c => c.CatSlug ?? "" }
            });
    }

}
