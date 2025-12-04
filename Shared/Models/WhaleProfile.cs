namespace WalOMat.Shared.Models;

/// <summary>
/// A whale species profile with its characteristics and imagery.
/// </summary>
public class WhaleProfile
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string ScientificName { get; init; }
    public required string Description { get; init; }
    public required string ImagePath { get; init; }
    
    /// <summary>
    /// Key personality traits for display on the results page.
    /// </summary>
    public required List<string> Traits { get; init; }
}

