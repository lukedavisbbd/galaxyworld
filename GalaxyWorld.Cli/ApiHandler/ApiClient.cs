
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Models;

namespace GalaxyWorld.Cli.ApiHandler;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private string baseUrl = ApiStore.BaseUrl;

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

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new CliException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T, P>(string endpoint, P payload)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new CliException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PatchAsync<T, P>(string endpoint, P payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, endpoint);
        request.Content = JsonContent.Create(payload);
        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new CliException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            throw new CliException(error?.Detail ?? error?.Title ?? response.StatusCode.ToString());
        }

        return response.IsSuccessStatusCode;
    }
}
