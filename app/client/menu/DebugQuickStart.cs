/// <summary>
/// Quick-starts the game locally. Should only be accessible when debugging.
/// </summary>
public partial class DebugQuickStart : MainMenu
{
    public const string ScenePath = @"res://app/client/menu/DebugQuickStart.tscn";
    
    public override void _Ready()
    {
        base._Ready();
        
        QuickStartCheckBox.ButtonPressed = true;
        
        OnPlayLocallyPressed();
    }
}
