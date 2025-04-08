namespace GalaxyWorld.Models.Catalogue;

public class CataloguePatch
{
    public Optional<string> CatName { get; init; } = default;
    public Optional<string> CatSlug { get; init; } = default;

    public string ToSql()
    {
        var changes =
            CatName.Map(_ => "cat_name = @CatName,").Or() +
            CatSlug.Map(_ => "cat_slug = @CatSlug,").Or();
        return changes.Substring(0, int.Max(changes.Length - 1, 0));
    }
}
