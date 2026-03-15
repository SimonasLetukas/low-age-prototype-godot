using System;
using Godot;

public partial class Network : Node
{
	public static Network Instance = null!;
	
	public event Action<long> PlayerRemoved = delegate { };

    public override void _Ready()
    {
	    Callable.From(ResetNetwork).CallDeferred();

	    Multiplayer.PeerDisconnected += OnPlayerDisconnected;
	    
	    // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
	    Instance ??= this;
    }

    /// <summary>
    /// Completely reset the game state and clear the network.
    /// </summary>
    public void ResetNetwork()
    {
	    var peer = Multiplayer.MultiplayerPeer as ENetMultiplayerPeer;
	    peer?.Close();
	    
	    Players.Instance.Reset();
	    Data.Instance.Reset();
	    Data.Instance.ReadBlueprint();
    }

    public MultiplayerApi? TryGetMultiplayer()
    {
	    try
	    {
		    var multiplayer = GetMultiplayer();
		    return multiplayer;
	    }
	    catch (Exception)
	    {
		    return null;
	    }
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
