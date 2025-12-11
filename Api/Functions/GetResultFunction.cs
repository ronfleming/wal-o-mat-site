using System.Net;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using WalOMat.Api.Entities;
using WalOMat.Api.Models;

namespace WalOMat.Api.Functions;

public class GetResultFunction
{
    private readonly ILogger<GetResultFunction> _logger;

    public GetResultFunction(ILogger<GetResultFunction> logger)
    {
        _logger = logger;
    }

    [Function("GetResult")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "results/{id}")] HttpRequestData req,
        string id)
    {
        _logger.LogInformation("GetResult function triggered for ID: {Id}", id);

        try
        {
            // Validate GUID format
            if (!Guid.TryParse(id, out _))
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid result ID format");
            }

            // Retrieve from Table Storage
            var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString")
                ?? throw new InvalidOperationException("TableStorageConnectionString not configured");
            
            var tableClient = new TableClient(connectionString, "QuizResults");
            
            QuizResultEntity? entity;
            try
            {
                entity = await tableClient.GetEntityAsync<QuizResultEntity>("results", id);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                _logger.LogWarning("Result {Id} not found", id);
                return await CreateNotFoundResponse(req);
            }

            // Check expiration
            if (entity.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogInformation("Result {Id} has expired", id);
                return await CreateExpiredResponse(req);
            }

            // Deserialize and return
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = JsonSerializer.Deserialize<List<ResultDto>>(entity.ResultsJson, options);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new GetResultResponse
            {
                ResultId = id,
                Language = entity.Language,
                Timestamp = entity.CreatedAt,
                Results = results,
                IsExpired = false
            });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving result {Id}", id);
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "Failed to retrieve result");
        }
    }

    private static async Task<HttpResponseData> CreateErrorResponse(
        HttpRequestData req, HttpStatusCode statusCode, string message)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }

    private static async Task<HttpResponseData> CreateNotFoundResponse(HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.NotFound);
        await response.WriteAsJsonAsync(new GetResultResponse { IsExpired = true });
        return response;
    }

    private static async Task<HttpResponseData> CreateExpiredResponse(HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.Gone);
        await response.WriteAsJsonAsync(new GetResultResponse { IsExpired = true });
        return response;
    }
}

