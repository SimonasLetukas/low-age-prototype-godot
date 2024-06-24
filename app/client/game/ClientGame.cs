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
        
        _interface.MouseEntered += _mouse.OnInterfaceMouseEntered;
        _interface.MouseExited += _mouse.OnInterfaceMouseExited;
        _interface.SelectedToBuild += _map.OnSelectedToBuild;

        _map.FinishedInitializing += OnMapFinishedInitializing;
        _map.EntityIsBeingPlaced += _interface.OnEntityIsBeingPlaced;
        _map.Entities.EntitySelected += _interface.OnEntitySelected;
        _map.Entities.EntityDeselected += _interface.OnEntityDeselected;
        
        _map.UnitMovementIssued += RegisterNewGameEvent;
        _map.Entities.EntityPlaced += RegisterNewGameEvent;
    }

    private void DisconnectSignals()
    {
        _interface.MouseEntered -= _mouse.OnInterfaceMouseEntered;
        _interface.MouseExited -= _mouse.OnInterfaceMouseExited;
        _interface.SelectedToBuild -= _map.OnSelectedToBuild;
        
        _map.FinishedInitializing -= OnMapFinishedInitializing;
        _map.EntityIsBeingPlaced -= _interface.OnEntityIsBeingPlaced;
        _map.Entities.EntitySelected -= _interface.OnEntitySelected;
        _map.Entities.EntityDeselected -= _interface.OnEntityDeselected;
        
        _map.UnitMovementIssued -= RegisterNewGameEvent;
        _map.Entities.EntityPlaced -= RegisterNewGameEvent;
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
            case InitializationCompletedEvent initializationCompletedEvent:
                OnEveryoneFinishedInitializing();
                break;
            case UnitMovedAlongPathEvent unitMovedAlongPathEvent:
                _map.MoveUnit(unitMovedAlongPathEvent);
                break;
            case EntityPlacedEvent entityPlacedEvent:
                _map.Entities.PlaceEntity(entityPlacedEvent);
                _map.OnEntityPlaced();
                break;
            default:
                GD.PrintErr($"{nameof(ClientGame)}.{nameof(ExecuteGameEvent)}: could not execute event " +
                            $"'{EventToString(gameEvent)}'. Type not implemented or not relevant for client.");
                break;
        }
    }

    private void OnMapFinishedInitializing()
    {
        GD.Print($"{nameof(ClientGame)}.{nameof(OnMapFinishedInitializing)}");
        RegisterNewGameEvent(new ClientFinishedInitializingEvent(GetTree().GetNetworkUniqueId()));
    }

    private void OnEveryoneFinishedInitializing()
    {
        _map.SetupFactionStart();
        GetTree().Paused = false;
    }
}
