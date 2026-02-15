using Godot;

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
    /// <param name="playerId"></param>
    /// <param name="newReadyStatus"></param>
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
        // TODO
        // load file,
        // serialize,
        // update label,
        // distribute save object to everyone,
        // change how players in lobby look like (ability to select player in saved game) for everyone,
        // disable loading for everyone
        
        _loadSaveButton.Visible = false;
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
}
