using Godot;
using low_age_data.Domain.Factions;

public class Server : Network
{
    public static Server Instance = null;

    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Called by clients when they connect
    /// </summary>
    public void RegisterSelf(int playerId, string playerName, FactionId playerFaction)
    {
        RpcId(Constants.ServerId, nameof(OnRegisterSelf), playerId, playerName, playerFaction.ToString());
    }

    [Remote]
    public void OnRegisterSelf(int playerId, string playerName, string playerFactionId)
    {
        // Register this client with the server
        Client.Instance.OnRegisterPlayer(playerId, playerName, playerFactionId);

        // Register the new player with all existing clients
        foreach (var currentPlayer in Data.Instance.Players)
        {
            Client.Instance.RegisterPlayer(currentPlayer.Id, playerId, playerName, new FactionId(playerFactionId));
        }

        // Catch the new player up with who is already here
        foreach (var currentPlayer in Data.Instance.Players)
        {
            if (currentPlayer.Id != playerId)
            {
                Client.Instance.RegisterPlayer(playerId, currentPlayer.Id, currentPlayer.Name, currentPlayer.Faction);
            }
        }
    }

    public void RunLocalServerInstance()
    {
        // TODO seems that the export is not working from game itself, should probably find another way to do this,
        // see useful links in: https://trello.com/c/VKRGNOYZ/69-make-it-easier-to-test-the-changes-locally
        // OS.Execute("godot", new []{ "--export", "\"Windows Desktop - Server\"", "server.exe" });
        OS.Execute("cmd", new []
        {
            "/c", "tasklist /fi \"imagename eq server.exe\" | findstr /B /I /C:\"server.exe \" >NUL " +
                  "& IF ERRORLEVEL 1 start /min \"server\" server.exe"
        }, false);
    }

    public bool IsHosting()
    {
        var networkPeer = GetTree().NetworkPeer;
        if (networkPeer is null) 
            return false;

        var connectionStatus = networkPeer.GetConnectionStatus();
        if (connectionStatus is NetworkedMultiplayerPeer.ConnectionStatus.Disconnected) 
            return false;

        return true;
    }

    public bool HostGame() 
    {
        ResetNetwork();
        Data.Instance.Reset();

        var peer = new NetworkedMultiplayerENet();
        var result = peer.CreateServer(Constants.ServerPort, Constants.MaxPlayers);
        if (result != Error.Ok)
        {
            GD.Print($"Failed to host game: {result}");
            return false;
        }

        GetTree().NetworkPeer = peer;
        GetTree().Connect(Constants.ENet.NetworkPeerConnectedEvent, this, nameof(OnPlayerConnected));
        
        Data.Instance.ReadBlueprint();
        
        GD.Print("Server started.");
        return true;
    }

    private void OnPlayerConnected(int playerId)
    {
        GD.Print($"Player connected: {playerId}");
    }
}