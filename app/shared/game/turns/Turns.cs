using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using Newtonsoft.Json;

/// <summary>
/// Responsible for keeping track of turns and phases.
///
/// This class is not a singleton to ensure constrained communication flow (Game -> Turns -> EventBus -> Listener).
/// </summary>
public partial class Turns : Node2D
{
	public event Action<PlanningPhaseEndedRequestEvent> PlanningPhaseEnded = delegate { };
	public event Action<PlanningPhaseEndResolvedEvent> PlanningPhaseEndResolved = delegate { };
	public event Action<ActionEndedRequestEvent> ActionEnded = delegate { };
	public event Action<ActionEndResolvedEvent> ActionEndResolved = delegate { };

	private int Turn { get; set; }
	private TurnPhase Phase { get; set; } = TurnPhase.Planning;
	private IList<ActorNode> InitiativeQueue { get; set; } = [];

	private bool _waitingForServerResponse = false;
	private Func<IList<ActorNode>>? _getActorsSortedByInitiative;
	private Func<IList<EntityNode>>? _getCandidateEntities;

	public override void _Ready()
	{
		if (Log.DebugEnabled) 
			Log.Info(nameof(Turns), nameof(_Ready), "Entering");
		
		base._Ready();

		EventBus.Instance.EntityDestroyed += OnEntityDestroyed;
	}

	public override void _ExitTree()
	{
		EventBus.Instance.EntityDestroyed -= OnEntityDestroyed;
		
		base._ExitTree();
	}

	public void Initialize(Func<IList<ActorNode>> getActorsSortedByInitiative, 
		Func<IList<EntityNode>> getCandidateEntities)
	{
		_getActorsSortedByInitiative = getActorsSortedByInitiative;
		_getCandidateEntities = getCandidateEntities;
	}

	public void OnNextTurnButtonClicked()
	{
		if (_waitingForServerResponse)
			return;
		
		if (Phase.Equals(TurnPhase.Planning))
		{
			var planningPhaseEndedRequest = new PlanningPhaseEndedRequestEvent
			{
				Turn = Turn,
				PlayerStableId = Players.Instance.Current.StableId,
				CandidateEntities = _getCandidateEntities is null ? [] : _getCandidateEntities().Select(x => 
					new CandidateEntity
					{
						EntityId = x.InstanceId,
						OccupyingPositions = x.EntityOccupyingPositions.ToList(),
					}).ToList()
			};

			_waitingForServerResponse = true;
			PlanningPhaseEnded(planningPhaseEndedRequest);
			return;
		}

		if (Phase.Equals(TurnPhase.Action))
		{
			var actorInAction = InitiativeQueue.FirstOrDefault();
			if (actorInAction == null) 
				return;
			
			var actionEndedEvent = new ActionEndedRequestEvent
			{
				ActorInAction = actorInAction.InstanceId,
			};
				
			ActionEnded(actionEndedEvent);
		}
	}

	public void HandleEvent(PlanningPhaseEndedResponseEvent @event)
	{
		EventBus.Instance.RaisePhaseEnded(Turn, Phase);
		
		PlanningPhaseEndResolved(new PlanningPhaseEndResolvedEvent
		{
			PlayerStableId = Players.Instance.Current.StableId
		});
	}

	public void HandleEvent(ActionPhaseStartedEvent @event)
	{
		_waitingForServerResponse = false;
		Turn = @event.Turn;
		
		AdvanceToNextPhase(false);
		
		InitiativeQueue = _getActorsSortedByInitiative is null ? [] : _getActorsSortedByInitiative();
		
		if (Turn == 0) // Turn 0 is used for initialization only
		{
			EventBus.Instance.RaiseInitiativeQueueUpdated([]);
			AdvanceToNextPhase();
			return;
		}
		
		EventBus.Instance.RaiseInitiativeQueueUpdated(InitiativeQueue);
		
		if (InitiativeQueue.IsEmpty())
		{
			AdvanceToNextPhase();
			return;
		}
		
		AdvanceToNextAction();
	}

	public void HandleEvent(ActionEndedRequestEvent @event)
	{
		if (Log.DebugEnabled)
			Log.Info(nameof(Turns), $"{nameof(HandleEvent)}.{nameof(ActionEndedRequestEvent)}", 
				$"Initiative queue '{JsonConvert.SerializeObject(InitiativeQueue
					.Select(a => new {a.InstanceId, a.Initiative?.CurrentAmount}))}'");
		
		ResolveActionEnd();
	}

	public void HandleEvent(ActionEndedResponseEvent @event)
	{
		if (Log.DebugEnabled)
			Log.Info(nameof(Turns), $"{nameof(HandleEvent)}.{nameof(ActionEndedResponseEvent)}", 
				$"Initiative queue '{JsonConvert.SerializeObject(InitiativeQueue
					.Select(a => new {a.InstanceId, a.Initiative?.CurrentAmount}))}'");
		
		if (InitiativeQueue.IsEmpty())
		{
			EventBus.Instance.RaiseInitiativeQueueUpdated([]);
			AdvanceToNextPhase();
			return;
		}
		
		AdvanceToNextAction();
	}

	private void ResolveActionEnd()
	{
		EventBus.Instance.RaiseActionEnded(InitiativeQueue.First());
		
		InitiativeQueue.RemoveAt(0);
		EventBus.Instance.RaiseInitiativeQueueUpdated(InitiativeQueue);

		var actionEndResolvedEvent = new ActionEndResolvedEvent
		{
			PlayerStableId = Players.Instance.Current.StableId
		};
		ActionEndResolved(actionEndResolvedEvent);
	}

	private void AdvanceToNextAction()
	{
		// Local variables are needed to capture current value in closure, otherwise it'd do
		// `this.InitiativeQueue.First()` and misbehave when multiple RaiseActionStarted are
		// called in one frame.
		var actor = InitiativeQueue.First();
		Callable.From(() => EventBus.Instance.RaiseActionStarted(actor)).CallDeferred();
	}

	private void AdvanceToNextPhase(bool raiseEndTurnEvent = true)
	{
		if (raiseEndTurnEvent)
			EventBus.Instance.RaisePhaseEnded(Turn, Phase);

		Phase = Phase.Next();

		if (Phase.Equals(TurnPhase.Planning))
			Turn++;

		// Local variables are needed to capture current value in closure, otherwise it'd do `this.Turn, this.Phase`
		// and misbehave when multiple RaisePhaseStarted are called in one frame.
		var turn = Turn;
		var phase = Phase;
		
		Callable.From(() => EventBus.Instance.RaisePhaseStarted(turn, phase)).CallDeferred();
	}
	
	private void OnEntityDestroyed(EntityNode entity, EntityNode? source, bool triggersOnDeathBehaviours)
	{
		if (InitiativeQueue.IsEmpty() || entity is not ActorNode actor) 
			return;

		var currentActorInActionWasDestroyed = InitiativeQueue.First().Equals(actor);
		if (currentActorInActionWasDestroyed)
		{
			if (actor.Player.Equals(Players.Instance.Current) is false)
				return; 
			
			var actionEndedEvent = new ActionEndedRequestEvent
			{
				ActorInAction = actor.InstanceId,
			};
				
			ActionEnded(actionEndedEvent);
			return;
		}
		
		var removed = InitiativeQueue.Remove(actor);
		if (removed)
			EventBus.Instance.RaiseInitiativeQueueUpdated(InitiativeQueue);
	}
}
