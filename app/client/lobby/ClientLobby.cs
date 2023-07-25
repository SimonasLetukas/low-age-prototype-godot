using Godot;

public class ClientLobby : Lobby
{
    public const string ScenePath = @"res://app/client/lobby/ClientLobby.tscn";

    private Button _startGameButton;
    
    public override void _Ready()
    {
        base._Ready();

        _startGameButton = GetNode<Button>("StartGame");
        _startGameButton.Connect(nameof(_startGameButton.Pressed).ToLower(), this, nameof(OnStartGamePressed));
        
        Client.Instance.Connect(nameof(Client.GameStarted), this, nameof(OnGameStarted));

        // Tell the server about you (client)
        Server.Instance.RegisterSelf(
            GetTree().GetNetworkUniqueId(), 
            Client.Instance.LocalPlayerName, 
            Client.Instance.LocalPlayerFaction);
    }

    private void OnGameStarted()
    {
        GD.Print($"{nameof(ClientLobby)}: game starting for client...");
        GetTree().ChangeScene(ClientGame.ScenePath);
    }

    private void OnStartGamePressed()
    {
        GD.Print($"{nameof(ClientLobby)}: start game button pressed.");
        Client.Instance.StartGame();
    }
}
