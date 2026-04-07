using LowAgeData.Domain.Factions;

namespace LowAgeData.Domain.Researches;

public class Research
{
    public Research(
        ResearchId id,
        string displayName,
        string description,
        FactionId faction)
    {
        Id = id;
        DisplayName = displayName;
        Description = description;
        Faction = faction;
    }
    
    public ResearchId Id { get; }
    public string DisplayName { get; }
    public string Description { get; }
    public FactionId Faction { get; }
}