namespace WalOMat.Shared.Models;

/// <summary>
/// A quiz question that maps user preferences to whale traits.
/// </summary>
public class Question
{
    public required string Id { get; init; }
    public required string Text { get; init; }
    public string? Category { get; init; }
    
    /// <summary>
    /// Position each whale takes on this question (-1 = disagree, 0 = neutral, 1 = agree).
    /// Key is the whale's Id.
    /// </summary>
    public required Dictionary<string, int> WhalePositions { get; init; }
}

