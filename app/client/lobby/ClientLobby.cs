using System.Linq;
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
        
        Log.Info(nameof(ClientLobby), nameof(OnPlayerAdded), $"Added {playerId}.");

        _startGameButton.Disabled = IsStartGameButtonDisabled();
        
        if (Client.Instance.QuickStartEnabled) // TODO also check for player count when quick launching multiple clients
        {
            Client.Instance.StartGame();
        }
    }

    /// <summary>
    /// Callback from the server for each client to update their original player selection
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void ChangeSelectedOriginalPlayerForPlayer(int playerId, int originalPlayerStableId)
    {
        base.ChangeSelectedOriginalPlayerForPlayer(playerId, originalPlayerStableId);
        Log.Info(nameof(ClientLobby), nameof(ChangeSelectedOriginalPlayerForPlayer), 
            $"{nameof(playerId)} '{playerId}', {nameof(originalPlayerStableId)} '{originalPlayerStableId}'");
        
        _startGameButton.Disabled = IsStartGameButtonDisabled();
    }
    
    /// <summary>
    /// Callback from the server for each client to update their player ready status
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void ChangeReadyStatusForPlayer(int playerId, bool newReadyStatus)
    {
        base.ChangeReadyStatusForPlayer(playerId, newReadyStatus);
        Log.Info(nameof(ClientLobby), nameof(ChangeReadyStatusForPlayer), 
            $"{nameof(playerId)} '{playerId}', {nameof(newReadyStatus)} '{newReadyStatus}'");

        _startGameButton.Disabled = IsStartGameButtonDisabled();
    }

    private void OnGameStarted()
    {
        Log.Info(nameof(ClientLobby), nameof(OnGameStarted), 
            $"Game starting for client. Players: {JsonConvert.SerializeObject(Players.Instance.GetAll())}");
        
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
        Log.Info(nameof(ClientLobby), nameof(UpdateSaveAdded), string.Empty);

        var save = Data.Instance.Save;
        if (save is null)
            return;
        
        UpdateLoadSaveUi(save);
    }

    private void OnLoadSavePressed() => _loadSaveDialog.PopupCentered();

    private void OnStartGamePressed()
    {
        Log.Info(nameof(ClientLobby), nameof(OnStartGamePressed), "Start game button pressed.");
        Client.Instance.StartGame();
    }
    
    private static bool IsStartGameButtonDisabled() => Players.Instance.AllReady is false 
                                                       || IsSavedGameFilled() is false;

    private static bool IsSavedGameFilled()
    {
        var save = Data.Instance.Save;
        if (save is null)
            return true;

        if (Players.Instance.Count < save.Players.Count)
            return false;

        foreach (var (originalPlayerStableId, _) in save.Players)
        {
            if (Players.Instance.GetAll().Any(p => p.StableId == originalPlayerStableId) is false)
                return false;
        }

        return true;
    }

    private void UpdateLoadSaveUi(Save save)
    {
        _loadSaveLabel.Text = $"Loaded ({save.SavedAtUtc.ToLocalTime():g}): {save.GameId}";
        _loadSaveButton.Visible = false;
        
        Callable.From(() => _startGameButton.Disabled = IsStartGameButtonDisabled()).CallDeferred();
    }
    
    private void OnSaveUpdated(Save save) => UpdateLoadSaveUi(save);
}
