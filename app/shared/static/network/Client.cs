using Godot;
using low_age_data.Domain.Factions;

public class Client : Network
{
    public static Client Instance = null;
    
    [Signal] public delegate void PlayerAdded(int playerId);
    [Signal] public delegate void GameStarted();
    
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
        
        GetTree().Connect(Constants.ENet.ConnectedToServerEvent, this, nameof(OnConnectedToServer));
        var peer = new NetworkedMultiplayerENet();
        var result = peer.CreateClient(Constants.ServerIp, Constants.ServerPort);

        if (result != Error.Ok)
        {
            return false;
        }
        
        GetTree().NetworkPeer = peer;
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

    [Remote]
    public void OnRegisterPlayer(int playerId, string playerName, bool playerReady, string playerFactionId)
    {
        GD.Print($"{nameof(OnRegisterPlayer)}: {playerId}, {playerName}, {playerReady}, {playerFactionId}");
        Data.Instance.AddPlayer(playerId, playerName, playerReady, new FactionId(playerFactionId));
        
        EmitSignal(nameof(PlayerAdded), playerId);
        GD.Print($"Total players: {Data.Instance.Players.Count}");
    }

    public void StartGame()
    {
        GD.Print($"{nameof(Client)}: {nameof(StartGame)} called for {LocalPlayerName}.");
        Rpc(nameof(OnStartGame));
    }

    [RemoteSync]
    public void OnStartGame()
    {
        GD.Print($"{nameof(Client)}: {nameof(OnStartGame)} called for {LocalPlayerName}.");
        EmitSignal(nameof(GameStarted));
    }
}