using Godot;
using low_age_data.Domain.Factions;

public class MainMenu : VBoxContainer
{
    public const string ScenePath = @"res://app/client/menu/MainMenu.tscn";

    private LineEdit _nameInput;
    private FactionSelection _factionSelection;
    private Button _connectButton;
    private Button _playLocallyButton;
    private Label _errorMessage;
    private Tween _tween;

    private FactionId _currentlySelectedFaction;

    public override void _Ready()
    {
        _nameInput = GetNode<LineEdit>("Name/Input");
        _factionSelection = GetNode<FactionSelection>("Faction/Faction");
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
        
        _currentlySelectedFaction = _factionSelection.GetSelectedFaction();

        GetTree().Connect(Constants.ENet.ConnectedToServerEvent, this, nameof(OnConnectedToServer));
        _connectButton?.Connect(nameof(_connectButton.Pressed).ToLower(), this, nameof(OnConnectPressed));
        _playLocallyButton?.Connect(nameof(_playLocallyButton.Pressed).ToLower(), this, nameof(OnPlayLocallyPressed));
        if (_factionSelection is null is false) 
            _factionSelection.FactionSelected += OnFactionSelectionSelected;
    }

    private void ConnectToServer()
    {
        PutConnectionMessage("Connecting to server");
        _connectButton.Disabled = true;
        
        if (Client.Instance.JoinGame(_nameInput.Text, _currentlySelectedFaction) is false)
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

    private void OnFactionSelectionSelected(FactionId factionId)
    {
        _currentlySelectedFaction = factionId;
    }
}
