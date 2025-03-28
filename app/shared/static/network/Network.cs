using System;
using Godot;

public partial class Network : Node
{ 
	public event Action<long> PlayerRemoved = delegate { };

    public override void _Ready()
    {
	    ResetNetwork();

	    Multiplayer.PeerDisconnected += OnPlayerDisconnected;
    }

    /// <summary>
    /// Completely reset the game state and clear the network.
    /// </summary>
    public void ResetNetwork()
    {
	    var peer = Multiplayer.MultiplayerPeer as ENetMultiplayerPeer;
	    peer?.Close();
    }

    /// <summary>
    /// Every network peer needs to clean up the disconnected client.
    /// </summary>
    private void OnPlayerDisconnected(long playerIdL)
    {
	    var playerId = (int)playerIdL;
	    GD.Print("Player disconnected: " + playerId);
	    Players.Instance.Remove(playerId);
	    
	    PlayerRemoved(playerIdL);

	    GD.Print($"Total players: {Players.Instance.Count}");
    }
}
