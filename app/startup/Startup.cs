using Godot;

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
            // TODO
        }

        if (OS.HasFeature(Server))
        {
            GD.Print($"Starting as {Server}.");
            // TODO
        }
        
        GD.Print($"Unidentified startup, starting as local {Client}.");
        GetTree().ChangeScene(LocalStartup.ScenePath);
    }
}
