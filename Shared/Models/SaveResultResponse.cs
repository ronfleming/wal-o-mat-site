namespace WalOMat.Shared.Models;

/// <summary>
/// Response after saving a quiz result.
/// </summary>
public class SaveResultResponse
{
    public string ResultId { get; set; } = string.Empty;
    public string ShareUrl { get; set; } = string.Empty;
}

