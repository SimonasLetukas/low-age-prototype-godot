using Godot;

public class Network : Node
{
    [Signal]
    public delegate void PlayerRemoved(int playerId);

    public override void _Ready()
    {
	    ResetNetwork();

        GetTree().Connect(Constants.ENet.NetworkPeerDisconnectedEvent, this, nameof(OnPlayerDisconnected));
    }

    /// <summary>
    /// Completely reset the game state and clear the network.
    /// </summary>
    public void ResetNetwork() // TODO not tested
    {
	    var peer = GetTree().NetworkPeer as NetworkedMultiplayerENet;
	    if (peer is null is false)
	    {
		    peer.CloseConnection();
	    }

	    Data.Reset();
    }

    /// <summary>
    /// Every network peer needs to clean up the disconnected client.
    /// </summary>
    private void OnPlayerDisconnected(int playerId) // TODO not tested
    {
	    GD.Print("Player disconnected: " + playerId);
	    Data.RemovePlayer(playerId);
	    
	    EmitSignal(nameof(PlayerRemoved), playerId);

	    GD.Print($"Total players: {Data.Players.Count}");
    }
}
