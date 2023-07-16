using Godot;

public class Client : Network
{
    public static Client Instance = null;
    
    [Signal] public delegate void PlayerAdded(int playerId);
    [Signal] public delegate void GameStarted();
    
    public string LocalPlayerName { get; private set; }
    public int LocalPlayerFaction { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
    }
    
    public bool JoinGame(string playerName, int playerFaction) // TODO not tested
    {
        LocalPlayerName = playerName;
        LocalPlayerFaction = playerFaction;
        
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

    private void OnConnectedToServer() // TODO not tested
    {
        GD.Print("Connected to server.");
    }

    public void RegisterPlayer(int recipientId, int playerId, string playerName, int playerFaction) // TODO not tested
    {
        RpcId(recipientId, nameof(OnRegisterPlayer), playerId, playerName, playerFaction);
    }

    [Remote]
    public void OnRegisterPlayer(int playerId, string playerName, int playerFaction) // TODO not tested
    {
        GD.Print($"{nameof(OnRegisterPlayer)}: {playerId}, {playerName}, {playerFaction}");
        Data.Instance.AddPlayer(playerId, playerName, (Constants.Game.Faction) playerFaction);
        
        EmitSignal(nameof(PlayerAdded), playerId);
        GD.Print($"Total players: {Data.Instance.Players.Count}");
    }

    public void StartGame() // TODO not tested
    {
        GD.Print($"{nameof(Client)}: {nameof(StartGame)} called for {LocalPlayerName}.");
        Rpc(nameof(OnStartGame));
    }

    [RemoteSync]
    public void OnStartGame() // TODO not tested
    {
        GD.Print($"{nameof(Client)}: {nameof(OnStartGame)} called for {LocalPlayerName}.");
        EmitSignal(nameof(GameStarted));
    }
}