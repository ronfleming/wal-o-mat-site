using System.Net.Http.Json;
using System.Text.Json;

namespace WalOMat.Client.Services;

/// <summary>
/// Service for saving and retrieving shared quiz results via Azure Functions API.
/// </summary>
public class ShareService
{
    private readonly HttpClient _http;
    private readonly string _apiBaseUrl;

    public ShareService(HttpClient http)
    {
        _http = http;
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
            Console.Error.WriteLine($"Error saving result: {ex.Message}");
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
            Console.Error.WriteLine($"Error retrieving result: {ex.Message}");
            return null;
        }
    }
}

// DTOs matching API models
public class SaveResultRequest
{
    public string Language { get; set; } = "de";
    public List<AnswerDto> Answers { get; set; } = new();
    public List<string> SelectedWhales { get; set; } = new();
    public List<ResultDto> Results { get; set; } = new();
}

public class AnswerDto
{
    public string QuestionId { get; set; } = string.Empty;
    public int? Position { get; set; }
    public bool IsWeighted { get; set; }
}

public class ResultDto
{
    public string WhaleId { get; set; } = string.Empty;
    public double Percentage { get; set; }
    
    // Ensure JSON deserialization works
    public ResultDto() { }
}

public class SaveResultResponse
{
    public string ResultId { get; set; } = string.Empty;
    public string ShareUrl { get; set; } = string.Empty;
}

public class GetResultResponse
{
    public string? ResultId { get; set; }
    public string? Language { get; set; }
    public DateTime? Timestamp { get; set; }
    public List<ResultDto>? Results { get; set; }
    public bool IsExpired { get; set; }
}

