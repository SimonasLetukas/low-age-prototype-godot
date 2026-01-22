using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using LowAgeCommon.Extensions;

public partial class ServerGame : Game
{
    public const string ScenePath = @"res://app/server/game/ServerGame.tscn";

    private readonly List<int> _notLoadedPlayers = [];
    private readonly List<int> _notInitializedPlayers = [];
    private readonly List<EntityPlacedRequestEvent> _pendingEntityPlacedRequests = [];
    private readonly List<PlanningPhaseEndedRequestEvent> _planningPhaseEndedEventsByTurn = [];
    private int _entityCreationTokenCounter;
    private readonly Dictionary<Guid, List<int>> _pendingPlayersByAbilityExecutions = [];
    private List<int> _playersPendingPlanningPhaseEndResolution = [];
    private int _turn;
    
    private Creator _creator = null!;
    
    public override async void _Ready()
    {
        GD.Print($"{nameof(ServerGame)}: entering");
        
        base._Ready();
        
        _creator = GetNode<Creator>($"{nameof(Creator)}");
        
        _creator.MapCreated += OnRegisterServerEvent;
        Server.Instance.PlayerRemoved += OnPlayerRemoved;

        // Wait until the parent scene is fully loaded
        await ToSignal(GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1), "ready");
        
        foreach (var playerId in Players.Instance.GetAllIds())
        {
            _notLoadedPlayers.Add(playerId);
            _notInitializedPlayers.Add(playerId);
        }
    }

    public override void _ExitTree()
    {
        _creator.MapCreated -= OnRegisterServerEvent;
        Server.Instance.PlayerRemoved -= OnPlayerRemoved;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected override void OnClientLoaded(int playerId)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnClientLoaded)}: '{playerId}' client loaded");
        _notLoadedPlayers.Remove(playerId);
        
        if (_notLoadedPlayers.IsEmpty())
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(OnClientLoaded)}: all clients have loaded, " +
                     "starting map generation");
            _creator.Generate();
            return;
        }
        
        GD.Print($"{nameof(ServerGame)}.{nameof(OnClientLoaded)}: still waiting for " +
                 $"{_notLoadedPlayers.Count} players to load");
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected override void OnRegisterNewGameEvent(string eventBody)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnRegisterNewGameEvent)}: registering new game event " +
                 $"'{eventBody.TrimForLogs()}'");

        var gameEvent = StringToEvent(eventBody);
        if (Events.Any(x => x.Id.Equals(gameEvent.Id)))
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(OnRegisterNewGameEvent)}: event '{gameEvent.Id}' " +
                     "already exists for server");
            return;
        }
        
        Events.Add(gameEvent);

        Rpc(nameof(OnNewGameEventRegistered), eventBody);
        
        ExecuteGameEvent(gameEvent);
    }

    private void OnRegisterServerEvent(IGameEvent gameEvent)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnRegisterServerEvent)}: called with {gameEvent.GetType()}");
        OnRegisterNewGameEvent(EventToString(gameEvent));
    }
    
    private void ExecuteGameEvent(IGameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case ClientFinishedInitializingEvent clientFinishedInitializingEvent:
                HandleEvent(clientFinishedInitializingEvent);
                break;
            case EntityPlacedRequestEvent entityPlacedRequestEvent:
                HandleEvent(entityPlacedRequestEvent);
                break;
            case EntityCandidatePlacementCancelledEvent candidatePlacementCancelledEvent:
                HandleEvent(candidatePlacementCancelledEvent);
                break;
            case PlanningPhaseEndedRequestEvent planningPhaseEndedRequestEvent:
                HandleEvent(planningPhaseEndedRequestEvent);
                break;
            case AbilityExecutionRequestedEvent abilityExecutionRequestedEvent:
                HandleEvent(abilityExecutionRequestedEvent);
                break;
            case AbilityExecutionCompletedEvent abilityExecutionCompletedEvent:
                HandleEvent(abilityExecutionCompletedEvent);
                break;
            case PlanningPhaseEndResolvedEvent planningPhaseEndResolvedEvent:
                HandleEvent(planningPhaseEndResolvedEvent);
                break;
            default:
                GD.Print($"{nameof(ServerGame)}.{nameof(ExecuteGameEvent)}: could not execute event " +
                         $"'{EventToString(gameEvent).TrimForLogs(50)}...'. Type not implemented or not relevant " +
                         $"for server.");
                break;
        }
    }

    private void HandleEvent(ClientFinishedInitializingEvent clientFinishedInitializingEvent)
    {
        var playerId = clientFinishedInitializingEvent.PlayerId;
        GD.Print($"{nameof(ServerGame)}.{nameof(ClientFinishedInitializingEvent)}: '{playerId}' client initialized");
        _notInitializedPlayers.Remove(playerId);
        
        if (_notInitializedPlayers.IsEmpty())
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(ClientFinishedInitializingEvent)}: all clients have loaded, " +
                     "notifying all players");
            
            var @event = new InitializationCompletedEvent
            {
                RandomSeed = SharedRandom.Seed,
            };
            OnRegisterServerEvent(@event);
            
            return;
        }
        
        GD.Print($"{nameof(ServerGame)}.{nameof(ClientFinishedInitializingEvent)}: still waiting for " +
                 $"{_notInitializedPlayers.Count} players to initialize");
    }

    private void HandleEvent(EntityPlacedRequestEvent entityPlacedRequestEvent)
    {
        _pendingEntityPlacedRequests.Add(entityPlacedRequestEvent);
    }

    private void HandleEvent(EntityCandidatePlacementCancelledEvent @event)
    {
        var placedRequest = _pendingEntityPlacedRequests.FirstOrDefault(r =>
            r.InstanceId.Equals(@event.InstanceId) && r.PlayerId.Equals(@event.PlayerId));

        if (placedRequest is null)
            return;

        _pendingEntityPlacedRequests.Remove(placedRequest);
    }

    private void HandleEvent(PlanningPhaseEndedRequestEvent @event)
    {
        var existingRequestFromThisPlayer = _planningPhaseEndedEventsByTurn.FirstOrDefault(x => 
            x.PlayerId.Equals(@event.PlayerId));
        if (existingRequestFromThisPlayer != null)
            _planningPhaseEndedEventsByTurn.Remove(existingRequestFromThisPlayer);
        
        _planningPhaseEndedEventsByTurn.Add(@event);
        
        if (_planningPhaseEndedEventsByTurn.Count != Players.Instance.Count)
            return;

        var cancelledCandidates = GetCancelledCandidates(_planningPhaseEndedEventsByTurn).ToHashSet();

        foreach (var entityPlacedResponse in _pendingEntityPlacedRequests
                     .Where(request => cancelledCandidates.Contains(request.InstanceId) is false)
                     .Select(request => EntityPlacedResponseEvent.From(request, ++_entityCreationTokenCounter)))
        {
            OnRegisterServerEvent(entityPlacedResponse);
        }

        _pendingEntityPlacedRequests.Clear();
        _planningPhaseEndedEventsByTurn.Clear();

        _pendingPlayersByAbilityExecutions.Clear();
        _playersPendingPlanningPhaseEndResolution = Players.Instance.GetAllIds().ToList();
        
        _turn = @event.Turn;
        
        var planningPhaseEndedResponse = new PlanningPhaseEndedResponseEvent
        {
            CancelledCandidateEntities = cancelledCandidates.ToList()
        };
        OnRegisterServerEvent(planningPhaseEndedResponse);
    }

    private void HandleEvent(AbilityExecutionRequestedEvent @event)
    {
        _pendingPlayersByAbilityExecutions[@event.Id] = Players.Instance.GetAllIds().ToList();
    }
    
    private void HandleEvent(AbilityExecutionCompletedEvent @event)
    {
        _pendingPlayersByAbilityExecutions[@event.AbilityExecutionRequestedEventId].Remove(@event.PlayerId);

        StartActionPhase();
    }
    
    private void HandleEvent(PlanningPhaseEndResolvedEvent @event)
    {
        _playersPendingPlanningPhaseEndResolution.Remove(@event.PlayerId);

        StartActionPhase();
    }

    private void StartActionPhase()
    {
        if (_playersPendingPlanningPhaseEndResolution.Count > 0
            || AllPlayersCompletedAbilityExecution() is false)
            return;
        
        var actionPhaseStartedEvent = new ActionPhaseStartedEvent
        {
            Turn = _turn
        };
        OnRegisterServerEvent(actionPhaseStartedEvent);
    }

    private bool AllPlayersCompletedAbilityExecution() => _pendingPlayersByAbilityExecutions.Values
        .All(pendingPlayers => pendingPlayers.Count == 0);

    private static IEnumerable<Guid> GetCancelledCandidates(
        IList<PlanningPhaseEndedRequestEvent> planningPhaseEndedEvents)
    {
        var checkedEventPairs = new HashSet<(Guid, Guid)>();
        
        foreach (var planningPhaseEndedA in planningPhaseEndedEvents)
        {
            foreach (var planningPhaseEndedB in planningPhaseEndedEvents)
            {
                if (planningPhaseEndedA.Id.Equals(planningPhaseEndedB.Id))
                    continue;
                
                var pair = planningPhaseEndedA.Id.CompareTo(planningPhaseEndedB.Id) < 0 
                    ? (planningPhaseEndedA.Id, planningPhaseEndedB.Id) 
                    : (planningPhaseEndedB.Id, planningPhaseEndedA.Id);
                if (checkedEventPairs.Contains(pair))
                    continue;

                foreach (var candidateEntityA in planningPhaseEndedA.CandidateEntities)
                {
                    foreach (var candidateEntityB in planningPhaseEndedB.CandidateEntities)
                    {
                        if (candidateEntityA.OccupyingPositions.Any(candidateEntityB.OccupyingPositions.Contains))
                        {
                            yield return candidateEntityA.EntityId;
                            yield return candidateEntityB.EntityId;
                        }
                    }
                }
                
                checkedEventPairs.Add(pair);
            }
        }
    }
    
    private void OnPlayerRemoved(long playerId)
    {
        if (Players.Instance.Count < 2)
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(OnPlayerRemoved)}: not enough players to run the " +
                     "game, returning to lobby");
            
            // Tell everyone that the game has ended
            Rpc(nameof(GameEnded));
            
            Server.Instance.ResetNetwork();
            Players.Instance.Reset();
            
            Callable.From(() => GetTree().ChangeSceneToFile(ServerLobby.ScenePath)).CallDeferred();
        }
        
        GD.Print($"{nameof(ServerGame)}.{nameof(OnPlayerRemoved)}: '{Players.Instance.Count}' players remaining");
    }
}
