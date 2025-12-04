namespace WalOMat.Shared.Models;

/// <summary>
/// Container for all quiz content loaded from JSON.
/// </summary>
public class QuizData
{
    public required List<Question> Questions { get; init; }
    public required List<WhaleProfile> Whales { get; init; }
}

