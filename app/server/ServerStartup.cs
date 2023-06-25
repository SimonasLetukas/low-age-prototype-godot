using Godot;

/// <summary>
/// Setup server and open a new lobby.
/// </summary>
public class ServerStartup : Node
{
    public const string ScenePath = @"res://app/server/ServerStartup.tscn";

    public override void _Ready()
    {
        GetTree().ChangeScene(ServerLobby.ScenePath);
    }
}
