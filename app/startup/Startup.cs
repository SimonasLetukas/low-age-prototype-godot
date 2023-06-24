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
        }

        if (OS.HasFeature(Server))
        {
            GD.Print($"Starting as {Server}.");
            // TODO
        }
        
        GD.Print($"Unidentified startup, starting as local {Client}.");
        var result = GetTree().ChangeScene(LocalStartup.ScenePath);
    }
}
