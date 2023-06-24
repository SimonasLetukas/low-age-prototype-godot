using Godot;

/// <summary>
/// Setup client and open the main menu.
/// </summary>
public class ClientStartup : Node2D
{
    public const string ScenePath = @"res://app/client/ClientStartup.tscn";
    
    public override void _Ready()
    {
        GetTree().ChangeScene(MainMenu.ScenePath);
    }
}
