using System;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using Newtonsoft.Json;

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
    protected bool CasterConsumesAction { get; set; } = true;
    
    public override void _Ready()
    {
        base._Ready();

        EventBus.Instance.PhaseStarted += OnPhaseStarted;
        EventBus.Instance.ActionStarted += OnActionStarted;
        EventBus.Instance.PhaseEnded += OnPhaseEnded;
        EventBus.Instance.ActionEnded += OnActionEnded;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        EventBus.Instance.ActionStarted -= OnActionStarted;
        EventBus.Instance.PhaseEnded -= OnPhaseEnded;
        EventBus.Instance.ActionEnded -= OnActionEnded;
        
        base._ExitTree();
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

    protected void RefundAction()
    {
        var currentPhase = GlobalRegistry.Instance.GetCurrentPhase();
        OwnerActor.ActionEconomy.Restore(currentPhase, true);

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

    protected override TPreProcessingResult PreProcessActivation(TActivationRequest request)
    {
        CancelActivation(request);
        var reservation = HandleReservation(request);
        return CreatePreProcessingResult(request, reservation);
    }

    protected virtual AbilityReservationResult HandleReservation(TActivationRequest request)
    {
        // TODO
        // var resources = ReserveResources(request.UseConsumableResources);
        
        GD.Print($"Starting reservation, {nameof(CasterConsumesAction)} '{CasterConsumesAction}', " +
                 $"{nameof(OwnerActor.WorkingOn)} '{JsonConvert.SerializeObject(OwnerActor.WorkingOn.Count)}'");
        
        var actionWasReserved = false;
        if (CasterConsumesAction)
        {
            OwnerActor.ActionEconomy.UsedAbilityAction();
            actionWasReserved = true;
        }

        var workingOn = new WorkingOnAbility
        {
            Ability = this,
            Timing = TurnPhase,
            ConsumesAction = actionWasReserved
        };
        if (OwnerActor.WorkingOn.Contains(workingOn) is false)
            OwnerActor.WorkingOn.Add(workingOn);
        
        var reservation = new AbilityReservationResult
        {
            PlayerId = OwnerActor.Player.Id,
            ActionWasReserved = actionWasReserved
        };
        
        GD.Print($"Returning new reservation '{JsonConvert.SerializeObject(reservation)}' for activation " +
                 $"request {nameof(request.UseConsumableResources)} '{request.UseConsumableResources}'.");
        
        return reservation;
    }
    
    protected abstract TPreProcessingResult CreatePreProcessingResult(TActivationRequest request, 
        AbilityReservationResult reservation);

    protected override void OnExecutionRequested(TFocus focus)
    {
        if (FocusQueue.Contains(focus))
            focus = FocusQueue.First(f => f.Equals(focus));
        else
            FocusQueue.Add(focus);
        
        if (TryExecutePrePayment(focus) is false)
            return;

        ExecutePaymentUpdate(focus);

        var paymentCompleted = TryExecutePostPayment(focus);
        
        GD.Print($"Payment completed is '{paymentCompleted}' for focus '{JsonConvert.SerializeObject(focus)}'");
        
        if (paymentCompleted)
            Complete(focus);
        else
            Requeue(focus);
        
        GD.Print($"End of execution current focus queue '{JsonConvert.SerializeObject(FocusQueue)}'");
    }

    protected abstract bool TryExecutePrePayment(TFocus focus);

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
        GD.Print($"Requeue successful for focus '{JsonConvert.SerializeObject(focus)}'");
    }

    private void HandleRequeuedFocus(TFocus focus)
    {
        GD.Print($"Handling requeued focus for '{JsonConvert.SerializeObject(focus)}'");
        
        var activationRequest = focus.ToActivationRequestForRequeue();
        if (activationRequest is not TActivationRequest typedRequest)
        {
            GD.Print($"Requeue focus was not the correct type");
            FocusQueue.Remove(focus);
            return;
        }
            
        var reservation = HandleReservation(typedRequest);
        focus.Reservation = reservation;
        focus.Requeued = false;
    }
    
    private bool ActionAllowedForPlanningPhase(TurnPhase currentPhase)
    {
        if (TurnPhase.Equals(TurnPhase.Planning) is false)
            return false;

        if (currentPhase.Equals(TurnPhase.Planning) is false)
            return false;

        return true;
    }

    private bool ActionAllowedAsAnActionInActionPhase(ActorNode actor)
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
            CancelActivation(nonRequeuedFocus.ToActivationRequestForRequeue());
        }
    }
    
    private void HandleRequeuedAbilityFocuses()
    {
        foreach (var focus in FocusQueue.Where(f => f.Requeued).ToList())
        {
            HandleRequeuedFocus(focus);
        }
    }

    private void OnPhaseStarted(int turn, TurnPhase phase)
    {
        if (ActionAllowedForPlanningPhase(phase) is false)
            return;
        
        CleanUpNonRequeuedAbilityFocuses();
        HandleRequeuedAbilityFocuses();
    }
    
    private void OnPhaseEnded(int turn, TurnPhase phase)
    {
        if (ActionAllowedForPlanningPhase(phase) is false)
            return;
        
        RequestExecution();
    }
    
    private void OnActionStarted(ActorNode actor)
    {
        if (ActionAllowedAsAnActionInActionPhase(actor) is false)
            return;
        
        CleanUpNonRequeuedAbilityFocuses();
        HandleRequeuedAbilityFocuses();
    }

    private void OnActionEnded(ActorNode actor)
    {
        if (ActionAllowedAsAnActionInActionPhase(actor) is false)
            return;
        
        RequestExecution();
    }
}

public record AbilityReservationResult
{
    // TODO public required Resources ReservedConsumableResources { get; init; }
    public required int PlayerId { get; init; }
    public required bool ActionWasReserved { get; init; }

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