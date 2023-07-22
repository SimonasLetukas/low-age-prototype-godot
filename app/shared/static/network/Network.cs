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
    public void ResetNetwork()
    {
	    var peer = GetTree().NetworkPeer as NetworkedMultiplayerENet;
	    if (peer is null is false)
	    {
		    peer.CloseConnection();
	    }

	    Data.Instance.Reset();
    }

    /// <summary>
    /// Every network peer needs to clean up the disconnected client.
    /// </summary>
    private void OnPlayerDisconnected(int playerId)
    {
	    GD.Print("Player disconnected: " + playerId);
	    Data.Instance.RemovePlayer(playerId);
	    
	    EmitSignal(nameof(PlayerRemoved), playerId);

	    GD.Print($"Total players: {Data.Instance.Players.Count}");
    }
}
