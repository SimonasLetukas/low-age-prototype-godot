using System;
using Godot;
using LowAgeData.Domain.Factions;
using MultipurposePathfinding;
using Newtonsoft.Json;

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

        Data.Instance.Reset();
        
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

    public void RegisterPlayer(int recipientId, int playerId, int playerStableId, string playerName, bool playerReady, 
        FactionId playerFaction, Team playerTeam)
    {
        RpcId(recipientId, nameof(OnRegisterPlayer), playerId, playerStableId, playerName, playerReady, 
            playerFaction.ToString(), playerTeam.Value);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void OnRegisterPlayer(int playerId, int playerStableId, string playerName, bool playerReady, 
        string playerFactionId, int playerTeam)
    {
        GD.Print($"{nameof(OnRegisterPlayer)}: {playerId}, {playerStableId}, {playerName}, {playerReady}, " +
                 $"{playerFactionId}");
        Players.Instance.Add(playerId, playerStableId, playerName, playerReady, new FactionId(playerFactionId), 
            new Team(playerTeam));
        
        PlayerAdded(playerId);
        GD.Print($"Total players: {Players.Instance.Count}");
    }

    public void SyncSaveGame(int recipientId)
    {
        var savePayload = JsonConvert.SerializeObject(Data.Instance.Save);
        RpcId(recipientId, nameof(OnSyncSaveGame), savePayload);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void OnSyncSaveGame(string savePayload)
    {
        GD.Print($"{nameof(OnSyncSaveGame)} received.");
        
        var save = JsonConvert.DeserializeObject<Save>(savePayload);
        if (save is null)
        {
            GD.PrintErr($"{nameof(OnSyncSaveGame)} could not deserialize received save game.");
            return;
        }
        
        Data.Instance.SetSave(save);
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