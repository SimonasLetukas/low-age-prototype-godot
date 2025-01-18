using Godot;
using low_age_data.Domain.Factions;

public partial class MainMenu : Control
{
    public const string ScenePath = @"res://app/client/menu/MainMenu.tscn";

    protected CheckBox QuickStartCheckBox { get; set; }

    private Button _settingsButton;
    private Settings _settings;
    private LineEdit _nameInput;
    private FactionSelection _factionSelection;
    private Button _connectButton;
    private Button _playLocallyButton;
    private Label _errorMessage;
    private Tween _tween;

    private FactionId _currentlySelectedFaction;

    public override void _Ready()
    {
        Data.Instance.ReadBlueprint();  // TODO perhaps fetch from server instead
                                        // TODO this should happen before (and instead of) the read in FactionSelection.cs

        _settingsButton = FindNode("SettingsButton") as Button;
        _nameInput = GetNode<LineEdit>("Items/Name/Input");
        _factionSelection = GetNode<FactionSelection>("Items/Faction/Faction");
        _connectButton = FindNode("Connect") as Button;
        _playLocallyButton = FindNode("PlayLocally") as Button;
        QuickStartCheckBox = FindNode("QuickStart") as CheckBox;
        _errorMessage = GetNode<Label>("Items/Connect/ErrorMessage"); // TODO consolidate all error messages in scene under one 
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
        
        _settings = Settings.Instance();
        AddChild(_settings);
        _settings.FactionSelected += OnFactionSelectionSelected;
        _settings.Visible = false;
        
        _currentlySelectedFaction = _factionSelection.GetSelectedFaction();

        GetTree().Connect(Constants.ENet.ConnectedToServerEvent, new Callable(this, nameof(OnConnectedToServer)));
        _settingsButton?.Connect(nameof(_settingsButton.Pressed).ToLower(), new Callable(this, nameof(OnSettingsPressed)));
        _connectButton?.Connect(nameof(_connectButton.Pressed).ToLower(), new Callable(this, nameof(OnConnectPressed)));
        _playLocallyButton?.Connect(nameof(_playLocallyButton.Pressed).ToLower(), new Callable(this, nameof(OnPlayLocallyPressed)));
        if (_factionSelection is null is false) 
            _factionSelection.FactionSelected += OnFactionSelectionSelected;
    }

    public override void _ExitTree()
    {
        _factionSelection.FactionSelected -= OnFactionSelectionSelected;
        _settings.FactionSelected -= OnFactionSelectionSelected;
    }

    protected void ConnectToServer()
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

    private void OnSettingsPressed() => _settings.Visible = !_settings.Visible;

    private void OnConnectedToServer() => GetTree().ChangeSceneToFile(ClientLobby.ScenePath);

    private void OnConnectPressed()
    {
        Constants.SetRemoteServer();
        ConnectToServer();
    }

    protected void OnPlayLocallyPressed()
    {
        Server.Instance.RunLocalServerInstance();
        Client.Instance.QuickStartEnabled = QuickStartCheckBox.Pressed;
        Constants.SetLocalServer();
        ConnectToServer();
    }

    private void OnFactionSelectionSelected(FactionId factionId)
    {
        _currentlySelectedFaction = factionId;
        _factionSelection.SetSelectedFaction(factionId);
    }
}
