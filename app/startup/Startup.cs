using Godot;

/// <summary>
/// Determine whether application is started as client or server (or locally) and then route to the appropriate scenes.
/// </summary>
public class Startup : Node
{
    public string Client = nameof(Client).ToLower();
    public string Server = nameof(Server).ToLower();

    public override void _Ready()
    {
        GD.Print("Application started.");
        
        if (OS.HasFeature(Client))
        {
            GD.Print($"Starting as {Client}.");
            GetTree().ChangeScene(ClientStartup.ScenePath);
            return;
        }

        if (OS.HasFeature(Server))
        {
            GD.Print($"Starting as {Server}.");
            GetTree().ChangeScene(ServerStartup.ScenePath);
            return;
        }
        
        GD.Print($"Unidentified startup, starting as {nameof(DebugQuickStart)}.");
        var result = GetTree().ChangeScene(DebugQuickStart.ScenePath);
    }
}
