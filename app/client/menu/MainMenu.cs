using Godot;

public class MainMenu : VBoxContainer
{
    public const string ScenePath = @"res://app/client/menu/MainMenu.tscn";

    private LineEdit _nameInput;
    private OptionButton _factionInput;
    private Button _connectButton;
    private Button _playLocallyButton;
    private Label _errorMessage;
    private Tween _tween;

    public override void _Ready()
    {
        _nameInput = GetNode<LineEdit>("Name/Input");
        _factionInput = GetNode<OptionButton>("Faction/Input");
        _connectButton = FindNode("Connect") as Button;
        _playLocallyButton = FindNode("PlayLocally") as Button;
        _errorMessage = GetNode<Label>("Connect/ErrorMessage"); // TODO consolidate all error messages in scene under one 
        _tween = GetNode<Tween>("Tween");

        if (OS.HasEnvironment(Constants.Os.Username))
        {
            _nameInput.Text = OS.GetEnvironment(Constants.Os.Username);
        }
        else
        {
            // TODO refactor desktopPath so that it's unit testable
            var desktopPath = OS.GetSystemDir(0).Replace("\\", "/").Split("/");
            _nameInput.Text = desktopPath[desktopPath.Length - 2];
        }
        
        GetTree().Connect(Constants.ENet.ConnectedToServerEvent, this, nameof(OnConnectedToServer));
        _connectButton?.Connect(nameof(_connectButton.Pressed).ToLower(), this, nameof(OnConnectPressed));
        _playLocallyButton?.Connect(nameof(_playLocallyButton.Pressed).ToLower(), this, nameof(OnPlayLocallyPressed));
    }

    private void ConnectToServer() // TODO use enum for faction instead of int
    {
        PutConnectionMessage("Connecting to server");
        _connectButton.Disabled = true;
        
        if (Client.Instance.JoinGame(_nameInput.Text, _factionInput.Selected) is false) // TODO test if singleton pattern works as expected
        {
            PutConnectionMessage("Failed to connect");
        }
        
        _connectButton.Disabled = false;
    }

    private void PutConnectionMessage(string message)
    {
        _errorMessage.Text = message;
        _tween.InterpolateProperty(_errorMessage, "self_modulate", new Color(1, 1, 1, 1), 
            new Color(1, 1, 1, 0), 2, Tween.TransitionType.Linear, Tween.EaseType.Out);
        _tween.Start();
    }

    private void OnConnectedToServer()
    {
        GetTree().ChangeScene(ClientLobby.ScenePath);
    }

    private void OnConnectPressed()
    {
        Constants.SetRemoteServer();
        ConnectToServer();
    }

    private void OnPlayLocallyPressed()
    {
        Constants.SetLocalServer();
        ConnectToServer();
    }
}
