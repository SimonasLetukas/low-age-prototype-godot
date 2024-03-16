using Godot;
using low_age_data.Domain.Factions;

public class Config : Node
{
    public static Config Instance = null;

    public AnimationSpeeds AnimationSpeed { get; set; } = AnimationSpeeds.Fast;
    public bool ResearchEnabled { get; set; } = false;
    public FactionId StartingFaction { get; set; } = FactionId.Revelators;
    
    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
    }

    public enum AnimationSpeeds
    {
        Slow,
        Medium,
        Fast
    }
}