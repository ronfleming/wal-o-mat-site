using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using WalOMat.Api.Entities;
using WalOMat.Api.Models;

namespace WalOMat.Api.Functions;

public class SaveResultFunction
{
    private readonly ILogger<SaveResultFunction> _logger;

    public SaveResultFunction(ILogger<SaveResultFunction> logger)
    {
        _logger = logger;
    }

    [Function("SaveResult")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "results")] HttpRequestData req)
    {
        _logger.LogInformation("SaveResult function triggered");

        try
        {
            // Parse request body (case-insensitive to match camelCase from client)
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var request = await JsonSerializer.DeserializeAsync<SaveResultRequest>(req.Body, options);
            if (request == null)
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid request body");
            }

            // Generate GUID
            var resultId = Guid.NewGuid().ToString();

            // Create entity
            var entity = new QuizResultEntity
            {
                RowKey = resultId,
                Language = request.Language,
                AnswersJson = JsonSerializer.Serialize(request.Answers),
                SelectedWhalesJson = JsonSerializer.Serialize(request.SelectedWhales),
                ResultsJson = JsonSerializer.Serialize(request.Results),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(90)
            };

            // Save to Table Storage
            var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString")
                ?? throw new InvalidOperationException("TableStorageConnectionString not configured");
            
            var tableClient = new TableClient(connectionString, "QuizResults");
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(entity);

            _logger.LogInformation("Saved result {ResultId}", resultId);

            // Build share URL using preview endpoint (handles both bots and humans)
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5042";
            // For local dev, API is on different port, so point directly to result page
            // In production, SWA routes /api/* to functions automatically
            var isLocal = baseUrl.Contains("localhost");
            var shareUrl = isLocal 
                ? $"{baseUrl}/result/{resultId}"  // Local: skip bot detection, go direct
                : $"{baseUrl}/api/share/{resultId}";  // Prod: use preview function

            // Return response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new SaveResultResponse
            {
                ResultId = resultId,
                ShareUrl = shareUrl
            });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving result");
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "Failed to save result");
        }
    }

    private static async Task<HttpResponseData> CreateErrorResponse(
        HttpRequestData req, HttpStatusCode statusCode, string message)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }
}

