using Godot;

public class Client : Network
{
    public static Client Instance = null;
    
    [Signal] public delegate void PlayerAdded(int playerId);
    [Signal] public delegate void GameStarted();

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
        Data.Players.Add(new Player
        {
            Id = playerId,
            Name = playerName,
            Faction = playerFaction
        });
        
        EmitSignal(nameof(PlayerAdded), playerId);
        GD.Print($"Total players: {Data.Players.Count}");
    }

    public void StartGame() // TODO not tested
    {
        Rpc(nameof(OnStartGame));
    }

    [RemoteSync]
    public void OnStartGame() // TODO not tested
    {
        EmitSignal(nameof(GameStarted));
    }
}