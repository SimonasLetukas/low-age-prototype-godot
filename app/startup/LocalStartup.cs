using Godot;

/// <summary>
/// Allows to start as client or server when app is launched locally (usually when debugging).
/// </summary>
public class LocalStartup : HBoxContainer
{
    public const string ScenePath = @"res://app/startup/LocalStartup.tscn";

    private Button _startAsServerButton;
    private Button _startAsClientButton;

    public override void _Ready()
    {
        _startAsServerButton = GetNode<Button>("StartAsServer");
        _startAsClientButton = GetNode<Button>("StartAsClient");
        
        Constants.SetLocalServer();

        _startAsServerButton?.Connect(nameof(_startAsServerButton.Pressed).ToLower(), this, 
            nameof(OnStartAsServerPressed));
        _startAsClientButton?.Connect(nameof(_startAsClientButton.Pressed).ToLower(), this, 
            nameof(OnStartAsClientPressed));
    }

    private void OnStartAsServerPressed()
    {
        GetTree().ChangeScene(ServerStartup.ScenePath);
    }

    private void OnStartAsClientPressed()
    {
        GetTree().ChangeScene(ClientStartup.ScenePath);
    }
}
