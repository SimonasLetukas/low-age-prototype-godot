using Godot;
using System.Linq;
using low_age_data.Domain.Factions;

public class Lobby : VBoxContainer
{
    private VBoxContainer _playersList;
    
    public override void _Ready()
    {
        _playersList = GetNode<VBoxContainer>("Players");
        
        Client.Instance.Connect(nameof(Client.PlayerAdded), this, nameof(OnPlayerAdded));
        Client.Instance.Connect(nameof(Network.PlayerRemoved), this, nameof(OnPlayerRemoved));
    }

    private void OnPlayerAdded(int playerId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerAdded)}: adding player {playerId} to lobby.");
        
        var playerInfo = (PlayerInLobby) GD.Load<PackedScene>(PlayerInLobby.ScenePath).Instance();
        _playersList.AddChild(playerInfo);
        
        var player = playerInfo.SetupPlayer(playerId);
        playerInfo.PlayerSelectedFaction += OnPlayerChangedSelectedFaction;
        
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerAdded)}: player {player.Name} ({player.Id}) " +
                 "successfully added to lobby.");
    }

    private void OnPlayerRemoved(int playerId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerRemoved)}: removing player {playerId} from lobby.");
        
        foreach (var player in _playersList.GetChildren().OfType<PlayerInLobby>())
        {
            if (player.Player.Id != playerId) continue;
            
            RemoveChild(player);
            
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
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedSelectedFaction)} called with " +
                 $"{nameof(PlayerInLobby)} '{playerInLobby}', {nameof(newFactionId)} '{newFactionId}'.");

        if (GetTree().IsNetworkServer()) 
            return;
        
        GD.Print($"{nameof(Lobby)}.{nameof(OnPlayerChangedSelectedFaction)}: calling " +
                 $"{nameof(ServerLobby.UpdateSelectedPlayerFaction)}.");
        RpcId(Constants.ServerId, nameof(ServerLobby.UpdateSelectedPlayerFaction), playerInLobby.Player.Id, 
            newFactionId.ToString());
    }
    
    /// <summary>
    /// Callback from the server for each client to update their player faction selection
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="factionId"></param>
    [RemoteSync]
    protected virtual void ChangeSelectedFactionForPlayer(int playerId, string factionId)
    {
        GD.Print($"{nameof(Lobby)}.{nameof(ChangeSelectedFactionForPlayer)}: trying to change player " +
                 $"'{playerId}' faction to '{factionId}'");
        
        foreach (var player in _playersList.GetChildren().OfType<PlayerInLobby>())
        {
            if (player.Player.Id != playerId) continue;
            
            player.FactionSelection.SetSelectedFaction(new FactionId(factionId));
            
            GD.Print($"{nameof(Lobby)}.{nameof(ChangeSelectedFactionForPlayer)}: player '{playerId}' changed " +
                     $"faction to '{factionId}'");
            return;
        }
    }
}
