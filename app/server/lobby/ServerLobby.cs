using Godot;

public partial class ServerLobby : Lobby
{
    public const string ScenePath = @"res://app/server/lobby/ServerLobby.tscn";
    
    public override void _Ready()
    {
        base._Ready();
        
        if (Server.Instance.HostGame() is false)
        {
            GD.Print("Failed to start server, shutting down.");
            GetTree().Quit();
        }

        Client.Instance.GameStarted += OnGameStarted;
    }

    public override void _ExitTree()
    {
        Client.Instance.GameStarted -= OnGameStarted;
        
        base._ExitTree();
    }

    protected override void OnPlayerRemoved(long playerId)
    {
        base.OnPlayerRemoved(playerId);
        
        if (Players.Instance.Count == 0)
        {
            GD.Print($"{nameof(ServerLobby)}.{nameof(OnPlayerRemoved)}: not enough players to keep the lobby " +
                     $"state, resetting.");
            
            Server.Instance.ResetNetwork();
            
            Callable.From(() => GetTree().ChangeSceneToFile(ScenePath)).CallDeferred();
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected override void RequestUpdateSaveAdded(string savePayload)
    {
        GD.Print($"{nameof(ServerLobby)}.{nameof(RequestUpdateSaveAdded)}: called.");
        
        Rpc(nameof(UpdateSaveAdded), savePayload);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected override void UpdateSelectedOriginalPlayer(int playerId, int originalPlayerStableId)
    {
        GD.Print($"{nameof(ServerLobby)}.{nameof(UpdateSelectedOriginalPlayer)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(originalPlayerStableId)} '{originalPlayerStableId}'.");
        
        Rpc(nameof(ChangeSelectedOriginalPlayerForPlayer), playerId, originalPlayerStableId);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected override void UpdateSelectedPlayerFaction(int playerId, string factionId)
    {
        GD.Print($"{nameof(ServerLobby)}.{nameof(UpdateSelectedPlayerFaction)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(factionId)} '{factionId}'.");
        
        Rpc(nameof(ChangeSelectedFactionForPlayer), playerId, factionId);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected override void UpdateSelectedPlayerTeam(int playerId, int team)
    {
        GD.Print($"{nameof(ServerLobby)}.{nameof(UpdateSelectedPlayerTeam)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(team)} '{team}'.");
        
        Rpc(nameof(ChangeSelectedTeamForPlayer), playerId, team);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected override void UpdatePlayerReadyStatus(int playerId, bool newReadyStatus)
    {
        GD.Print($"{nameof(ServerLobby)}.{nameof(UpdatePlayerReadyStatus)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(newReadyStatus)} '{newReadyStatus}'.");
        
        Rpc(nameof(ChangeReadyStatusForPlayer), playerId, newReadyStatus);
    }

    private void OnGameStarted()
    {
        GD.Print("Game starting for server...");
        Callable.From(() => GetTree().ChangeSceneToFile(ServerGame.ScenePath)).CallDeferred();
    }
}
