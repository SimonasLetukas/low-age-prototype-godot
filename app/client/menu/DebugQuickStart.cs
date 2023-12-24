/// <summary>
/// Quick-starts the game locally. Should only be accessible when debugging.
/// </summary>
public class DebugQuickStart : MainMenu
{
    public const string ScenePath = @"res://app/client/menu/DebugQuickStart.tscn";
    
    public override void _Ready()
    {
        base._Ready();
        
        OnPlayLocallyPressed();
        ConnectToServer();
    }
}
