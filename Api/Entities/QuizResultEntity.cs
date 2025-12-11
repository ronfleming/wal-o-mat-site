using Azure;
using Azure.Data.Tables;

namespace WalOMat.Api.Entities;

/// <summary>
/// Table Storage entity for persisting quiz results.
/// PartitionKey = "results", RowKey = GUID
/// </summary>
public class QuizResultEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "results";
    public string RowKey { get; set; } = string.Empty; // GUID
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    // Custom properties (stored as JSON strings)
    public string Language { get; set; } = "de";
    public string AnswersJson { get; set; } = string.Empty;
    public string SelectedWhalesJson { get; set; } = string.Empty;
    public string ResultsJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(90);
}

