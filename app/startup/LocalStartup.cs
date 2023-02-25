using Godot;

public class LocalStartup : HBoxContainer
{
    public const string ScenePath = @"res://app/startup/LocalStartup.tscn";

    public override void _Ready()
    {
        Constants.SetLocalServer();
    }

    public void _on_StartAsServer_pressed()
    {
        // TODO
    }
    
    public void _on_StartAsClient_pressed()
    {
        // TODO
    }
}
