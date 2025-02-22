using Godot;
using System.Linq;
using LowAgeData.Domain.Factions;
using MultipurposePathfinding;

public partial class Lobby : VBoxContainer
{
    private VBoxContainer _playersList = null!;
    
    public override void _Ready()
    {
        _playersList = GetNode<VBoxContainer>("Players");
        
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
        
        playerInfo.PlayerSelectedFaction += OnPlayerChangedSelectedFaction;
        playerInfo.PlayerSelectedTeam += OnPlayerChangedSelectedTeam;
        playerInfo.PlayerChangedReadyStatus += OnPlayerChangedReadyStatus;

        var player = Players.Instance.Get(playerId);
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerAdded)}: player {player.Name} ({player.Id}) " +
                 "successfully added to lobby.");
    }

    private void OnPlayerRemoved(long playerId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerRemoved)}: removing player {playerId} from lobby.");
        
        foreach (var playerInfo in _playersList.GetChildren().OfType<PlayerInLobby>())
        {
            if (playerInfo.Player.Id != playerId)
            {
                playerInfo.AdjustMaxSelectedTeam();
                continue;
            }
            
            playerInfo.PlayerSelectedFaction -= OnPlayerChangedSelectedFaction;
            playerInfo.PlayerSelectedTeam -= OnPlayerChangedSelectedTeam;
            playerInfo.PlayerChangedReadyStatus -= OnPlayerChangedReadyStatus;
            playerInfo.QueueFree();
            
            GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerRemoved)}: player {playerId} successfully removed from lobby.");
            return;
        }
        
        GD.PrintErr($"{nameof(Lobby)}.{nameof(OnPlayerRemoved)}: player {playerId} could not be removed from lobby.");
    }

    /// <summary>
    /// Calls the server that the faction selection has changed
    /// </summary>
    /// <param name="playerInLobby"></param>
    /// <param name="newFactionId"></param>
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
    /// <param name="playerId"></param>
    /// <param name="factionId"></param>
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
    /// <param name="playerInLobby"></param>
    /// <param name="newTeam"></param>
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
    /// <param name="playerId"></param>
    /// <param name="team"></param>
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
    /// <param name="playerInLobby"></param>
    /// <param name="newReadyStatus"></param>
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
    /// <param name="playerId"></param>
    /// <param name="newReadyStatus"></param>
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
