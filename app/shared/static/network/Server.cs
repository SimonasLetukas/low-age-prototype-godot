using Godot;
using LowAgeData.Domain.Factions;

public partial class Server : Network
{
    public static Server Instance = null!;

    public override void _Ready()
    {
        base._Ready();

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
    }

    /// <summary>
    /// Called by clients when they connect
    /// </summary>
    public void RegisterSelf(int playerId, string playerName, bool playerReady, FactionId playerFaction)
    {
        RpcId(Constants.ENet.ServerId, nameof(OnRegisterSelf), playerId, playerName, playerReady, 
            playerFaction.ToString());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void OnRegisterSelf(int playerId, string playerName, bool playerReady, string playerFactionId)
    {
        // Server has the authority to determine the next available team
        var playerTeam = Players.Instance.GetNextAvailableTeam();
        
        // Register this client with the server
        Client.Instance.OnRegisterPlayer(playerId, playerName, playerReady, playerFactionId, playerTeam.Value);

        // Register the new player with all existing clients
        foreach (var currentPlayerId in Players.Instance.GetAllIds())
        {
            Client.Instance.RegisterPlayer(currentPlayerId, playerId, playerName, playerReady, 
                new FactionId(playerFactionId), playerTeam);
        }

        // Catch the new player up with who is already here
        foreach (var currentPlayer in Players.Instance.GetAll())
        {
            if (currentPlayer.Id != playerId)
            {
                Client.Instance.RegisterPlayer(playerId, currentPlayer.Id, currentPlayer.Name, 
                    currentPlayer.Ready, currentPlayer.Faction, currentPlayer.Team);
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
        var result = peer.CreateServer(Constants.ENet.ServerPort, Constants.ENet.MaxPlayers);
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