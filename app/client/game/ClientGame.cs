using Godot;
using System.Linq;
using LowAgeCommon.Extensions;

public partial class ClientGame : Game
{
    public const string ScenePath = @"res://app/client/game/ClientGame.tscn";
    
    private ClientMap _map = null!;
    private Camera _camera = null!;
    private Mouse _mouse = null!;
    private Interface _interface = null!;
    
    public override async void _Ready()
    {
        GD.Print($"{LogPrefix}: entering.");
        
        base._Ready();
        
        _map = GetNode<ClientMap>($"{nameof(Map)}");
        _camera = GetNode<Camera>($"{nameof(Camera)}");
        _mouse = GetNode<Mouse>($"{nameof(Mouse)}");
        _interface = GetNode<Interface>($"{nameof(Interface)}");
        
        SetPaused(true);
        
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
        GD.Print($"{LogPrefix}.{nameof(ConnectSignals)}: connecting signals.");
        
        _mouse.Connect(nameof(Mouse.LeftReleasedWithoutDrag), new Callable(_map, nameof(ClientMap.OnMouseLeftReleasedWithoutDrag)));
        _mouse.Connect(nameof(Mouse.RightReleasedWithoutExamine), new Callable(_map, nameof(ClientMap.OnMouseRightReleasedWithoutExamine)));

        _mouse.Connect(nameof(Mouse.MouseDragged), new Callable(_camera, nameof(Camera.OnMouseDragged)));
        _mouse.Connect(nameof(Mouse.TakingControl), new Callable(_camera, nameof(Camera.OnMouseTakingControl)));
        
        _interface.MouseEntered += _mouse.OnInterfaceMouseEntered;
        _interface.MouseExited += _mouse.OnInterfaceMouseExited;
        _interface.SelectedToBuild += _map.OnInterfaceSelectedToBuild;
        _interface.AttackSelected += _map.OnInterfaceAttackSelected;
        _interface.NextTurnClicked += Turns.OnNextTurnButtonClicked;
        _interface.InitiativePanelActorHovered += _map.OnInitiativePanelActorHovered;
        _interface.InitiativePanelActorSelected += _map.OnInitiativePanelActorSelected;
        _interface.AbilitySelected += _map.OnInterfaceAbilitySelected;
        _interface.AbilityDeselected += _map.OnInterfaceAbilityDeselected;
        _interface.CandidatePlacementCancelled += _map.Entities.OnInterfaceCandidatePlacementCancelled;
        
        _map.FinishedInitializing += OnMapFinishedInitializing;
        _map.EntityIsBeingPlaced += _interface.OnEntityIsBeingPlaced;
        _map.Entities.EntitySelected += _interface.OnEntitySelected;
        _map.Entities.EntityDeselected += _interface.OnEntityDeselected;
        
        _map.UnitMovementIssued += RegisterNewGameEvent;
        _map.EntityAttacked += RegisterNewGameEvent;
        _map.Entities.EntityPlaced += RegisterNewGameEvent;
        _map.Entities.CandidatePlacementCancelled += RegisterNewGameEvent;
        _map.Entities.AbilityExecutionRequested += RegisterNewGameEvent;
        _map.Entities.AbilityExecutionCompleted += RegisterNewGameEvent;
        
        Turns.PlanningPhaseEnded += RegisterNewGameEvent;
        Turns.PlanningPhaseEndResolved += RegisterNewGameEvent;
        Turns.ActionEnded += RegisterNewGameEvent;
    }

