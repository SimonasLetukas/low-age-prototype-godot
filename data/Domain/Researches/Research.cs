using LowAgeCommon;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Factions;

namespace LowAgeData.Domain.Researches;

public class Research : IDisplayable
{
    public Research(
        ResearchId id,
        string displayName,
        string description,
        FactionId faction,
        string sprite)
    {
        Id = id;
        DisplayName = displayName;
        Description = description;
        Faction = faction;
        Sprite = sprite;
        CenterOffset = Vector2Int.Zero;
    }
    
    public ResearchId Id { get; }
    public string DisplayName { get; }
    public string Description { get; }
    public FactionId Faction { get; }
    public string? Sprite { get; }
    public Vector2Int CenterOffset { get; }
}