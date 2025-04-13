namespace GalaxyWorld.Core.Models.Star;

public class StarBulkResponse
{
    public required Status Status { get; init; }
    public Star? Star { get; init; }
    public string? Error { get; init; }
}
