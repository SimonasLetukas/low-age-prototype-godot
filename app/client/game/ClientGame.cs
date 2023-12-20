using Godot;
using System.Linq;

public class ClientGame : Game
{
    public const string ScenePath = @"res://app/client/game/ClientGame.tscn";
    
    private ClientMap _map;
    private Camera _camera;
    private Mouse _mouse;
    private Interface _interface;
    
    public override async void _Ready()
    {
        base._Ready();
        
        _map = GetNode<ClientMap>($"{nameof(Map)}");
        _camera = GetNode<Camera>($"{nameof(Camera)}");
        _mouse = GetNode<Mouse>($"{nameof(Mouse)}");
        _interface = GetNode<Interface>($"{nameof(Interface)}");
        
        GD.Print($"{nameof(ClientGame)}: entering.");
        GetTree().Paused = true;
        
        // Wait until the parent scene is fully loaded
        await ToSignal(GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1), "ready");

        ConnectSignals();
        
        MarkAsLoaded();
    }

    public override void _ExitTree()
    {
        DisconnectSignals();
        base._ExitTree();
    }

    private void ConnectSignals()
    {
        GD.Print($"{nameof(ClientGame)}.{nameof(ConnectSignals)}: connecting signals.");
        
        _mouse.Connect(nameof(Mouse.LeftReleasedWithoutDrag), _map, nameof(ClientMap.OnMouseLeftReleasedWithoutDrag));
        _mouse.Connect(nameof(Mouse.RightReleasedWithoutExamine), _map, nameof(ClientMap.OnMouseRightReleasedWithoutExamine));

        _mouse.Connect(nameof(Mouse.MouseDragged), _camera, nameof(Camera.OnMouseDragged));
        _mouse.Connect(nameof(Mouse.TakingControl), _camera, nameof(Camera.OnMouseTakingControl));

        _interface.Connect(nameof(Interface.MouseEntered), _mouse, nameof(Mouse.OnInterfaceMouseEntered));
        _interface.Connect(nameof(Interface.MouseExited), _mouse, nameof(Mouse.OnInterfaceMouseExited));

        _map.FinishedInitializing += OnMapFinishedInitializing;
        _map.NewTileHovered += _interface.OnMapNewTileHovered;
        
        _map.UnitMovementIssued += RegisterNewGameEvent;
    }

    private void DisconnectSignals()
    {
        _map.FinishedInitializing -= OnMapFinishedInitializing;
        _map.NewTileHovered -= _interface.OnMapNewTileHovered;
        _map.UnitMovementIssued -= RegisterNewGameEvent;
    }

    [RemoteSync]
    protected override void GameEnded()
    {
        GD.Print($"{nameof(ClientGame)}.{nameof(GameEnded)}: returning to main menu.");
        Client.Instance.ResetNetwork();
        GetTree().ChangeScene(MainMenu.ScenePath);
    }
    
    [RemoteSync]
    protected override void OnNewGameEventRegistered(string eventBody)
    {
        var playerId = GetTree().GetNetworkUniqueId();
        GD.Print($"{nameof(ClientGame)}.{nameof(OnNewGameEventRegistered)}: event '{eventBody.TrimForLogs()}' " +
                 $"received for player {playerId} '{Data.Instance.GetPlayerName(playerId)}'");

        var gameEvent = StringToEvent(eventBody);
        if (Events.Any(x => x.Id.Equals(gameEvent.Id)))
        {
            GD.Print($"{nameof(ClientGame)}.{nameof(OnNewGameEventRegistered)}: event '{gameEvent.Id}' " +
                     $"already exists for player {playerId} '{Data.Instance.GetPlayerName(playerId)}'");
            return;
        }

        Events.Add(gameEvent);
        ExecuteGameEvent(gameEvent);
    }

    private void ExecuteGameEvent(IGameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case MapCreatedEvent mapCreatedEvent:
                _camera.SetMapSize(mapCreatedEvent.MapSize);
                _interface.SetMapSize(mapCreatedEvent.MapSize);
                _map.Initialize(mapCreatedEvent);
                break;
            case UnitMovedAlongPathEvent unitMovedAlongPathEvent:
                _map.MoveUnit(unitMovedAlongPathEvent);
                break;
            default:
                GD.PrintErr($"{nameof(ClientGame)}.{nameof(ExecuteGameEvent)}: could not execute event " +
                            $"'{EventToString(gameEvent)}'. Type not implemented.");
                break;
        }
    }

    private void OnMapFinishedInitializing()
    {
        GD.Print($"{nameof(ClientGame)}.{nameof(OnMapFinishedInitializing)}");
        GetTree().Paused = false;
    }
}
