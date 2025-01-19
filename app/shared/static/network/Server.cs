using Godot;
using low_age_data.Domain.Factions;

public partial class Server : Network
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
    public void RegisterSelf(int playerId, string playerName, bool playerReady, FactionId playerFaction)
    {
        RpcId(Constants.ServerId, nameof(OnRegisterSelf), playerId, playerName, playerReady, 
            playerFaction.ToString());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void OnRegisterSelf(int playerId, string playerName, bool playerReady, string playerFactionId)
    {
        // Register this client with the server
        Client.Instance.OnRegisterPlayer(playerId, playerName, playerReady, playerFactionId);

        // Register the new player with all existing clients
        foreach (var currentPlayer in Data.Instance.Players)
        {
            Client.Instance.RegisterPlayer(currentPlayer.Id, playerId, playerName, playerReady, 
                new FactionId(playerFactionId));
        }

        // Catch the new player up with who is already here
        foreach (var currentPlayer in Data.Instance.Players)
        {
            if (currentPlayer.Id != playerId)
            {
                Client.Instance.RegisterPlayer(playerId, currentPlayer.Id, currentPlayer.Name, 
                    currentPlayer.Ready, currentPlayer.Faction);
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
        });
    }

    public bool IsHosting()
    {
        var networkPeer = Multiplayer.MultiplayerPeer;
        if (networkPeer is null) 
            return false;

        var connectionStatus = networkPeer.GetConnectionStatus();
        if (connectionStatus is MultiplayerPeer.ConnectionStatus.Disconnected) 
            return false;

        return true;
    }

    public bool HostGame() 
    {
        ResetNetwork();
        Data.Instance.Reset();

        var peer = new ENetMultiplayerPeer();
        var result = peer.CreateServer(Constants.ServerPort, Constants.MaxPlayers);
        if (result != Error.Ok)
        {
            GD.Print($"Failed to host game: {result}");
            return false;
        }

        Multiplayer.MultiplayerPeer = peer;
        Multiplayer.PeerConnected += OnPlayerConnected;
        
        Data.Instance.ReadBlueprint();
        
        GD.Print("Server started.");
        return true;
    }

    private void OnPlayerConnected(long playerId)
    {
        GD.Print($"Player connected: {playerId}");
    }
}