using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeCommon.Extensions;
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
    public event Action<IActiveAbilityNode> ActivationsCancelled = delegate { };
    public event Action<IActiveAbilityFocus> FocusRemoved = delegate { };
    
    public bool IsActivated => FocusQueue.Any();
    
    protected bool CasterConsumesAction { get; set; } = true;
    
    public override void _Ready()
    {
        base._Ready();
        
        EventBus.Instance.PhaseEnded += OnPhaseEnded;
        EventBus.Instance.ActionEnded += OnActionEnded;
        EventBus.Instance.EntityDestroyed += OnOwnerActorDestroyed;
    }

    public override void _ExitTree()
    {
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

        ActivationsCancelled(this);
    }

    public void CancelActivation(IAbilityActivationRequest request)
    {
        if (request is not TActivationRequest typedRequest)
        {
            Log.Error(nameof(ActiveAbilityNode<,,>), nameof(CancelActivation), 
                $"Invalid {nameof(IAbilityActivationRequest)} type. Expected " +
                        $"'{typeof(TActivationRequest).Name}', but got '{request.GetType().Name}'");
            return;
        }

        CancelActivation(typedRequest);
    }
    
    protected abstract void CancelActivation(TActivationRequest request);

    protected void RefundAction()
    {
        var currentPhase = Registry.GetCurrentPhase();
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
        EventBus.Instance.RaiseResourcePaymentRequested(OwnerActor.Player, resources, true);
    }

    protected override TPreProcessingResult PreProcessActivation(TActivationRequest request)
    {
        CancelActivation(request);
        var reservation = HandleReservation(request);
        return CreatePreProcessingResult(request, reservation);
    }

    protected virtual AbilityReservationResult HandleReservation(TActivationRequest request)
    {
        var reservedConsumableResources = ReserveResources(request);
        
        var actionWasReserved = false;
        if (CasterConsumesAction && OwnerActor.AddWorkingOnAbility(this, TurnPhase, CasterConsumesAction))
        {
            OwnerActor.ActionEconomy.UsedAbilityAction();
            actionWasReserved = true;
        }
        
        var reservation = new AbilityReservationResult
        {
            PlayerStableId = OwnerActor.Player.StableId,
            ActionWasReserved = actionWasReserved,
            ReservedConsumableResources = reservedConsumableResources
        };
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleReservation), 
                $"{OwnerActor} finishing to reserve with result '{JsonConvert.SerializeObject(reservation)}'");
        
        return reservation;
    }
    
    protected abstract IList<Payment> ReserveResources(TActivationRequest request);

    protected IList<Payment> ReserveResources()
    {
        EventBus.Instance.RaiseResourcePaymentRequested(OwnerActor.Player, ConsumableCost, false);
        return ConsumableCost;
    }
    
    protected abstract TPreProcessingResult CreatePreProcessingResult(TActivationRequest request, 
        AbilityReservationResult reservation);

    protected override void RequestExecution()
    {
        var currentPlayerStableId = Players.Instance.Current.StableId;
        foreach (var focus in FocusQueue
                     .Where(focus => focus.Reservation.PlayerStableId.Equals(currentPlayerStableId) is false)
                     .ToList())
        {
            RemoveFocus(focus);
        }

        base.RequestExecution();
    }

    protected override void OnExecutionRequested(TFocus focus)
    {
        SpendActionAndConsumableResources(focus);

        ExecuteNonConsumablePayment(focus, null);

        var paymentCompleted = ExecutePostPaymentAndDetermineIfPaymentCompleted(focus);

        if (paymentCompleted is false)
        {
            Requeue(focus);
            return;
        }
        
        ExecuteFocus(focus);
        
        if (Loops)
            Requeue(focus);
    }
    
    protected void SpendActionAndConsumableResources(TFocus focus)
    {
        if (focus.Reservation.PlayerStableId == Players.Instance.Current.StableId
            && Registry.GetLoadingSavedGame() is false)
            return; // We already spent these when creating reservation (in a non-loading game state)
        
        var activationRequest = focus.ToActivationRequest();
        if (activationRequest is not TActivationRequest typedRequest)
            return;
        
        HandleReservation(typedRequest);
    }

    protected void ExecuteNonConsumablePayment(TFocus focus, IList<Payment>? nonConsumableCost)
    {
        nonConsumableCost ??= NonConsumableCost;
        if (nonConsumableCost.IsEmpty())
            return;
        
        var currentPlayerStockpile = Registry.GetCurrentPlayerStockpile(OwnerActor.Player);
        var nonConsumableStockpile = Registry.GetNonConsumableResources(currentPlayerStockpile);
        var (_, updatedPaidSoFar) = Registry.SimulatePayment(nonConsumableCost,
            nonConsumableStockpile, focus.NonConsumableResourcesPaidSoFar, 1f);

        focus.NonConsumableResourcesPaidSoFar = updatedPaidSoFar;
    }
    
    protected abstract bool ExecutePostPaymentAndDetermineIfPaymentCompleted(TFocus focus);

    private void Requeue(TFocus focus)
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleReservation), 
                $"{OwnerActor} requeue: reservation player ID '{focus.Reservation.PlayerStableId}', " +
                $"current player stable ID '{Players.Instance.Current.StableId}', " +
                $"focus '{JsonConvert.SerializeObject(focus)}'");
        
        // Only requeue for the owner player for convenience but still allow to cancel if desired,
        // every other player will receive (or not) a new execution request from the owner player.
        if (focus.Reservation.PlayerStableId != Players.Instance.Current.StableId)
            return; 
        
        focus.Requeued = true;
    }

    private void HandleRequeuedFocus(TFocus focus)
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleRequeuedFocus), 
                $"Starting requeue for {this}");
        
        if (WasAlreadyCompleted(focus))
        {
            if (Log.DebugEnabled)
                Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleRequeuedFocus), 
                    $"{OwnerActor} requeued focus '{JsonConvert.SerializeObject(focus)}' will not be " +
                    $"handled because it was already completed");
            
            ExecuteFocus(focus);
            RemoveFocus(focus);
            RefundAction();
            return;
        }
        
        var activationRequest = focus.ToActivationRequestForRequeue();
        if (activationRequest is not TActivationRequest typedRequest)
        {
            if (Log.DebugEnabled)
                Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleRequeuedFocus), 
                    "Requeue focus was not the correct type");
            
            RemoveFocus(focus);
            return;
        }

        var validationResult = ValidateActivation(typedRequest);
        if (Loops is false && validationResult.IsValid is false)
        {
            if (Log.DebugEnabled)
                Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleRequeuedFocus), 
                    $"{this} could not requeue focus '{JsonConvert.SerializeObject(focus)}' " +
                    $"because '{validationResult.Message}'");
            
            CancelActivation(typedRequest);
            return;
        }
            
        var reservation = HandleReservation(typedRequest);
        focus.Reservation = reservation;
        focus.Requeued = false;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleRequeuedFocus), 
                $"{OwnerActor} requeued focus '{JsonConvert.SerializeObject(focus)}' was handled");
    }

    protected void RemoveFocus(TFocus focus)
    {
        FocusQueue.Remove(focus);
        
        if (FocusQueue.IsEmpty())
        {
            OwnerActor.RemoveWorkingOnAbility(this); 
        }
        
        FocusRemoved(focus);
    }

    /// <summary>
    /// Relevant for abilities that allow helpers.
    /// </summary>
    protected virtual bool WasAlreadyCompleted(TFocus focus) => false;

    private bool AbilityAllowedAsAnActionInActionPhaseFor(ActorNode actor)
    {
        if (TurnPhase.Equals(TurnPhase.Action) is false)
            return false;
        
        if (OwnerActor.Equals(actor) is false)
            return false;

        return true;
    }

    private void CleanUpAllFocuses()
    {
        foreach (var focus in FocusQueue.ToList())
        {
            var activationRequest = focus.ToActivationRequest();
            CancelActivation(activationRequest);
        }
    }
    
    private void CleanUpNonRequeuedFocuses()
    {
        foreach (var nonRequeuedFocus in FocusQueue.Where(f => f.Requeued is false).ToList())
        {
            var activationRequest = nonRequeuedFocus.ToActivationRequest();
            CancelActivation(activationRequest);
        }
    }
    
    private void HandleRequeuedAbilityFocuses()
    {
        foreach (var focus in FocusQueue.Where(f => f.Requeued).ToList())
        {
            HandleRequeuedFocus(focus);
        }
    }

    private void HandleEndOfPlanningPhase()
    {
        if (TurnPhase.Equals(TurnPhase.Planning) is false)
            return;
        
        if (Registry.GetLoadingSavedGame())
            CleanUpAllFocuses();
        
        RequestExecution();
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleEndOfPlanningPhase), 
                $"{this} current {nameof(FocusQueue)} (after {nameof(RequestExecution)}): " +
                $"'{JsonConvert.SerializeObject(FocusQueue)}'");
    }

    private void HandleEndOfActionPhase()
    {
        CleanUpNonRequeuedFocuses();
        
        if (TurnPhase.Equals(TurnPhase.Planning) is false)
            return;
        
        if (Registry.GetLoadingSavedGame())
            return;
        
        HandleRequeuedAbilityFocuses();
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ActiveAbilityNode<,,>), nameof(HandleEndOfActionPhase), 
                $"{this} current {nameof(FocusQueue)} (after {nameof(HandleRequeuedAbilityFocuses)}): " +
                $"'{JsonConvert.SerializeObject(FocusQueue)}'");
    }
    
    private void OnPhaseEnded(int turn, TurnPhase currentPhase)
    {
        if (currentPhase.Equals(TurnPhase.Planning))
            HandleEndOfPlanningPhase();
        
        if (currentPhase.Equals(TurnPhase.Action))
            HandleEndOfActionPhase();
    }

    private void OnActionStarted(ActorNode actor)
    {
        if (AbilityAllowedAsAnActionInActionPhaseFor(actor) is false)
            return;
        
        if (Registry.GetLoadingSavedGame())
            return;
        
        CleanUpNonRequeuedFocuses();
        HandleRequeuedAbilityFocuses();
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ActiveAbilityNode<,,>), nameof(OnActionStarted), 
                $"{this} current {nameof(FocusQueue)}: '{JsonConvert.SerializeObject(FocusQueue)}'");
    }

    private void OnActionEnded(ActorNode actor)
    {
        if (AbilityAllowedAsAnActionInActionPhaseFor(actor) is false)
            return;
        
        if (Registry.GetLoadingSavedGame())
            CleanUpAllFocuses();
        
        RequestExecution();
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ActiveAbilityNode<,,>), nameof(OnActionEnded), 
                $"{this} current {nameof(FocusQueue)}: '{JsonConvert.SerializeObject(FocusQueue)}'");
    }
    
    private void OnOwnerActorDestroyed(EntityNode entity, EntityNode? source, bool triggersOnDeathBehaviours)
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
    public required int PlayerStableId { get; init; }
    public required bool ActionWasReserved { get; init; }
    public required IList<Payment> ReservedConsumableResources { get; init; }

    public bool IsReservedFor(Player player) => player.StableId == PlayerStableId;
}

public interface IConsumableAbilityActivationRequest : IAbilityActivationRequest
{
    bool IsRequeued { get; init; }
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
    IList<Payment> NonConsumableResourcesPaidSoFar { get; set; }
    
    IConsumableAbilityActivationRequest ToActivationRequest();
    IConsumableAbilityActivationRequest ToActivationRequestForRequeue();
}