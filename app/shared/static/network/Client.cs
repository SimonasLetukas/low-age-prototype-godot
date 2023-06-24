using Godot;

public class Client : Network
{
    public static Client Instance = null;

    public override void _Ready()
    {
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

        if (result is Error.Ok)
        {
            GetTree().NetworkPeer = peer;
            GD.Print("Connecting to server...");
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnConnectedToServer()
    {
        GD.Print("Connected to server.");
    }
    
    // TODO rest of methods
}