using System.Net.Http.Headers;
using System.Net.Http.Json;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Models;
using CoreModels = GalaxyWorld.Core.Models;
using StarModels = GalaxyWorld.Core.Models.Star;
using CatalogueModels = GalaxyWorld.Core.Models.Catalogue;
using ConstellationModels = GalaxyWorld.Core.Models.Constellation;
using PlanetModels = GalaxyWorld.Core.Models.Planets;
using EntryModels = GalaxyWorld.Core.Models.CatalogueEntry;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;

namespace GalaxyWorld.Cli.ApiHandler;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private static JsonSerializerOptions JsonOptions { get
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }

    public const string BASE_URL = "http://ec2-13-245-23-146.af-south-1.compute.amazonaws.com/";

    public static string? DefaultAuthToken { get; set; }

    public ApiClient(string? authToken = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BASE_URL)
        };

        authToken ??= DefaultAuthToken;

        if (!string.IsNullOrWhiteSpace(authToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new AppException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions) ?? throw new AppException();
    }

    public async Task<T?> GetDefaultAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new AppException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions) ?? throw new AppException();
    }

    public async Task<T> PostAsync<T, P>(string endpoint, P payload)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            var str = await response.Content.ReadAsStringAsync();
            throw new AppException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions) ?? throw new AppException();
    }

    public async Task<T> PatchAsync<T, P>(string endpoint, P payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, endpoint);
        request.Content = JsonContent.Create(payload);
        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new AppException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions) ?? throw new AppException();
    }

    public async Task<T> DeleteAsync<T>(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new AppException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions) ?? throw new AppException();
    }

    public static string FormatQuery<S, T>(int start = 0, int length = 100, S sort = default!, CoreModels::Filter<T>[]? filters = null)
        // where S : Enum
    {
        var list = new List<string>
        {
            $"start={start}",
            $"length={length}",
            $"sort={sort}"
        };

        foreach (var filter in filters ?? []) {
            list.Add($"filter={filter}");
        }

        var query = string.Join("&", list);

        return query;
    }

    public async Task<List<T>> GetWithQueryAsync<S, T>(string path, int start = 0, int length = 100, S sort = default!, CoreModels::Filter<T>[]? filters = null)
        // where S : Enum
    {
        var query = FormatQuery(start, length, sort, filters);
        Console.WriteLine($"{path}?{query}");
        return await GetAsync<List<T>>($"{path}?{query}");
    }


    // Auth endpoints

    public async Task<CoreModels::GetAuthResponse> GetAuth()
    {
        return await GetAsync<CoreModels::GetAuthResponse>($"/auth");
    }

    public async Task<CoreModels::AuthResponse> PostAuth(CoreModels::AuthRequest request)
    {
        return await PostAsync<CoreModels::AuthResponse, CoreModels::AuthRequest>($"/auth", request);
    }

    // Star endpoints

    public async Task<List<StarModels::Star>> GetStars(int start = 0, int length = 100, StarModels::StarSort sort = default, CoreModels::Filter<StarModels::Star>[]? filters = null) {
        var query = FormatQuery(start, length, sort, filters);
        return await GetAsync<List<StarModels::Star>>($"/stars?{query}");
    }

    public async Task<StarModels::Star> PostStar(StarModels::StarInsert insert) {
        return await PostAsync<StarModels::Star, StarModels::StarInsert>($"/stars", insert);
    }

    public async Task<List<StarModels::StarBulkResponse>> PostStarsBulk(IEnumerable<StarModels::StarInsert> inserts) {
        return await PostAsync<List<StarModels::StarBulkResponse>, IEnumerable<StarModels::StarInsert>>($"/stars/bulk", inserts);
    }

    public async Task<StarModels::Star> GetStar(int starId)
    {
        return await GetAsync<StarModels::Star>($"/stars/{starId}");
    }

    public async Task<PlanetModels::PlanetarySystem?> GetStarPlanets(int starId)
    {
        return await GetDefaultAsync<PlanetModels::PlanetarySystem>($"/stars/{starId}/planets");
    }

    public async Task<StarModels::Star> PatchStar(int starId, StarModels::StarPatch patch) {
        return await PatchAsync<StarModels::Star, StarModels::StarPatch>($"/stars/{starId}", patch);
    }

    public async Task<StarModels::Star> DeleteStar(int starId) {
        return await DeleteAsync<StarModels::Star>($"/stars/{starId}");
    }

    public async Task<List<EntryModels::CatalogueEntry>> GetStarCatalogueEntries(int starId, int start = 0, int length = 100, EntryModels::CatalogueEntrySort sort = default, CoreModels::Filter<EntryModels::CatalogueEntry>[]? filters = null) {
        var query = FormatQuery(start, length, sort, filters);
        return await GetAsync<List<EntryModels::CatalogueEntry>>($"/stars/{starId}/catalogues?{query}");
    }

    public async Task<EntryModels::CatalogueEntry> DeleteStarCatalogueEntry(int starId, int catId) {
        return await DeleteAsync<EntryModels::CatalogueEntry>($"/stars/{starId}/catalogues/{catId}");
    }

    // Catalogue endpoints

    public async Task<List<CatalogueModels::Catalogue>> GetCatalogues(int start = 0, int length = 100, CatalogueModels::CatalogueSort sort = default, CoreModels::Filter<CatalogueModels::Catalogue>[]? filters = null) {
        var query = FormatQuery(start, length, sort, filters);
        return await GetAsync<List<CatalogueModels::Catalogue>>($"/catalogues?{query}");
    }

    public async Task<CatalogueModels::Catalogue> PostCatalogue(CatalogueModels::CatalogueInsert insert) {
        return await PostAsync<CatalogueModels::Catalogue, CatalogueModels::CatalogueInsert>($"/catalogues", insert);
    }

    public async Task<CatalogueModels::Catalogue> GetCatalogue(int catId) {
        return await GetAsync<CatalogueModels::Catalogue>($"/catalogues/{catId}");
    }

    public async Task<CatalogueModels::Catalogue> PatchCatalogue(int catId, CatalogueModels::CataloguePatch patch) {
        return await PatchAsync<CatalogueModels::Catalogue, CatalogueModels::CataloguePatch>($"/catalogues/{catId}", patch);
    }

    public async Task<CatalogueModels::Catalogue> DeleteCatalogue(int catId) {
        return await DeleteAsync<CatalogueModels::Catalogue>($"/catalogues/{catId}");
    }

    // Catalogue entry endpoints

    public async Task<List<EntryModels::CatalogueEntry>> GetCatalogueStarEntries(int catId, int start = 0, int length = 100, EntryModels::CatalogueEntrySort sort = default, CoreModels::Filter<EntryModels::CatalogueEntry>[]? filters = null) {
        var query = FormatQuery(start, length, sort, filters);
        return await GetAsync<List<EntryModels::CatalogueEntry>>($"/catalogues/{catId}/stars?{query}");
    }

    public async Task<EntryModels::CatalogueEntry> PostCatalogueEntry(int catId, int starId, EntryModels::CatalogueEntryInsert insert) {
        return await PostAsync<EntryModels::CatalogueEntry, EntryModels::CatalogueEntryInsert>($"/catalogues/{catId}/stars/{starId}", insert);
    }

    public async Task<EntryModels::CatalogueEntry> GetCatalogueEntry(int catId, int starId) {
        return await GetAsync<EntryModels::CatalogueEntry>($"/catalogues/{catId}/stars/{starId}");
    }

    public async Task<EntryModels::CatalogueEntry> PatchCatalogueEntry(int catId, int starId, EntryModels::CatalogueEntryPatch patch) {
        return await PatchAsync<EntryModels::CatalogueEntry, EntryModels::CatalogueEntryPatch>($"/catalogues/{catId}/stars/{starId}", patch);
    }

    public async Task<EntryModels::CatalogueEntry> DeleteCatalogueEntry(int catId, int starId) {
        return await DeleteAsync<EntryModels::CatalogueEntry>($"/catalogues/{catId}/stars/{starId}");
    }

    // Constellation endpoints

    public async Task<List<ConstellationModels::Constellation>> GetConstellations(int start = 0, int length = 100, ConstellationModels::ConstellationSort sort = default, CoreModels::Filter<ConstellationModels::Constellation>[]? filters = null) {
        var query = FormatQuery(start, length, sort, filters);
        return await GetAsync<List<ConstellationModels::Constellation>>($"/constellations?{query}");
    }

    public async Task<ConstellationModels::Constellation> PostConstellation(ConstellationModels::ConstellationInsert insert) {
        return await PostAsync<ConstellationModels::Constellation, ConstellationModels::ConstellationInsert>($"/constellations", insert);
    }

    public async Task<ConstellationModels::Constellation> GetConstellation(int conId) {
        return await GetAsync<ConstellationModels::Constellation>($"/constellations/{conId}");
    }

    public async Task<List<StarModels::Star>> GetConstellationStars(int conId, int start = 0, int length = 100, StarModels::StarSort sort = default, CoreModels::Filter<StarModels::Star>[]? filters = null) {
        var query = FormatQuery(start, length, sort, filters);
        return await GetAsync<List<StarModels::Star>>($"/constellations/{conId}/stars?{query}");
    }

    public async Task<ConstellationModels::Constellation> PatchConstellation(int conId, ConstellationModels::ConstellationPatch patch) {
        return await PatchAsync<ConstellationModels::Constellation, ConstellationModels::ConstellationPatch>($"/constellations/{conId}", patch);
    }

    public async Task<ConstellationModels::Constellation> DeleteConstellation(int conId) {
        return await DeleteAsync<ConstellationModels::Constellation>($"/constellations/{conId}");
    }
}
