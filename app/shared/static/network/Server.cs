using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Godot;
using LowAgeData.Domain.Factions;

public partial class Server : Network
{
    public static Server Instance = null!;
    
    private Process? _serverProcess;
    
    public override void _Ready()
    {
        base._Ready();

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            ShutdownServer();
        }
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
        
        // Server has the authority to set the initial stable ID
        var playerStableId = Players.Instance.GetNextAvailableStableId();
        
        // Register this client with the server
        Client.Instance.OnRegisterPlayer(playerId, playerStableId, playerName, playerReady, playerFactionId, 
            playerTeam.Value);

        // Register the new player with all existing clients
        foreach (var currentPlayerId in Players.Instance.GetAllIds())
        {
            Client.Instance.RegisterPlayer(currentPlayerId, playerId, playerStableId, playerName, playerReady, 
                new FactionId(playerFactionId), playerTeam);
        }

        // Catch the new player up with who is already here
        foreach (var currentPlayer in Players.Instance.GetAll())
        {
            if (currentPlayer.Id != playerId)
            {
                Client.Instance.RegisterPlayer(playerId, currentPlayer.Id, currentPlayer.StableId, currentPlayer.Name, 
                    currentPlayer.Ready, currentPlayer.Faction, currentPlayer.Team);
            }
        }
        
        // Catch the new player up with the added save game
        if (Data.Instance.Save is not null)
        {
            Client.Instance.SyncSaveGame(playerId);
        }
    }

    public void RunLocalServerInstance()
    {
        if (_serverProcess is { HasExited: false })
        {
            GD.Print("Server already running.");
            return;
        }

        var executablePath = GetLocalServerExecutablePath();

        _serverProcess = new Process();
        _serverProcess.StartInfo.FileName = executablePath;
        _serverProcess.StartInfo.UseShellExecute = false;
        _serverProcess.StartInfo.CreateNoWindow = true;

        _serverProcess.Start();
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

        var peer = new ENetMultiplayerPeer();
        var result = peer.CreateServer(Constants.ENet.ServerPort, Constants.ENet.MaxPlayers);
        if (result != Error.Ok)
        {
            GD.Print($"Failed to host game: {result}");
            return false;
        }

        Multiplayer.MultiplayerPeer = peer;
        
        var onPlayerConnected = new Callable(this, MethodName.OnPlayerConnected);
        if (Multiplayer.IsConnected(MultiplayerApi.SignalName.PeerConnected, onPlayerConnected) is false)
        {
            // Check is needed so that server restarts do not crash when trying to connect already connected signal.
            Multiplayer.Connect(MultiplayerApi.SignalName.PeerConnected, onPlayerConnected);
        }
        
        GD.Print("Server started.");
        return true;
    }
    
    private void ShutdownServer()
    {
        if (_serverProcess is not { HasExited: false }) 
            return;
        
        _serverProcess.Kill(true);
        _serverProcess.Dispose();
        GD.Print("Server stopped.");
    }
    
    private static string GetLocalServerExecutablePath()
    {
        if (OS.HasFeature(nameof(Client).ToLower()) is false)
        {
            // Running from editor (Rider / Godot editor), otherwise there would be "client" feature at this point.
            return ProjectSettings.GlobalizePath("res://server.exe");
        }
        
        var basePath = OS.GetExecutablePath().GetBaseDir();

        if (OS.GetName() == "Windows")
            return basePath + "/server.exe";
        if (OS.GetName() == "macOS")
            return basePath + "/server";
        
        // Linux
        return basePath + "/server";
    }

    private void OnPlayerConnected(long playerId)
    {
        GD.Print($"Player connected: {playerId}");
    }
}