    private void DisconnectSignals()
    {
        _interface.MouseEntered -= _mouse.OnInterfaceMouseEntered;
        _interface.MouseExited -= _mouse.OnInterfaceMouseExited;
        _interface.SelectedToBuild -= _map.OnInterfaceSelectedToBuild;
        _interface.AttackSelected -= _map.OnInterfaceAttackSelected;
        _interface.NextTurnClicked -= Turns.OnNextTurnButtonClicked;
        _interface.InitiativePanelActorHovered -= _map.OnInitiativePanelActorHovered;
        _interface.InitiativePanelActorSelected -= _map.OnInitiativePanelActorSelected;
        _interface.AbilitySelected -= _map.OnInterfaceAbilitySelected;
        _interface.AbilityDeselected -= _map.OnInterfaceAbilityDeselected;
        _interface.CandidatePlacementCancelled -= _map.Entities.OnInterfaceCandidatePlacementCancelled;
        
        _map.FinishedInitializing -= OnMapFinishedInitializing;
        _map.EntityIsBeingPlaced -= _interface.OnEntityIsBeingPlaced;
        _map.Entities.EntitySelected -= _interface.OnEntitySelected;
        _map.Entities.EntityDeselected -= _interface.OnEntityDeselected;
        
        _map.UnitMovementIssued -= RegisterNewGameEvent;
        _map.EntityAttacked -= RegisterNewGameEvent;
        _map.Entities.EntityPlaced -= RegisterNewGameEvent;
        _map.Entities.CandidatePlacementCancelled -= RegisterNewGameEvent;
        _map.Entities.AbilityExecutionRequested -= RegisterNewGameEvent;
        _map.Entities.AbilityExecutionCompleted -= RegisterNewGameEvent;

        Turns.PlanningPhaseEnded -= RegisterNewGameEvent;
        Turns.PlanningPhaseEndResolved -= RegisterNewGameEvent;
        Turns.ActionEnded -= RegisterNewGameEvent;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void GameEnded()
    {
        GD.Print($"{LogPrefix}.{nameof(GameEnded)}: returning to main menu.");
        Client.Instance.ResetNetwork();
        Callable.From(() => GetTree().ChangeSceneToFile(MainMenu.ScenePath)).CallDeferred();
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void OnNewGameEventRegistered(string eventBody)
    {
        var playerId = Multiplayer.GetUniqueId();
        GD.Print($"{LogPrefix}.{nameof(OnNewGameEventRegistered)}: event '{eventBody.TrimForLogs()}' " +
                 $"received for player {playerId} '{Players.Instance.GetName(playerId)}'");

        var gameEvent = StringToEvent(eventBody);
        if (Events.Any(x => x.Id.Equals(gameEvent.Id)))
        {
            GD.Print($"{LogPrefix}.{nameof(OnNewGameEventRegistered)}: event '{gameEvent.Id}' " +
                     $"already exists for player {playerId} '{Players.Instance.GetName(playerId)}'");
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
                GlobalRegistry.Instance.ProvideMapSize(mapCreatedEvent.MapSize);
                _camera.SetMapSize(mapCreatedEvent.MapSize.ToGodotVector2());
                _interface.SetMapSize(mapCreatedEvent.MapSize.ToGodotVector2());
                _map.Initialize(mapCreatedEvent);
                Turns.Initialize(_map.Entities.GetActorsSortedByInitiative, _map.Entities.GetCandidateEntities);
                break;
            case InitializationCompletedEvent initializationCompletedEvent:
                HandleEvent(initializationCompletedEvent);
                break;
            case UnitMovedAlongPathEvent unitMovedAlongPathEvent:
                _map.HandleEvent(unitMovedAlongPathEvent);
                break;
            case AbilityExecutionRequestedEvent abilityExecutionRequestedEvent:
                _map.Entities.HandleEvent(abilityExecutionRequestedEvent);
                break;
            case EntityAttackedEvent entityAttackedEvent:
                _map.Entities.HandleEvent(entityAttackedEvent);
                break;
            case EntityPlacedResponseEvent entityPlacedEvent:
                _map.Entities.HandleEvent(entityPlacedEvent);
                _map.OnEntityPlaced();
                break;
            case PlanningPhaseEndedResponseEvent planningPhaseEndedResponseEvent:
                _map.Entities.CancelCandidateEntities(planningPhaseEndedResponseEvent.CancelledCandidateEntities);
                Turns.HandleEvent(planningPhaseEndedResponseEvent);
                break;
            case ActionPhaseStartedEvent actionPhaseStartedEvent:
                Turns.HandleEvent(actionPhaseStartedEvent);
                break;
            case ActionEndedEvent actionEndedEvent:
                Turns.HandleEvent(actionEndedEvent);
                break;
            default:
                GD.Print($"{LogPrefix}.{nameof(ExecuteGameEvent)}: could not execute event " +
                         $"'{EventToString(gameEvent)}'. Type not implemented or not relevant for client.");
                break;
        }
    }
    
    private void HandleEvent(InitializationCompletedEvent @event)
    {
        SharedRandom.Set(@event.RandomSeed);
        _map.SetupFactionStart();
        _interface.Visible = true;
        SetPaused(false);
        Callable.From(() => Turns.OnNextTurnButtonClicked()).CallDeferred();
    }

    private void SetPaused(bool to)
    {
        GetTree().Paused = to;
        _map.SetPaused(to);
    }

    private void OnMapFinishedInitializing()
    {
        GD.Print($"{LogPrefix}.{nameof(OnMapFinishedInitializing)}");
        RegisterNewGameEvent(new ClientFinishedInitializingEvent(Multiplayer.GetUniqueId()));
    }
}
