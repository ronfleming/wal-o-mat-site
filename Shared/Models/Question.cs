namespace WalOMat.Shared.Models;

/// <summary>
/// A quiz question that maps user preferences to whale traits.
/// Supports both German and English text.
/// </summary>
public class Question
{
    public required string Id { get; init; }
    public required string TextDe { get; init; }
    public required string TextEn { get; init; }
    public string? Category { get; init; }
    
    /// <summary>
    /// Gets the text in the specified language.
    /// </summary>
    public string GetText(string language) => language == "en" ? TextEn : TextDe;
    
    // For backward compatibility
    public string Text => TextDe;
    
    /// <summary>
    /// Position each whale takes on this question (-1 = disagree, 0 = neutral, 1 = agree).
    /// Key is the whale's Id.
    /// </summary>
    public required Dictionary<string, int> WhalePositions { get; init; }
}
