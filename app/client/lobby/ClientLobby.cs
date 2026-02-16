using Godot;
using Newtonsoft.Json;

public partial class ClientLobby : Lobby
{
    public const string ScenePath = @"res://app/client/lobby/ClientLobby.tscn";

    private Button _loadSaveButton = null!;
    private FileDialog _loadSaveDialog = null!;
    private Label _loadSaveLabel = null!;
    private Button _startGameButton = null!;
    
    public override void _Ready()
    {
        base._Ready();

        _loadSaveButton = GetNode<Button>("Save/Loading/LoadSave");
        _loadSaveDialog = GetNode<FileDialog>($"Save/Loading/LoadSave/{nameof(FileDialog)}");
        _loadSaveLabel = GetNode<Label>("Save/Loading/LoadSaveLabel");
        _startGameButton = GetNode<Button>("StartGame");

        _loadSaveLabel.Text = "";

        _loadSaveButton.Pressed += OnLoadSavePressed;
        _loadSaveDialog.FileSelected += OnLoadSaveDialogFileSelected;
        _startGameButton.Pressed += OnStartGamePressed;
        Client.Instance.GameStarted += OnGameStarted;
        Data.Instance.SaveUpdated += OnSaveUpdated;
        
        // Tell the server about you (client)
        Server.Instance.RegisterSelf(
            Multiplayer.GetUniqueId(), 
            Client.Instance.LocalPlayerName, 
            Client.Instance.LocalPlayerReady,
            Client.Instance.LocalPlayerFaction);
    }

    public override void _ExitTree()
    {
        _loadSaveButton.Pressed -= OnLoadSavePressed;
        _loadSaveDialog.FileSelected -= OnLoadSaveDialogFileSelected;
        _startGameButton.Pressed -= OnStartGamePressed;
        Client.Instance.GameStarted -= OnGameStarted;
        
        base._ExitTree();
    }

    protected override void OnPlayerAdded(int playerId)
    {
        base.OnPlayerAdded(playerId);
        
        GD.Print($"{nameof(ClientLobby)}.{nameof(OnPlayerAdded)}");

        _startGameButton.Disabled = IsStartGameButtonDisabled();
        
        if (Client.Instance.QuickStartEnabled) // TODO also check for player count when quick launching multiple clients
        {
            Client.Instance.StartGame();
        }
    }
    
    /// <summary>
    /// Callback from the server for each client to update their player ready status
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void ChangeReadyStatusForPlayer(int playerId, bool newReadyStatus)
    {
        base.ChangeReadyStatusForPlayer(playerId, newReadyStatus);
        GD.Print($"{nameof(ClientLobby)}.{nameof(ChangeReadyStatusForPlayer)}");

        _startGameButton.Disabled = IsStartGameButtonDisabled();
    }

    private void OnGameStarted()
    {
        GD.Print($"{nameof(ClientLobby)}: game starting for client...");
        Callable.From(() => GetTree().ChangeSceneToFile(ClientGame.ScenePath)).CallDeferred();
    }
    
    private void OnLoadSaveDialogFileSelected(string path)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var save = file.GetAsText();
        file.Close();

        if (save is null)
            return;
        
        OnPlayerSaveAdded(save);
    }
    
    /// <summary>
    /// Callback from the server for each client to add the saved game
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void UpdateSaveAdded(string savePayload)
    {
        base.UpdateSaveAdded(savePayload);
        GD.Print($"{nameof(ClientLobby)}.{nameof(UpdateSaveAdded)}");

        var save = Data.Instance.Save;
        if (save is null)
            return;
        
        UpdateLoadSaveUi(save);
    }

    private void OnLoadSavePressed() => _loadSaveDialog.PopupCentered();

    private void OnStartGamePressed()
    {
        GD.Print($"{nameof(ClientLobby)}: start game button pressed.");
        Client.Instance.StartGame();
    }
    
    private static bool IsStartGameButtonDisabled()
    {
        // TODO also check if enough players have joined for the save game AND all stable IDs are picked (no overlaps)
        return Players.Instance.AllReady is false;
    }

    private void UpdateLoadSaveUi(Save save)
    {
        _loadSaveLabel.Text = $"Loaded ({save.SavedAtUtc.ToLocalTime():g}): {save.GameId}";
        _loadSaveButton.Visible = false;
    }
    
    private void OnSaveUpdated(Save save) => UpdateLoadSaveUi(save);
}
