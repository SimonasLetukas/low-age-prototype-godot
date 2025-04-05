using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;

/// <summary>
/// Responsible for keeping track of turns and phases.
///
/// This class is not a singleton to ensure constrained communication flow (Game -> Turns -> EventBus -> Listener).
/// </summary>
public partial class Turns : Node2D
{
	[Export] public bool DebugEnabled { get; set; } = false;
	
	public event Action<PlanningPhaseEndedRequestEvent> PlanningPhaseEnded = delegate { };
	public event Action<ActionEndedEvent> ActionEnded = delegate { };

	private int Turn { get; set; }
	private TurnPhase Phase { get; set; } = TurnPhase.Planning;
	private IList<ActorNode> InitiativeQueue { get; set; } = [];

	private bool _waitingForServerResponse = false;
	private Func<IList<ActorNode>>? _getActorsSortedByInitiative;
	private Func<IList<EntityNode>>? _getCandidateEntities;

	public override void _Ready()
	{
		if (DebugEnabled) GD.Print($"{nameof(Turns)}: entering");
		
		base._Ready();
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
				PlayerId = Players.Instance.Current.Id,
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
			
			var actionEndedEvent = new ActionEndedEvent
			{
				ActorInAction = actorInAction.InstanceId,
			};
				
			ActionEnded(actionEndedEvent);
		}
	}

	public void HandleEvent(PlanningPhaseEndedResponseEvent @event)
	{
		_waitingForServerResponse = false;
		Turn = @event.Turn;
		
		AdvanceToNextPhase();
		
		InitiativeQueue = _getActorsSortedByInitiative is null ? [] : _getActorsSortedByInitiative();

		if (InitiativeQueue.IsEmpty())
		{
			AdvanceToNextPhase();
			return;
		}
		
		AdvanceToNextAction(true);
	}

	public void HandleEvent(ActionEndedEvent @event)
	{
		if (InitiativeQueue.FirstOrDefault() is not { } actor
		    || actor.InstanceId.Equals(@event.ActorInAction) is false)
			return;
		
		AdvanceToNextAction(false);
		
		if (InitiativeQueue.IsEmpty())
		{
			AdvanceToNextPhase();
		}
	}

	private void AdvanceToNextAction(bool firstAction)
	{
		if (InitiativeQueue.IsEmpty())
			return;

		if (firstAction is false)
		{
			EventBus.Instance.RaiseActionEnded(InitiativeQueue.First());
			
			InitiativeQueue.RemoveAt(0);
			EventBus.Instance.RaiseInitiativeQueueUpdated(InitiativeQueue);
			
			if (InitiativeQueue.IsEmpty())
				return;
		}
		
		EventBus.Instance.RaiseActionStarted(InitiativeQueue.First());
	}

	private void AdvanceToNextPhase()
	{
		EventBus.Instance.RaisePhaseEnded(Turn, Phase);

		Phase = Phase.Next();

		if (Phase.Equals(TurnPhase.Planning))
			Turn++;
		
		EventBus.Instance.RaisePhaseStarted(Turn, Phase);
	}
}
