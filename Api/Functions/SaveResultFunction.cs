using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using WalOMat.Api.Entities;
using WalOMat.Shared.Models;

namespace WalOMat.Api.Functions;

public class SaveResultFunction
{
    private readonly ILogger<SaveResultFunction> _logger;
    private readonly TableClient _tableClient;

    public SaveResultFunction(ILogger<SaveResultFunction> logger, TableClient tableClient)
    {
        _logger = logger;
        _tableClient = tableClient;
    }

    [Function("SaveResult")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "results")] HttpRequestData req)
    {
        _logger.LogInformation("SaveResult function triggered");

        try
        {
            // Reject oversized payloads (50 KB limit)
            if (req.Headers.TryGetValues("Content-Length", out var lengthValues)
                && long.TryParse(lengthValues.FirstOrDefault(), out var contentLength)
                && contentLength > 51_200)
            {
                return await CreateErrorResponse(req, HttpStatusCode.RequestEntityTooLarge, "Request payload too large");
            }

            // Parse request body (case-insensitive to match camelCase from client)
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var request = await JsonSerializer.DeserializeAsync<SaveResultRequest>(req.Body, options);
            if (request == null)
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid request body");
            }

            // Validate language
            if (request.Language != "de" && request.Language != "en")
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid language");
            }

            // Validate answer count (quiz has 14 questions)
            if (request.Answers == null || request.Answers.Count == 0 || request.Answers.Count > 20)
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid number of answers");
            }

            // Validate individual answers
            foreach (var answer in request.Answers)
            {
                if (string.IsNullOrWhiteSpace(answer.QuestionId) || answer.QuestionId.Length > 10)
                    return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid answer data");
                if (answer.Position.HasValue && (answer.Position.Value < -1 || answer.Position.Value > 1))
                    return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid answer position");
            }

            // Validate results
            if (request.Results == null || request.Results.Count == 0 || request.Results.Count > 20)
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid number of results");
            }

            foreach (var result in request.Results)
            {
                if (string.IsNullOrWhiteSpace(result.WhaleId) || result.WhaleId.Length > 20)
                    return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid result data");
                if (result.Percentage < 0 || result.Percentage > 100)
                    return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid result percentage");
            }

            // Validate selected whales
            if (request.SelectedWhales == null || request.SelectedWhales.Count == 0 || request.SelectedWhales.Count > 20)
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid whale selection");
            }

            foreach (var whaleId in request.SelectedWhales)
            {
                if (string.IsNullOrWhiteSpace(whaleId) || whaleId.Length > 20)
                    return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid whale ID");
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
            await _tableClient.AddEntityAsync(entity);

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

