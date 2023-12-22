using Godot;

public class Config : Node
{
    public static Config Instance = null;

    public AnimationSpeeds AnimationSpeed { get; set; } = AnimationSpeeds.Fast;
    
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