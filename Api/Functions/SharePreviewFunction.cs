using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using WalOMat.Api.Entities;
using WalOMat.Api.Models;

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
        _logger.LogInformation($"Share preview request for ID: {id}");

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
            _logger.LogError(ex, $"Error fetching result {id}");
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
        
        // Parse top result
        var results = JsonSerializer.Deserialize<List<ResultDto>>(entity.ResultsJson);
        var topResult = results?.OrderByDescending(r => r.Percentage).FirstOrDefault();
        
        if (topResult == null)
        {
            return GenerateFallbackHtml(baseUrl, resultId);
        }

        // Map whale IDs to names and images
        var whaleData = GetWhaleData(topResult.WhaleId);
        var imageUrl = $"{baseUrl}/{whaleData.ImagePath}";
        var pageUrl = $"{baseUrl}/result/{resultId}";
        var title = $"Mein Wal-O-Mat Ergebnis: {whaleData.NameDe}";
        var description = $"Ich bin {topResult.Percentage:F0}% {whaleData.NameDe}! Finde heraus, welcher Wal du bist.";

        return $@"<!DOCTYPE html>
<html lang=""de"">
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

    private (string NameDe, string NameEn, string ImagePath) GetWhaleData(string whaleId)
    {
        // Map whale IDs to their data (must match IDs in whales.json)
        return whaleId switch
        {
            "orca" => ("Orca", "Orca", "images/whales/orca1.webp"),
            "blue" => ("Blauwal", "Blue Whale", "images/whales/blue1.webp"),
            "humpback" => ("Buckelwal", "Humpback Whale", "images/whales/humpback1.webp"),
            "beluga" => ("Beluga", "Beluga Whale", "images/whales/beluga1.webp"),
            "sperm" => ("Pottwal", "Sperm Whale", "images/whales/sperm1.webp"),
            "narwhal" => ("Narwal", "Narwhal", "images/whales/narwhal1.webp"),
            "gray" => ("Grauwal", "Gray Whale", "images/whales/gray1.webp"),
            "pilot" => ("Grindwal", "Pilot Whale", "images/whales/pilot1.webp"),
            "minke" => ("Zwergwal", "Minke Whale", "images/whales/minke1.webp"),
            "bowhead" => ("Grönlandwal", "Bowhead Whale", "images/whales/bowhead1.webp"),
            _ => ("Wal", "Whale", "images/whales/orca1.webp")
        };
    }
}

