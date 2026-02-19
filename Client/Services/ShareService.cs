using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WalOMat.Shared.Models;

namespace WalOMat.Client.Services;

/// <summary>
/// Service for saving and retrieving shared quiz results via Azure Functions API.
/// </summary>
public class ShareService
{
    private readonly HttpClient _http;
    private readonly ILogger<ShareService> _logger;
    private readonly string _apiBaseUrl;

    public ShareService(HttpClient http, ILogger<ShareService> logger)
    {
        _http = http;
        _logger = logger;
        // Use environment-specific API URL
        // Local: http://localhost:7071/api
        // Production: /api (SWA serves it automatically)
        var isLocal = http.BaseAddress?.Host == "localhost";
        _apiBaseUrl = isLocal ? "http://localhost:7071/api" : "/api";
    }

    public async Task<SaveResultResponse?> SaveResultAsync(SaveResultRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{_apiBaseUrl}/results", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SaveResultResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving result");
            return null;
        }
    }

    public async Task<GetResultResponse?> GetResultAsync(string id)
    {
        try
        {
            var response = await _http.GetAsync($"{_apiBaseUrl}/results/{id}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound ||
                response.StatusCode == System.Net.HttpStatusCode.Gone)
            {
                return new GetResultResponse { IsExpired = true };
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GetResultResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving result {Id}", id);
            return null;
        }
    }
}

