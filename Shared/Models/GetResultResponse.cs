namespace WalOMat.Shared.Models;

/// <summary>
/// Response when retrieving a saved result.
/// </summary>
public class GetResultResponse
{
    public string? ResultId { get; set; }
    public string? Language { get; set; }
    public DateTime? Timestamp { get; set; }
    public List<ResultDto>? Results { get; set; }
    public bool IsExpired { get; set; }
}

