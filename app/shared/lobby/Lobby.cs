using Godot;
using System.Linq;
using LowAgeData.Domain.Factions;
using MultipurposePathfinding;
using Newtonsoft.Json;

public partial class Lobby : VBoxContainer
{
    private VBoxContainer _playersList = null!;
    
    public override void _Ready()
    {
        _playersList = GetNode<VBoxContainer>("Players/PlayersList");
        
        Client.Instance.PlayerAdded += OnPlayerAdded;
        Client.Instance.PlayerRemoved += OnPlayerRemoved;
    }

    public override void _ExitTree()
    {
        Client.Instance.PlayerAdded -= OnPlayerAdded;
        Client.Instance.PlayerRemoved -= OnPlayerRemoved;
        
        base._ExitTree();
    }

    protected virtual void OnPlayerAdded(int playerId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerAdded)}: adding player {playerId} to lobby.");

        var playerInfo = PlayerInLobby.Instance();
        _playersList.AddChild(playerInfo);
        playerInfo.GetTree().SetMultiplayer(Multiplayer);
        
        Callable.From(() => playerInfo.SetupPlayer(playerId)).CallDeferred();

        playerInfo.PlayerSelectedOriginalPlayer += OnPlayerChangedSelectedOriginalPlayer;
        playerInfo.PlayerSelectedFaction += OnPlayerChangedSelectedFaction;
        playerInfo.PlayerSelectedTeam += OnPlayerChangedSelectedTeam;
        playerInfo.PlayerChangedReadyStatus += OnPlayerChangedReadyStatus;

        var player = Players.Instance.GetById(playerId);
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerAdded)}: player {player.Name} ({player.Id}) " +
                 "successfully added to lobby.");
    }

    protected virtual void OnPlayerRemoved(long playerId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerRemoved)}: removing player {playerId} from lobby.");
        
        foreach (var playerInfo in _playersList.GetChildren().OfType<PlayerInLobby>())
        {
            if (playerInfo.Player.Id != playerId)
            {
                playerInfo.AdjustMaxSelectedTeam();
                continue;
            }
            
            playerInfo.PlayerSelectedOriginalPlayer -= OnPlayerChangedSelectedOriginalPlayer;
            playerInfo.PlayerSelectedFaction -= OnPlayerChangedSelectedFaction;
            playerInfo.PlayerSelectedTeam -= OnPlayerChangedSelectedTeam;
            playerInfo.PlayerChangedReadyStatus -= OnPlayerChangedReadyStatus;
            playerInfo.QueueFree();
            
            GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerRemoved)}: player {playerId} successfully removed from lobby.");
        }
        
        GD.PrintErr($"{nameof(Lobby)}.{nameof(OnPlayerRemoved)}: player {playerId} could not be removed from lobby.");
    }
    
    /// <summary>
    /// Calls the server that a new save has been loaded
    /// </summary>
    protected void OnPlayerSaveAdded(string savePayload)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerSaveAdded)}: called.");
        RpcId(Constants.ENet.ServerId, nameof(RequestUpdateSaveAdded), savePayload);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected virtual void RequestUpdateSaveAdded(string savePayload)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(RequestUpdateSaveAdded)}: called.");
    }
    
    /// <summary>
    /// Callback from the server for each client to add the saved game
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected virtual void UpdateSaveAdded(string savePayload)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(UpdateSaveAdded)}: trying to add saved game.");
        
        var save = JsonConvert.DeserializeObject<Save>(savePayload);

        if (save is null)
        {
            GD.Print($"{nameof(Lobby)}.{nameof(UpdateSaveAdded)}: could not add saved game.");
            return;
        }

        Data.Instance.SetSave(save);
    }
    
    /// <summary>
    /// Calls the server that the original player selection has changed
    /// </summary>
    private void OnPlayerChangedSelectedOriginalPlayer(PlayerInLobby playerInLobby, int originalPlayerStableId)
    {
        var playerId = playerInLobby.Player.Id;
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedSelectedOriginalPlayer)} called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(originalPlayerStableId)} '{originalPlayerStableId}'.");

        if (Multiplayer.IsServer()) 
            return;
        
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedSelectedOriginalPlayer)}: calling " +
                 $"{nameof(UpdateSelectedOriginalPlayer)}.");
        RpcId(Constants.ENet.ServerId, nameof(UpdateSelectedOriginalPlayer), playerId, 
            originalPlayerStableId);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected virtual void UpdateSelectedOriginalPlayer(int playerId, int originalPlayerStableId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(UpdateSelectedOriginalPlayer)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(originalPlayerStableId)} '{originalPlayerStableId}'.");
    }
    
    /// <summary>
    /// Callback from the server for each client to update their original player selection
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected virtual void ChangeSelectedOriginalPlayerForPlayer(int playerId, int originalPlayerStableId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(ChangeSelectedOriginalPlayerForPlayer)}: trying to change player " +
                 $"'{playerId}' selected original player to '{originalPlayerStableId}'");
        
        foreach (var player in _playersList.GetChildren().OfType<PlayerInLobby>())
        {
            if (player.Player.Id != playerId) continue;
            
            player.SetOriginalPlayer(originalPlayerStableId);
            
            GD.Print($"{nameof(Lobby)}.{nameof(ChangeSelectedOriginalPlayerForPlayer)}: player '{playerId}' " +
                     $"changed selected original player to '{originalPlayerStableId}'");
            return;
        }
    }

    /// <summary>
    /// Calls the server that the faction selection has changed
    /// </summary>
    private void OnPlayerChangedSelectedFaction(PlayerInLobby playerInLobby, FactionId newFactionId)
    {
        var playerId = playerInLobby.Player.Id;
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedSelectedFaction)} called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(newFactionId)} '{newFactionId}'.");

        if (Multiplayer.IsServer()) 
            return;
        
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedSelectedFaction)}: calling " +
                 $"{nameof(UpdateSelectedPlayerFaction)}.");
        RpcId(Constants.ENet.ServerId, nameof(UpdateSelectedPlayerFaction), playerId, 
            newFactionId.ToString());
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected virtual void UpdateSelectedPlayerFaction(int playerId, string factionId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(UpdateSelectedPlayerFaction)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(factionId)} '{factionId}'.");
    }
    
    /// <summary>
    /// Callback from the server for each client to update their player faction selection
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected virtual void ChangeSelectedFactionForPlayer(int playerId, string factionId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(ChangeSelectedFactionForPlayer)}: trying to change player " +
                 $"'{playerId}' faction to '{factionId}'");
        
        foreach (var player in _playersList.GetChildren().OfType<PlayerInLobby>())
        {
            if (player.Player.Id != playerId) continue;
            
            player.SetSelectedFaction(new FactionId(factionId));
            
            GD.Print($"{nameof(Lobby)}.{nameof(ChangeSelectedFactionForPlayer)}: player '{playerId}' changed " +
                     $"faction to '{factionId}'");
            return;
        }
    }
    
    /// <summary>
    /// Calls the server that the team selection has changed
    /// </summary>
    private void OnPlayerChangedSelectedTeam(PlayerInLobby playerInLobby, Team newTeam)
    {
        var playerId = playerInLobby.Player.Id;
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedSelectedTeam)} called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(newTeam)} '{newTeam}'.");

        if (Multiplayer.IsServer()) 
            return;
        
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedSelectedTeam)}: calling " +
                 $"{nameof(UpdateSelectedPlayerTeam)}.");
        RpcId(Constants.ENet.ServerId, nameof(UpdateSelectedPlayerTeam), playerId, 
            newTeam.Value);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected virtual void UpdateSelectedPlayerTeam(int playerId, int team)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(UpdateSelectedPlayerTeam)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(team)} '{team}'.");
    }
    
    /// <summary>
    /// Callback from the server for each client to update their player team selection
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected virtual void ChangeSelectedTeamForPlayer(int playerId, int team)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(ChangeSelectedTeamForPlayer)}: trying to change player " +
                 $"'{playerId}' team to '{team}'");
        
        foreach (var player in _playersList.GetChildren().OfType<PlayerInLobby>())
        {
            if (player.Player.Id != playerId) continue;
            
            player.SetSelectedTeam(new Team(team));
            
            GD.Print($"{nameof(Lobby)}.{nameof(ChangeSelectedTeamForPlayer)}: player '{playerId}' changed " +
                     $"team to '{team}'");
            return;
        }
    }

    /// <summary>
    /// Calls the server that the player ready status has changed
    /// </summary>
    private void OnPlayerChangedReadyStatus(PlayerInLobby playerInLobby, bool newReadyStatus)
    {
        var playerId = playerInLobby.Player.Id;
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedReadyStatus)} called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(newReadyStatus)} '{newReadyStatus}'.");

        if (Multiplayer.IsServer()) 
            return;
        
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedReadyStatus)}: calling " +
                 $"{nameof(UpdatePlayerReadyStatus)}.");
        RpcId(Constants.ENet.ServerId, nameof(UpdatePlayerReadyStatus), playerId, newReadyStatus);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected virtual void UpdatePlayerReadyStatus(int playerId, bool newReadyStatus)
    {
        GD.Print($"{nameof(ServerLobby)}.{nameof(UpdatePlayerReadyStatus)}: called with " +
                 $"{nameof(playerId)} '{playerId}', {nameof(newReadyStatus)} '{newReadyStatus}'.");
    }
    
    /// <summary>
    /// Callback from the server for each client to update their player ready status
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected virtual void ChangeReadyStatusForPlayer(int playerId, bool newReadyStatus)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(ChangeReadyStatusForPlayer)}: trying to change player " +
                 $"'{playerId}' ready status to '{newReadyStatus}'");
        
        foreach (var player in _playersList.GetChildren().OfType<PlayerInLobby>())
        {
            if (player.Player.Id != playerId) continue;
            
            player.SetReadyStatus(newReadyStatus);
            
            GD.Print($"{nameof(Lobby)}.{nameof(ChangeReadyStatusForPlayer)}: player '{playerId}' changed " +
                     $"ready status to '{newReadyStatus}'");
            return;
        }
    }
}
