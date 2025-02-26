using Godot;
using LowAgeData.Domain.Factions;

public partial class Client : Network
{
    public static Client Instance = null;
    
    [Signal] public delegate void PlayerAddedEventHandler(int playerId);
    [Signal] public delegate void GameStartedEventHandler();
    
    public string LocalPlayerName { get; private set; }
    public FactionId LocalPlayerFaction { get; private set; }
    public bool LocalPlayerReady { get; private set; } = false;
    public bool QuickStartEnabled { get; set; } = false;

    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
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

    public void RegisterPlayer(int recipientId, int playerId, string playerName, bool playerReady, FactionId playerFaction)
    {
        RpcId(recipientId, nameof(OnRegisterPlayer), playerId, playerName, playerReady, 
            playerFaction.ToString());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void OnRegisterPlayer(int playerId, string playerName, bool playerReady, string playerFactionId)
    {
        GD.Print($"{nameof(OnRegisterPlayer)}: {playerId}, {playerName}, {playerReady}, {playerFactionId}");
        Data.Instance.AddPlayer(playerId, playerName, playerReady, new FactionId(playerFactionId));
        
        EmitSignal(SignalName.PlayerAdded, playerId);
        GD.Print($"Total players: {Data.Instance.Players.Count}");
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
        EmitSignal(nameof(GameStarted));
    }
}