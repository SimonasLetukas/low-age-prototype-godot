using Godot;
using System;

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

        Client.Instance.Connect(nameof(Client.GameStarted), new Callable(this, nameof(OnGameStarted)));
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void UpdateSelectedPlayerFaction(int playerId, string factionId)
    {
        GD.Print($"{nameof(ServerLobby)}.{nameof(UpdateSelectedPlayerFaction)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(factionId)} '{factionId}'.");
        
        Rpc(nameof(ChangeSelectedFactionForPlayer), playerId, factionId);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void UpdatePlayerReadyStatus(int playerId, bool newReadyStatus)
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
