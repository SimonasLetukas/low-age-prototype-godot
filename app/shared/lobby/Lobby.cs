using Godot;
using System.Linq;

public class Lobby : VBoxContainer
{
    public override void _Ready()
    {
        Client.Instance.Connect(nameof(Client.PlayerAdded), this, nameof(OnPlayerAdded));
        Client.Instance.Connect(nameof(Network.PlayerRemoved), this, nameof(OnPlayerRemoved));
    }

    private void OnPlayerAdded(int playerId)
    {
        GD.Print($"Adding player {playerId} to lobby.");
        
        var playerInfo = GD.Load<PackedScene>(PlayerInLobby.ScenePath).Instance();
        playerInfo.SetNetworkMaster(playerId);
        playerInfo.Name = $"{playerId}";

        var player = Data.Instance.Players.Single(x => x.Id.Equals(playerId));
        playerInfo.GetNode<Label>("Name").Text = player.Name;
        
        GetNode<VBoxContainer>("Players").AddChild(playerInfo);
        
        GD.Print($"Player {player.Name} ({player.Id}) successfully added to lobby.");
    }

    private void OnPlayerRemoved(int playerId)
    {
        GD.Print($"Removing player {playerId} from lobby.");
        
        var players = GetNode<VBoxContainer>("Players");
        foreach (var player in players.GetChildren().OfType<Node>())
        {
            if (player.Name != $"{playerId}") continue;
            
            RemoveChild(player);
            GD.Print($"Player {playerId} successfully removed from lobby.");
            return;
        }
        
        GD.PrintErr($"Player {playerId} could not be removed from the lobby.");
    }
}
