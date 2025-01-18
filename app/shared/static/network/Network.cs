using Godot;

public partial class Network : Node
{
    [Signal]
    public delegate void PlayerRemoved(int playerId);

    public override void _Ready()
    {
	    ResetNetwork();
	    
        GetTree().Connect(Constants.ENet.NetworkPeerDisconnectedEvent, new Callable(this, nameof(OnPlayerDisconnected)));
    }

    /// <summary>
    /// Completely reset the game state and clear the network.
    /// </summary>
    public void ResetNetwork()
    {
	    var peer = GetTree().NetworkPeer as ENetMultiplayerPeer;
	    peer?.CloseConnection();
    }

    protected void SetPeerTimeout()
    {
	    var peer = GetTree().NetworkPeer as ENetMultiplayerPeer;
	    peer?.SetPeerTimeout(GetTree().GetUniqueId(), Constants.TimeoutLimitMs, 
		    Constants.TimeoutMinimumMs, Constants.TimeoutMaximumMs);
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
