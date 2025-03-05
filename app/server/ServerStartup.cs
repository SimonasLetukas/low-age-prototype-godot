using Godot;

/// <summary>
/// Setup server and open a new lobby.
/// </summary>
public partial class ServerStartup : Node
{
    public const string ScenePath = @"res://app/server/ServerStartup.tscn";

    public override void _Ready()
    {
        Callable.From(() => GetTree().ChangeSceneToFile(ServerLobby.ScenePath)).CallDeferred();
    }
}
