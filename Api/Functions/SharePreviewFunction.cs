using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using WalOMat.Api.Entities;
using WalOMat.Api.Models;
using WalOMat.Shared.Models;

namespace WalOMat.Api.Functions;

public class SharePreviewFunction
{
    private readonly ILogger<SharePreviewFunction> _logger;

    public SharePreviewFunction(ILogger<SharePreviewFunction> logger)
    {
        _logger = logger;
    }

    [Function("SharePreview")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/{id}")] 
        HttpRequestData req,
        string id)
    {
        _logger.LogInformation("Share preview request for ID: {Id}", id);

        // Check if this is a bot/crawler
        var userAgent = req.Headers.TryGetValues("User-Agent", out var values) 
            ? values.FirstOrDefault() ?? "" 
            : "";
        
        var isCrawler = IsBot(userAgent);

        // If human user, redirect to the actual Blazor page
        if (!isCrawler)
        {
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5042";
            var response = req.CreateResponse(HttpStatusCode.Redirect);
            response.Headers.Add("Location", $"{baseUrl}/result/{id}");
            return response;
        }

        // Bot detected - fetch result and return static HTML with Open Graph tags
        try
        {
            var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString")
                ?? throw new InvalidOperationException("TableStorageConnectionString not configured");
            var tableClient = new TableClient(connectionString, "QuizResults");
            var entity = await tableClient.GetEntityAsync<QuizResultEntity>("results", id);
            
            if (entity?.Value == null || entity.Value.ExpiresAt < DateTime.UtcNow)
            {
                return await CreateNotFoundResponse(req);
            }

            var html = GeneratePreviewHtml(entity.Value);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/html; charset=utf-8");
            await response.WriteStringAsync(html);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching result {Id}", id);
            return await CreateNotFoundResponse(req);
        }
    }

    private bool IsBot(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return false;
        
        var botPatterns = new[]
        {
            "facebookexternalhit",
            "Facebot",
            "WhatsApp",
            "Twitterbot",
            "LinkedInBot",
            "Slackbot",
            "TelegramBot",
            "discordbot",
            "SkypeUriPreview",
            "vkShare",
            "Pinterest",
            "tumblr",
            "redditbot"
        };

        return botPatterns.Any(pattern => 
            userAgent.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    private string GeneratePreviewHtml(QuizResultEntity entity)
    {
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5042";
        var resultId = entity.RowKey;
        var language = entity.Language ?? "de";
        
        // Parse top result
        var results = JsonSerializer.Deserialize<List<ResultDto>>(entity.ResultsJson);
        var topResult = results?.OrderByDescending(r => r.Percentage).FirstOrDefault();
        
        if (topResult == null)
        {
            return GenerateFallbackHtml(baseUrl, resultId);
        }

        // Use shared whale metadata (single source of truth with whales.json)
        var whaleInfo = WhaleMetadata.Get(topResult.WhaleId);
        var whaleName = language == "en" ? whaleInfo.NameEn : whaleInfo.NameDe;
        var imageUrl = $"{baseUrl}/{whaleInfo.ImagePath}";
        var pageUrl = $"{baseUrl}/result/{resultId}";
        
        // Generate localized OG text
        var title = language == "en"
            ? $"My Wal-O-Mat Result: {whaleName}"
            : $"Mein Wal-O-Mat Ergebnis: {whaleName}";
        var description = language == "en"
            ? $"I'm {topResult.Percentage:F0}% {whaleName}! Find out which whale you are."
            : $"Ich bin {topResult.Percentage:F0}% {whaleName}! Finde heraus, welcher Wal du bist.";
        var locale = language == "en" ? "en_US" : "de_DE";

        return $@"<!DOCTYPE html>
<html lang=""{language}"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
    
    <!-- Open Graph / Facebook -->
    <meta property=""og:type"" content=""website"">
    <meta property=""og:url"" content=""{pageUrl}"">
    <meta property=""og:title"" content=""{title}"">
    <meta property=""og:description"" content=""{description}"">
    <meta property=""og:image"" content=""{imageUrl}"">
    <meta property=""og:image:width"" content=""1200"">
    <meta property=""og:image:height"" content=""630"">
    <meta property=""og:locale"" content=""{locale}"">
    
    <!-- Twitter -->
    <meta name=""twitter:card"" content=""summary_large_image"">
    <meta name=""twitter:url"" content=""{pageUrl}"">
    <meta name=""twitter:title"" content=""{title}"">
    <meta name=""twitter:description"" content=""{description}"">
    <meta name=""twitter:image"" content=""{imageUrl}"">
    
    <!-- Redirect humans to actual app -->
    <meta http-equiv=""refresh"" content=""0; url={pageUrl}"">
</head>
<body>
    <p>Redirecting to <a href=""{pageUrl}"">your Wal-O-Mat result</a>...</p>
</body>
</html>";
    }

    private string GenerateFallbackHtml(string baseUrl, string resultId)
    {
        var pageUrl = $"{baseUrl}/result/{resultId}";
        var title = "Wal-O-Mat – Welcher Wal bist du?";
        var description = "Finde heraus, welcher Wal am besten zu deiner Persönlichkeit passt!";
        var imageUrl = $"{baseUrl}/images/whales/orca1.webp";

        return $@"<!DOCTYPE html>
<html lang=""de"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
    
    <meta property=""og:type"" content=""website"">
    <meta property=""og:url"" content=""{pageUrl}"">
    <meta property=""og:title"" content=""{title}"">
    <meta property=""og:description"" content=""{description}"">
    <meta property=""og:image"" content=""{imageUrl}"">
    
    <meta name=""twitter:card"" content=""summary_large_image"">
    <meta name=""twitter:title"" content=""{title}"">
    <meta name=""twitter:description"" content=""{description}"">
    <meta name=""twitter:image"" content=""{imageUrl}"">
    
    <meta http-equiv=""refresh"" content=""0; url={pageUrl}"">
</head>
<body>
    <p>Redirecting to <a href=""{pageUrl}"">Wal-O-Mat</a>...</p>
</body>
</html>";
    }

    private async Task<HttpResponseData> CreateNotFoundResponse(HttpRequestData req)
    {
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5042";
        var html = $@"<!DOCTYPE html>
<html lang=""de"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Ergebnis nicht gefunden – Wal-O-Mat</title>
    <meta http-equiv=""refresh"" content=""3; url={baseUrl}"">
</head>
<body>
    <h1>Ergebnis nicht gefunden oder abgelaufen</h1>
    <p>Redirecting to <a href=""{baseUrl}"">Wal-O-Mat</a>...</p>
</body>
</html>";

        var response = req.CreateResponse(HttpStatusCode.NotFound);
        response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        await response.WriteStringAsync(html);
        return response;
    }

}

