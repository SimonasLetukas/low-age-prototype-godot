using Godot;

public class ServerStartup : Node
{
    // Setup server and open a new lobby
    public override void _Ready()
    {
        GetTree().ChangeScene(ServerLobby.ScenePath);
    }
}
