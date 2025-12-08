namespace WalOMat.Shared.Models;

/// <summary>
/// A whale species profile with its characteristics and imagery.
/// Supports both German and English text.
/// </summary>
public class WhaleProfile
{
    public required string Id { get; init; }
    public required string NameDe { get; init; }
    public required string NameEn { get; init; }
    public required string ScientificName { get; init; }
    public required string DescriptionDe { get; init; }
    public required string DescriptionEn { get; init; }
    public required string ImagePath { get; init; }
    
    /// <summary>
    /// Key personality traits in German.
    /// </summary>
    public required List<string> TraitsDe { get; init; }
    
    /// <summary>
    /// Key personality traits in English.
    /// </summary>
    public required List<string> TraitsEn { get; init; }
    
    // Convenience accessors
    public string GetName(string language) => language == "en" ? NameEn : NameDe;
    public string GetDescription(string language) => language == "en" ? DescriptionEn : DescriptionDe;
    public List<string> GetTraits(string language) => language == "en" ? TraitsEn : TraitsDe;
    
    // For backward compatibility
    public string Name => NameDe;
    public string Description => DescriptionDe;
    public List<string> Traits => TraitsDe;
}
