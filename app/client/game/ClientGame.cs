using System;
using System.IO;
using Godot;
using System.Linq;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

public partial class ClientGame : Game
{
    public const string ScenePath = @"res://app/client/game/ClientGame.tscn";
    private const string SaveLocation = "user://saves";
    
    private ClientMap _map = null!;
    private Camera _camera = null!;
    private Mouse _mouse = null!;
    private Interface _interface = null!;
    
    public override async void _Ready()
    {
        Log.Info(nameof(ClientGame), nameof(_Ready), "Entering");
        
        base._Ready();
        
        _map = GetNode<ClientMap>($"{nameof(Map)}");
        _camera = GetNode<Camera>($"{nameof(Camera)}");
        _mouse = GetNode<Mouse>($"{nameof(Mouse)}");
        _interface = GetNode<Interface>($"{nameof(Interface)}");
        
        SetPaused(true);
        
        // Wait until the parent scene is fully loaded
        await ToSignal(GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1), "ready");

        ConnectSignals();

        var save = Data.Instance.Save;
        if (save is not null)
            LoadGame(save);
        
        MarkAsLoaded();
    }

    public override void _ExitTree()
    {
        DisconnectSignals();
        base._ExitTree();
    }

    private void ConnectSignals()
    {
        Log.Info(nameof(ClientGame), nameof(ConnectSignals), "Connecting signals.");

        SaveLoadingFinished += OnClientSaveLoadingFinished;
        
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
        _interface.CandidatePlacementCancelled += _map.Entities.OnCandidatePlacementCancelled;
        
        _map.FinishedInitializing += OnMapFinishedInitializing;
        _map.EntityIsBeingPlaced += _interface.OnEntityIsBeingPlaced;
        _map.Entities.EntitySelected += _interface.OnEntitySelected;
        _map.Entities.EntityDeselected += _interface.OnEntityDeselected;

        EventBus.Instance.PhaseStarted += OnPhaseStarted;
        
        _map.UnitMovementIssued += RegisterNewGameEvent;
        _map.EntityAttacked += RegisterNewGameEvent;
        _map.Entities.EntityPlaced += RegisterNewGameEvent;
        _map.Entities.CandidatePlacementCancelled += RegisterNewGameEvent;
        _map.Entities.AbilityExecutionRequested += RegisterNewGameEvent;
        _map.Entities.AbilityExecutionCompleted += RegisterNewGameEvent;
        
        Turns.PlanningPhaseEnded += RegisterNewGameEvent;
        Turns.PlanningPhaseEndResolved += RegisterNewGameEvent;
        Turns.ActionEnded += RegisterNewGameEvent;
        Turns.ActionEndResolved += RegisterNewGameEvent;
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
        _interface.CandidatePlacementCancelled -= _map.Entities.OnCandidatePlacementCancelled;
        
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
        Turns.ActionEndResolved -= RegisterNewGameEvent;
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    protected override void OnSaveGameLoadedByAllPeers()
    {
        Log.Info(nameof(ClientGame), nameof(OnSaveGameLoadedByAllPeers), string.Empty);
        SetPaused(false);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void GameEnded()
    {
        Log.Info(nameof(ClientGame), nameof(GameEnded), "Returning to main menu.");
        Client.Instance.ResetNetwork();
        Callable.From(() => GetTree().ChangeSceneToFile(MainMenu.ScenePath)).CallDeferred();
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected override void OnNewGameEventRegistered(string eventBody)
    {
        var player = Players.Instance.Current;
        
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(ClientGame), nameof(OnNewGameEventRegistered), 
                $"Event '{eventBody.TrimForLogs()}' received for player {player.StableId} '{player.Name}'");

        var gameEvent = StringToEvent(eventBody);
        if (Events.Any(x => x.Id.Equals(gameEvent.Id)))
        {
            Log.Info(nameof(ClientGame), nameof(OnNewGameEventRegistered), 
                $"Event '{gameEvent.Id}' already exists for player {player.StableId} '{player.Name}'");
            return;
        }

        Events.Add(gameEvent);
        ExecuteGameEvent(gameEvent);
    }

    protected override void ExecuteGameEvent(IGameEvent gameEvent)
    {
        Log.Info(nameof(ClientGame), nameof(ExecuteGameEvent), 
            $"Executing event '{EventToString(gameEvent)}'.");
        
        switch (gameEvent)
        {
            case MapCreatedEvent mapCreatedEvent:
                HandleEvent(mapCreatedEvent);
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
                _map.Entities.HandleCancelledEntities(planningPhaseEndedResponseEvent.CancelledCandidateEntities);
                Turns.HandleEvent(planningPhaseEndedResponseEvent);
                break;
            case ActionPhaseStartedEvent actionPhaseStartedEvent:
                Turns.HandleEvent(actionPhaseStartedEvent);
                break;
            case ActionEndedRequestEvent actionEndedRequestEvent:
                Turns.HandleEvent(actionEndedRequestEvent);
                break;
            case ActionEndedResponseEvent actionEndedResponseEvent:
                Turns.HandleEvent(actionEndedResponseEvent);
                break;
            default:
                if (Log.VerboseDebugEnabled)
                    Log.Info(nameof(ClientGame), nameof(ExecuteGameEvent), 
                        $"Could not execute event '{gameEvent.GetType()}'. Type not implemented or " +
                        $"not relevant for client.");
                return;
        }
        
        Log.Info(nameof(ClientGame), nameof(ExecuteGameEvent), 
            $"Executed event '{gameEvent.GetType()}'.\n");
    }

    private void HandleEvent(MapCreatedEvent mapCreatedEvent)
    {
        MapLocation = mapCreatedEvent.MapLocation;
        GlobalRegistry.Instance.ProvideMapSize(mapCreatedEvent.MapSize);
        _camera.SetMapSize(mapCreatedEvent.MapSize.ToGodotVector2());
        _interface.SetMapSize(mapCreatedEvent.MapSize.ToGodotVector2());
        _map.Initialize(mapCreatedEvent);
        Turns.Initialize(_map.Entities.GetActorsSortedByInitiative, _map.Entities.GetCandidateEntities);
    }

    private void HandleEvent(InitializationCompletedEvent initializationCompletedEvent)
    {
        SharedRandom.Set(initializationCompletedEvent.RandomSeed);
        GameId = initializationCompletedEvent.GameId;
        GlobalRegistry.Instance.ProvideGameId(GameId);
        if (LoadingSavedGame is false)
            _map.SetupFactionStart();
        _interface.Visible = true;
        
        if (LoadingSavedGame is false)
            SetPaused(false); // Otherwise we wait until all peers loaded their saves
        
        Callable.From(() => Turns.OnNextTurnButtonClicked()).CallDeferred();
    }

    private void SaveGame()
    {
        if (LoadingSavedGame)
            return;

        if (DirAccess.Open(SaveLocation) is null)
        {
            var dir = DirAccess.Open("user://");
            dir.MakeDirRecursive(SaveLocation);
        }

        var save = new Save
        {
            GameId = GameId,
            MapLocation = MapLocation,
            SavedAtUtc = DateTimeOffset.UtcNow,
            Players = Players.Instance.GetAll().ToDictionary(p => p.StableId, p => new SavePlayer
            {
                FactionId = p.Faction,
                Team = p.Team.Value,
                OriginalName = p.Name
            }),
            Events = Events.Select(EventToString).ToList()
        };

        try
        {
            var json = JsonConvert.SerializeObject(save);
            using var file = FileAccess.Open($"{SaveLocation}/{GameId}.save", FileAccess.ModeFlags.WriteRead);
            file.Resize(0);
            file.StoreString(json);
            file.Close();
        }
        catch (IOException e)
        {
            Log.Error(nameof(ClientGame), nameof(SaveGame), 
                $"Received an exception while saving the game '{e.Message}'.");
        }
    }

    private void SetPaused(bool to)
    {
        _mouse.SetPaused(to);
        _map.SetPaused(to);
        return;
        GetTree().Paused = to; // Doesn't work during loading a saved game because events can't be iterated and executed
    }
    
    private void OnClientSaveLoadingFinished() => MarkAsLoaded();

    private void OnMapFinishedInitializing()
    {
        Log.Info(nameof(ClientGame), nameof(OnMapFinishedInitializing), string.Empty);
        RegisterNewGameEvent(new ClientFinishedInitializingEvent(Players.Instance.Current.StableId));
        
        // TODO save loading should wait for this event AGAIN during loading
    }
    
    private void OnPhaseStarted(int turn, TurnPhase phase)
    {
        if (phase.Equals(TurnPhase.Planning))
            SaveGame();
    }
}
