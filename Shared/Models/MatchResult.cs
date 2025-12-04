namespace WalOMat.Shared.Models;

/// <summary>
/// The calculated match between a user and a whale species.
/// </summary>
public class MatchResult
{
    public required string WhaleId { get; init; }
    public required string WhaleName { get; init; }
    public required string ImagePath { get; init; }
    
    /// <summary>
    /// Match percentage (0-100).
    /// </summary>
    public required double MatchPercentage { get; init; }
    
    /// <summary>
    /// Points earned out of maximum possible.
    /// </summary>
    public required int PointsEarned { get; init; }
    public required int PointsMaximum { get; init; }
}

