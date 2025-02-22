using System;
using Godot;
using LowAgeData.Domain.Factions;
using MultipurposePathfinding;

public partial class Client : Network
{
    public static Client Instance = null!;

    public event Action<int> PlayerAdded = delegate { };
    public event Action GameStarted = delegate { };
    
    public string LocalPlayerName { get; private set; } = null!;
    public FactionId LocalPlayerFaction { get; private set; } = null!;
    public bool LocalPlayerReady { get; private set; } = false;
    public bool QuickStartEnabled { get; set; } = false;

    public override void _Ready()
    {
        base._Ready();

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
    }
    
    public bool JoinGame(string playerName, FactionId playerFaction)
    {
        LocalPlayerName = playerName;
        LocalPlayerFaction = playerFaction;
        LocalPlayerReady = false;

        Multiplayer.ConnectedToServer += OnConnectedToServer;
        var peer = new ENetMultiplayerPeer();
        var result = peer.CreateClient(Constants.ENet.ServerIp, Constants.ENet.ServerPort);

        if (result != Error.Ok)
        {
            return false;
        }
        
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Connecting to server...");
        return true;
    }

    private void OnConnectedToServer()
    {
        GD.Print("Connected to server.");
        
        Data.Instance.ReadBlueprint();
    }

    public void RegisterPlayer(int recipientId, int playerId, string playerName, bool playerReady, 
        FactionId playerFaction, Team playerTeam)
    {
        RpcId(recipientId, nameof(OnRegisterPlayer), playerId, playerName, playerReady, 
            playerFaction.ToString(), playerTeam.Value);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void OnRegisterPlayer(int playerId, string playerName, bool playerReady, string playerFactionId, 
        int playerTeam)
    {
        GD.Print($"{nameof(OnRegisterPlayer)}: {playerId}, {playerName}, {playerReady}, {playerFactionId}");
        Players.Instance.Add(playerId, playerName, playerReady, new FactionId(playerFactionId), 
            new Team(playerTeam));
        
        PlayerAdded(playerId);
        GD.Print($"Total players: {Players.Instance.Count}");
    }

    public void StartGame()
    {
        GD.Print($"{nameof(Client)}: {nameof(StartGame)} called for {LocalPlayerName}.");
        Rpc(nameof(OnStartGame));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void OnStartGame()
    {
        GD.Print($"{nameof(Client)}: {nameof(OnStartGame)} called for {LocalPlayerName}.");
        GameStarted();
    }
}