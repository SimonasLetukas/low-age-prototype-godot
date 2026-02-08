using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;

/// <summary>
/// Note: since visualisation in-game is not needed, this abstract class has no node/scene. 
/// </summary>
public abstract partial class ActiveAbilityNode<
    TActivationRequest, 
    TPreProcessingResult, 
    TFocus> 
    : AbilityNode<TActivationRequest, TPreProcessingResult, TFocus>, IActiveAbilityNode
    where TActivationRequest : IConsumableAbilityActivationRequest
    where TPreProcessingResult : IActiveAbilityActivationPreProcessingResult
    where TFocus : IActiveAbilityFocus
{
    public event Action<IActiveAbilityFocus> Cancelled = delegate { };
    
    public bool IsActivated => FocusQueue.Any();
    
    protected bool CasterConsumesAction { get; set; } = true;
    
    public override void _Ready()
    {
        base._Ready();

        EventBus.Instance.PhaseStarted += OnPhaseStarted;
        EventBus.Instance.ActionStarted += OnActionStarted;
        EventBus.Instance.PhaseEnded += OnPhaseEnded;
        EventBus.Instance.ActionEnded += OnActionEnded;
        EventBus.Instance.EntityDestroyed += OnOwnerActorDestroyed;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        EventBus.Instance.ActionStarted -= OnActionStarted;
        EventBus.Instance.PhaseEnded -= OnPhaseEnded;
        EventBus.Instance.ActionEnded -= OnActionEnded;
        EventBus.Instance.EntityDestroyed -= OnOwnerActorDestroyed;

        base._ExitTree();
    }

    public void CancelActivations()
    {
        foreach (var focus in FocusQueue.ToList())
        {
            var request = focus.ToActivationRequest();
            CancelActivation(request);
        }
    }

    public void CancelActivation(IAbilityActivationRequest request)
    {
        if (request is not TActivationRequest typedRequest)
        {
            GD.PrintErr($"Invalid {nameof(IAbilityActivationRequest)} type. Expected " +
                        $"'{typeof(TActivationRequest).Name}', but got '{request.GetType().Name}'");
            return;
        }

        CancelActivation(typedRequest);
    }
    
    protected abstract void CancelActivation(TActivationRequest request);
    
    protected void RaiseCancelled(IActiveAbilityFocus focus) => Cancelled(focus);

    protected void RefundAction()
    {
        var currentPhase = GlobalRegistry.Instance.GetCurrentPhase();
        OwnerActor.RestoreActionEconomy(currentPhase, true);

        if (currentPhase.Equals(TurnPhase.Action))
        {
            // TODO re-add into the initiative queue calculation if the actor isn't already part of the
            // initiative queue (i.e. it's its turn), or just always recalculate for each such instance.
            // Think of how to handle this in the multiplayer, maybe this does not need to be sent over 
            // the multiplayer because each client will handle the ability execution anyway, during which
            // the work would be cancelled.
            // Alternatively, just move the initiative queue creation to always happen one step after
            // ability executions.
        }
    }

    protected void RefundResources(IList<Payment> resources)
    {
        EventBus.Instance.RaisePaymentRequested(OwnerActor.Player, resources, true);
    }

    protected override TPreProcessingResult PreProcessActivation(TActivationRequest request)
    {
        CancelActivation(request);
        var reservation = HandleReservation(request);
        return CreatePreProcessingResult(request, reservation);
    }

    protected virtual AbilityReservationResult HandleReservation(TActivationRequest request)
    {
        var reservedResources = ReserveResources(request);
        
        var actionWasReserved = false;
        if (CasterConsumesAction)
        {
            OwnerActor.ActionEconomy.UsedAbilityAction();
            actionWasReserved = true;
        }
        
        OwnerActor.AddWorkingOnAbility(this, TurnPhase, actionWasReserved);
        
        var reservation = new AbilityReservationResult
        {
            PlayerId = OwnerActor.Player.Id,
            ActionWasReserved = actionWasReserved,
            ReservedResources = reservedResources
        };
        
        return reservation;
    }
    
    protected abstract IList<Payment> ReserveResources(TActivationRequest request);
    
    protected abstract TPreProcessingResult CreatePreProcessingResult(TActivationRequest request, 
        AbilityReservationResult reservation);

    protected override void OnExecutionRequested(TFocus focus)
    {
        if (FocusQueue.Contains(focus))
            focus = FocusQueue.First(f => f.Equals(focus));
        else
            FocusQueue.Add(focus);
        
        SpendActionAndConsumableResources(focus);

        ExecutePaymentUpdate(focus);

        var paymentCompleted = TryExecutePostPayment(focus);
        
        if (paymentCompleted is false)
            Requeue(focus);
        
        RemainingCooldown.ResetDuration();
    }
    
    protected void SpendActionAndConsumableResources(TFocus focus)
    {
        if (focus.Reservation.PlayerId == Players.Instance.Current.Id)
            return; // We already spent these when creating reservation

        var activationRequest = focus.ToActivationRequest();
        if (activationRequest is not TActivationRequest typedRequest)
            return;
        
        HandleReservation(typedRequest);
    }

    private static void ExecutePaymentUpdate(TFocus focus)
    {
        // TODO 
        
        // Most probably a static method which takes the current resources and adds them to the paid amount
        
        // Would probably need to return the delta or percentage of each resource added so that post-payment
        // could use those numbers to update the progress if needed 
        
        // Also input might need to specify if consumable resources need to be deducted, but that most probably
        // should happen in pre-payment (and naming should probably be adjusted as well, something to indicate
        // "consumable" part)
    }
    
    protected abstract bool TryExecutePostPayment(TFocus focus);

    private static void Requeue(TFocus focus)
    {
        GD.Print($"Requeue: reservation player ID '{focus.Reservation.PlayerId}', " +
                 $"current player ID '{Players.Instance.Current.Id}'");
        
        // Only requeue for the owner player for convenience but still allow to cancel if desired,
        // every other player will receive (or not) a new execution request from the owner player.
        if (focus.Reservation.PlayerId != Players.Instance.Current.Id)
            return; 
        
        focus.Requeued = true;
    }

    private void HandleRequeuedFocus(TFocus focus)
    {
        if (WasAlreadyCompleted(focus))
        {
            Complete(focus);
            return;
        }
        
        var activationRequest = focus.ToActivationRequestForRequeue();
        if (activationRequest is not TActivationRequest typedRequest)
        {
            GD.Print("Requeue focus was not the correct type");
            FocusQueue.Remove(focus);
            return;
        }
            
        var reservation = HandleReservation(typedRequest);
        focus.Reservation = reservation;
        focus.Requeued = false;
    }

    /// <summary>
    /// Relevant for abilities that allow helpers.
    /// </summary>
    protected virtual bool WasAlreadyCompleted(TFocus focus) => false;
    
    private bool AbilityAllowedForPlanningPhaseGiven(TurnPhase currentPhase)
    {
        if (TurnPhase.Equals(TurnPhase.Planning) is false)
            return false;

        if (currentPhase.Equals(TurnPhase.Planning) is false)
            return false;

        return true;
    }

    private bool AbilityAllowedAsAnActionInActionPhaseFor(ActorNode actor)
    {
        if (TurnPhase.Equals(TurnPhase.Action) is false)
            return false;
        
        if (OwnerActor.Equals(actor) is false)
            return false;

        return true;
    }
    
    private void CleanUpNonRequeuedAbilityFocuses()
    {
        foreach (var nonRequeuedFocus in FocusQueue.Where(f => f.Requeued is false).ToList())
        {
            Complete(nonRequeuedFocus);
        }
    }
    
    private void HandleRequeuedAbilityFocuses()
    {
        foreach (var focus in FocusQueue.Where(f => f.Requeued).ToList())
        {
            HandleRequeuedFocus(focus);
        }
    }

    private void OnPhaseStarted(int turn, TurnPhase currentPhase)
    {
        if (AbilityAllowedForPlanningPhaseGiven(currentPhase) is false)
            return;
        
        CleanUpNonRequeuedAbilityFocuses();
        HandleRequeuedAbilityFocuses();
    }
    
    private void OnPhaseEnded(int turn, TurnPhase currentPhase)
    {
        if (AbilityAllowedForPlanningPhaseGiven(currentPhase) is false)
            return;
        
        RequestExecution();
    }
    
    private void OnActionStarted(ActorNode actor)
    {
        if (AbilityAllowedAsAnActionInActionPhaseFor(actor) is false)
            return;
        
        CleanUpNonRequeuedAbilityFocuses();
        HandleRequeuedAbilityFocuses();
    }

    private void OnActionEnded(ActorNode actor)
    {
        if (AbilityAllowedAsAnActionInActionPhaseFor(actor) is false)
            return;
        
        RequestExecution();
    }
    
    private void OnOwnerActorDestroyed(EntityNode entity)
    {
        if (entity is not ActorNode actor)
            return;

        if (OwnerActor.Equals(actor) is false)
            return;
        
        CancelActivations();
    }
}

public record AbilityReservationResult
{
    public required int PlayerId { get; init; }
    public required bool ActionWasReserved { get; init; }
    public required IList<Payment> ReservedResources { get; init; }

    public bool IsReservedFor(Player player) => player.Id == PlayerId;
}

public interface IConsumableAbilityActivationRequest : IAbilityActivationRequest
{
    bool UseConsumableResources { get; init; }
}

public interface IActiveAbilityActivationPreProcessingResult : IAbilityActivationPreProcessingResult
{
    AbilityReservationResult Reservation { get; init; }
}

public interface IActiveAbilityFocus : IAbilityFocus, IEquatable<IActiveAbilityFocus>
{
    bool Requeued { get; set; }
    AbilityReservationResult Reservation { get; set; }
    //IList<Payment> Cost { get; init; }
    //IList<Payment> PaymentPaid { get; init; }
    
    IConsumableAbilityActivationRequest ToActivationRequest();
    IConsumableAbilityActivationRequest ToActivationRequestForRequeue();
}