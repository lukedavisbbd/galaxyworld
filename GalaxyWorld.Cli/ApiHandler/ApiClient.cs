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
    {
        var query = FormatQuery(start, length, sort, filters);
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

    public async Task<List<StarModels::StarBulkResponse>> PostStarsBulk(IEnumerable<StarModels::StarInsert> inserts) {
        return await PostAsync<List<StarModels::StarBulkResponse>, IEnumerable<StarModels::StarInsert>>($"/stars/bulk", inserts);
    }

    // Catalogue endpoints

    public async Task<List<CatalogueModels::Catalogue>> GetCatalogues(int start = 0, int length = 100, CatalogueModels::CatalogueSort sort = default, CoreModels::Filter<CatalogueModels::Catalogue>[]? filters = null) {
        var query = FormatQuery(start, length, sort, filters);
        return await GetAsync<List<CatalogueModels::Catalogue>>($"/catalogues?{query}");
    }

    // Constellation endpoints

    public async Task<List<ConstellationModels::Constellation>> GetConstellations(int start = 0, int length = 100, ConstellationModels::ConstellationSort sort = default, CoreModels::Filter<ConstellationModels::Constellation>[]? filters = null) {
        var query = FormatQuery(start, length, sort, filters);
        return await GetAsync<List<ConstellationModels::Constellation>>($"/constellations?{query}");
    }
}
