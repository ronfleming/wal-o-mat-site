namespace WalOMat.Shared.Models;

/// <summary>
/// Lightweight whale metadata for contexts where full WhaleProfile data
/// isn't available (e.g., Azure Functions generating social share previews).
/// 
/// This must stay in sync with Client/wwwroot/data/whales.json.
/// </summary>
public static class WhaleMetadata
{
    public record WhaleInfo(string NameDe, string NameEn, string ImagePath);

    public static readonly Dictionary<string, WhaleInfo> All = new()
    {
        ["orca"] = new("Orca", "Orca", "images/whales/orca1.webp"),
        ["blue"] = new("Blauwal", "Blue Whale", "images/whales/blue1.webp"),
        ["humpback"] = new("Buckelwal", "Humpback Whale", "images/whales/humpback1.webp"),
        ["beluga"] = new("Beluga", "Beluga Whale", "images/whales/beluga1.webp"),
        ["sperm"] = new("Pottwal", "Sperm Whale", "images/whales/sperm1.webp"),
        ["narwhal"] = new("Narwal", "Narwhal", "images/whales/narwhal1.webp"),
        ["gray"] = new("Grauwal", "Gray Whale", "images/whales/gray1.webp"),
        ["pilot"] = new("Grindwal", "Pilot Whale", "images/whales/pilot1.webp"),
        ["minke"] = new("Zwergwal", "Minke Whale", "images/whales/minke1.webp"),
        ["bowhead"] = new("Grönlandwal", "Bowhead Whale", "images/whales/bowhead1.webp"),
    };

    /// <summary>
    /// Gets whale info by ID, falling back to a generic entry if not found.
    /// </summary>
    public static WhaleInfo Get(string whaleId)
    {
        return All.TryGetValue(whaleId, out var info)
            ? info
            : new WhaleInfo("Wal", "Whale", "images/whales/orca1.webp");
    }

    /// <summary>
    /// Gets the localized whale name.
    /// </summary>
    public static string GetName(string whaleId, string language)
    {
        var info = Get(whaleId);
        return language == "en" ? info.NameEn : info.NameDe;
    }
}

