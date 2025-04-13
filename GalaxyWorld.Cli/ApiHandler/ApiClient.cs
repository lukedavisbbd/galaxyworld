
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Models;
using CoreModels = GalaxyWorld.Core.Models;
using StarModels = GalaxyWorld.Core.Models.Star;
using CatalogueModels = GalaxyWorld.Core.Models.Catalogue;
using ConstellationModels = GalaxyWorld.Core.Models.Constellation;
using EntryModels = GalaxyWorld.Core.Models.CatalogueEntry;

namespace GalaxyWorld.Cli.ApiHandler;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private string baseUrl = ApiStore.BaseUrl;
    private string token = ApiStore.AccessToken;

    public ApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<T> Get<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new CliException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>() ?? throw new CliException();
    }

    private async Task<T> Post<T, P>(string endpoint, P payload)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new CliException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>() ?? throw new CliException();
    }

    private async Task<T> Patch<T, P>(string endpoint, P payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, endpoint);
        request.Content = JsonContent.Create(payload);
        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new CliException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>() ?? throw new CliException();
    }

    private async Task<T> Delete<T>(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new CliException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>() ?? throw new CliException();
    }

    private static string FormatQuery<S, T>(int start = 0, int length = 100, S sort = default!, CoreModels.Filter<T>[]? filters = null)
        where S : System.Enum
    {
        var list = new List<string>();
        list.Add($"start={start}");
        list.Add($"length={length}");
        list.Add($"sort={sort}");

        foreach (var filter in filters ?? []) {
            list.Add($"filter={filter}");
        }

        var query = string.Join("&", list);

        return query;
    }

    // Star endpoints

    public async Task<List<StarModels.Star>> GetStars(int start = 0, int length = 100, StarModels.StarSort sort = default, CoreModels.Filter<StarModels.Star>[]? filters = null) {
        var query = FormatQuery<StarModels.StarSort, StarModels.Star>(start, length, sort, filters);
        return await Get<List<StarModels.Star>>($"/stars?{query}");
    }

    public async Task<StarModels.Star> PostStar(StarModels.StarInsert insert) {
        return await Post<StarModels.Star, StarModels.StarInsert>($"/stars", insert);
    }

    public async Task<List<StarModels.StarBulkResponse>> PostStarsBulk(IEnumerable<StarModels.StarInsert> inserts) {
        return await Post<List<StarModels.StarBulkResponse>, IEnumerable<StarModels.StarInsert>>($"/stars/bulk", inserts);
    }

    public async Task<StarModels.Star> GetStar(int starId) {
        return await Get<StarModels.Star>($"/stars/{starId}");
    }

    public async Task<StarModels.Star> PatchStar(int starId, StarModels.StarPatch patch) {
        return await Patch<StarModels.Star, StarModels.StarPatch>($"/stars/{starId}", patch);
    }

    public async Task<StarModels.Star> DeleteStar(int starId) {
        return await Delete<StarModels.Star>($"/stars/{starId}");
    }

    public async Task<List<EntryModels.CatalogueEntry>> GetStarCatalogueEntries(int starId, int start = 0, int length = 100, EntryModels.CatalogueEntrySort sort = default, CoreModels.Filter<EntryModels.CatalogueEntry>[]? filters = null) {
        var query = FormatQuery<EntryModels.CatalogueEntrySort, EntryModels.CatalogueEntry>(start, length, sort, filters);
        return await Get<List<EntryModels.CatalogueEntry>>($"/stars/{starId}/catalogues?{query}");
    }

    public async Task<EntryModels.CatalogueEntry> PostStarCatalogueEntry(int starId, EntryModels.CatalogueEntryInsert insert) {
        return await Post<EntryModels.CatalogueEntry, EntryModels.CatalogueEntryInsert>($"/stars/{starId}/catalogues", insert);
    }

    public async Task<EntryModels.CatalogueEntry> GetStarCatalogueEntry(int starId, int catId) {
        return await Get<EntryModels.CatalogueEntry>($"/stars/{starId}/catalogues/{catId}");
    }

    public async Task<EntryModels.CatalogueEntry> PatchStarCatalogueEntry(int starId, int catId, EntryModels.CatalogueEntryPatch patch) {
        return await Patch<EntryModels.CatalogueEntry, EntryModels.CatalogueEntryPatch>($"/stars/{starId}/catalogues/{catId}", patch);
    }

    public async Task<EntryModels.CatalogueEntry> DeleteStarCatalogueEntry(int starId, int catId) {
        return await Delete<EntryModels.CatalogueEntry>($"/stars/{starId}/catalogues/{catId}");
    }

    // Catalogue endpoints

    public async Task<List<CatalogueModels.Catalogue>> GetCatalogues(int start = 0, int length = 100, CatalogueModels.CatalogueSort sort = default, CoreModels.Filter<CatalogueModels.Catalogue>[]? filters = null) {
        var query = FormatQuery<CatalogueModels.CatalogueSort, CatalogueModels.Catalogue>(start, length, sort, filters);
        return await Get<List<CatalogueModels.Catalogue>>($"/catalogues?{query}");
    }

    public async Task<CatalogueModels.Catalogue> PostCatalogue(CatalogueModels.CatalogueInsert insert) {
        return await Post<CatalogueModels.Catalogue, CatalogueModels.CatalogueInsert>($"/catalogues", insert);
    }

    public async Task<CatalogueModels.Catalogue> GetCatalogue(int catId) {
        return await Get<CatalogueModels.Catalogue>($"/catalogues/{catId}");
    }

    public async Task<CatalogueModels.Catalogue> PatchCatalogue(int catId, CatalogueModels.CataloguePatch patch) {
        return await Patch<CatalogueModels.Catalogue, CatalogueModels.CataloguePatch>($"/catalogues/{catId}", patch);
    }

    public async Task<CatalogueModels.Catalogue> DeleteCatalogue(int catId) {
        return await Delete<CatalogueModels.Catalogue>($"/catalogues/{catId}");
    }

    public async Task<List<EntryModels.CatalogueEntry>> GetCatalogueStarEntries(int catId, int start = 0, int length = 100, EntryModels.CatalogueEntrySort sort = default, CoreModels.Filter<EntryModels.CatalogueEntry>[]? filters = null) {
        var query = FormatQuery<EntryModels.CatalogueEntrySort, EntryModels.CatalogueEntry>(start, length, sort, filters);
        return await Get<List<EntryModels.CatalogueEntry>>($"/catalogues/{catId}/stars?{query}");
    }

    public async Task<EntryModels.CatalogueEntry> PostCatalogueStarEntry(int catId, EntryModels.CatalogueEntryInsert insert) {
        return await Post<EntryModels.CatalogueEntry, EntryModels.CatalogueEntryInsert>($"/catalogues/{catId}/stars", insert);
    }

    public async Task<EntryModels.CatalogueEntry> GetCatalogueStarEntry(int catId, int starId) {
        return await Get<EntryModels.CatalogueEntry>($"/catalogues/{catId}/stars/{starId}");
    }

    public async Task<EntryModels.CatalogueEntry> PatchCatalogueStarEntry(int catId, int starId, EntryModels.CatalogueEntryPatch patch) {
        return await Patch<EntryModels.CatalogueEntry, EntryModels.CatalogueEntryPatch>($"/catalogues/{catId}/stars/{starId}", patch);
    }

    public async Task<EntryModels.CatalogueEntry> DeleteCatalogueStarEntry(int catId, int starId) {
        return await Delete<EntryModels.CatalogueEntry>($"/catalogues/{catId}/stars/{starId}");
    }

    // Constellation endpoints

    public async Task<List<ConstellationModels.Constellation>> GetConstellations(int start = 0, int length = 100, ConstellationModels.ConstellationSort sort = default, CoreModels.Filter<ConstellationModels.Constellation>[]? filters = null) {
        var query = FormatQuery<ConstellationModels.ConstellationSort, ConstellationModels.Constellation>(start, length, sort, filters);
        return await Get<List<ConstellationModels.Constellation>>($"/constellations?{query}");
    }

    public async Task<ConstellationModels.Constellation> PostConstellation(ConstellationModels.ConstellationInsert insert) {
        return await Post<ConstellationModels.Constellation, ConstellationModels.ConstellationInsert>($"/constellations", insert);
    }

    public async Task<ConstellationModels.Constellation> GetConstellation(int conId) {
        return await Get<ConstellationModels.Constellation>($"/constellations/{conId}");
    }

    public async Task<List<StarModels.Star>> GetConstellationStars(int conId, int start = 0, int length = 100, StarModels.StarSort sort = default, CoreModels.Filter<StarModels.Star>[]? filters = null) {
        var query = FormatQuery<StarModels.StarSort, StarModels.Star>(start, length, sort, filters);
        return await Get<List<StarModels.Star>>($"/constellations/{conId}/stars?{query}");
    }

    public async Task<ConstellationModels.Constellation> PatchConstellation(int conId, ConstellationModels.ConstellationPatch patch) {
        return await Patch<ConstellationModels.Constellation, ConstellationModels.ConstellationPatch>($"/constellations/{conId}", patch);
    }

    public async Task<ConstellationModels.Constellation> DeleteConstellation(int conId) {
        return await Delete<ConstellationModels.Constellation>($"/constellations/{conId}");
    }
}
