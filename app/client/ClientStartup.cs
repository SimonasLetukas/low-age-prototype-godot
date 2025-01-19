using Godot;

/// <summary>
/// Setup client and open the main menu.
/// </summary>
public partial class ClientStartup : Node2D
{
    public const string ScenePath = @"res://app/client/ClientStartup.tscn";
    
    public override void _Ready()
    {
        Callable.From(() => GetTree().ChangeSceneToFile(MainMenu.ScenePath)).CallDeferred();
    }
}
