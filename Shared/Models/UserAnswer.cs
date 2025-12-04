namespace WalOMat.Shared.Models;

/// <summary>
/// A user's response to a single question.
/// </summary>
public class UserAnswer
{
    public required string QuestionId { get; init; }
    
    /// <summary>
    /// User's position: -1 = disagree, 0 = neutral, 1 = agree, null = skipped.
    /// </summary>
    public int? Position { get; init; }
    
    /// <summary>
    /// Whether this question should count double in scoring.
    /// </summary>
    public bool IsWeighted { get; init; }
}

