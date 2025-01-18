using Godot;

/// <summary>
/// Determine whether application is started as client or server (or locally) and then route to the appropriate scenes.
/// </summary>
public partial class Startup : Node
{
    public string Client = nameof(Client).ToLower();
    public string Server = nameof(Server).ToLower();

    public override void _Ready()
    {
        GD.Print("Application started.");
        
        if (OS.HasFeature(Client))
        {
            GD.Print($"Starting as {Client}.");
            GetTree().ChangeSceneToFile(ClientStartup.ScenePath);
            return;
        }

        if (OS.HasFeature(Server))
        {
            GD.Print($"Starting as {Server}.");
            GetTree().ChangeSceneToFile(ServerStartup.ScenePath);
            return;
        }
        
        GD.Print($"Unidentified startup, starting as {nameof(DebugQuickStart)}.");
        var result = GetTree().ChangeSceneToFile(DebugQuickStart.ScenePath);
    }
}
