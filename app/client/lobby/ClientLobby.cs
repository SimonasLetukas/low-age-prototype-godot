using System.Linq;
using Godot;

public partial class ClientLobby : Lobby
{
    public const string ScenePath = @"res://app/client/lobby/ClientLobby.tscn";

    private Button _startGameButton;
    
    public override void _Ready()
    {
        base._Ready();

        _startGameButton = GetNode<Button>("StartGame");
        _startGameButton.Connect(nameof(_startGameButton.Pressed).ToLower(), new Callable(this, nameof(OnStartGamePressed)));
        
        Client.Instance.Connect(nameof(Client.GameStarted), new Callable(this, nameof(OnGameStarted)));

        // Tell the server about you (client)
        Server.Instance.RegisterSelf(
            Multiplayer.GetUniqueId(), 
            Client.Instance.LocalPlayerName, 
            Client.Instance.LocalPlayerReady,
            Client.Instance.LocalPlayerFaction);

        if (Client.Instance.QuickStartEnabled)
        {
            Client.Instance.StartGame();
        }
    }

    protected override void OnPlayerAdded(int playerId)
    {
        base.OnPlayerAdded(playerId);
        GD.Print($"{nameof(ClientLobby)}.{nameof(OnPlayerAdded)}");

        _startGameButton.Disabled = IsStartGameButtonDisabled();
    }
    
    /// <summary>
    /// Callback from the server for each client to update their player ready status
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="newReadyStatus"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void ChangeReadyStatusForPlayer(int playerId, bool newReadyStatus)
    {
        base.ChangeReadyStatusForPlayer(playerId, newReadyStatus);
        GD.Print($"{nameof(ClientLobby)}.{nameof(ChangeReadyStatusForPlayer)}");

        _startGameButton.Disabled = IsStartGameButtonDisabled();
    }

    private void OnGameStarted()
    {
        GD.Print($"{nameof(ClientLobby)}: game starting for client...");
        Callable.From(() => GetTree().ChangeSceneToFile(ClientGame.ScenePath)).CallDeferred();
    }

    private void OnStartGamePressed()
    {
        GD.Print($"{nameof(ClientLobby)}: start game button pressed.");
        Client.Instance.StartGame();
    }

    private static bool IsStartGameButtonDisabled() => Data.Instance.AllPlayersReady is false;
}
