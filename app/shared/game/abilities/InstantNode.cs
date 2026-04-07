using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Effects;
using Newtonsoft.Json;

public partial class InstantNode : ActiveAbilityNode<
        InstantNode.ActivationRequest, 
        InstantNode.PreProcessingResult, 
        InstantNode.Focus>,
    INodeFromBlueprint<Instant>
{
    private const string ScenePath = @"res://app/shared/game/abilities/InstantNode.tscn";
    private static InstantNode Instance() => (InstantNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static InstantNode InstantiateAsChild(Instant blueprint, Node parentNode, ActorNode owner)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.OwnerActor = owner;
        ability.SetBlueprint(blueprint);
        return ability;
    }
    
    private Instant Blueprint { get; set; } = null!;
    private IList<EffectId> Effects { get; set; } = null!;
    
    public void SetBlueprint(Instant blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        Effects = Blueprint.Effects;
    }
    
    protected override void CancelActivation(ActivationRequest request)
    {
        var focus = FocusQueue.FirstOrDefault();
        
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(InstantNode), nameof(CancelActivation), 
                $"Found focus '{JsonConvert.SerializeObject(focus)}'.");
        
        if (focus is not null)
        {
            if (focus.NonConsumableResourcesPaidSoFar.IsEmpty()) 
                RefundResources(ConsumableCost);
            
            RemoveFocus(focus);
        }
        
        if (FocusQueue.IsEmpty())
            RefundAction();
    }

    protected override ValidationResult ValidateActivation(ActivationRequest request) => AbilityValidator.With([
            // TODO missing validations: research
            new AbilityValidator.CorrectTurnPhase
            {
                CurrentTurnPhase = Registry.GetCurrentPhase(),
                RequiredTurnPhase = Blueprint.TurnPhase,
                IsRequeued = request.IsRequeued
            },
            new AbilityValidator.ActorHasEnoughAction
            {
                Actor = OwnerActor,
                ActionNeeded = CasterConsumesAction,
                IsRequeued = request.IsRequeued
            },
            new AbilityValidator.CooldownCompleted
            {
                Cooldown = RemainingCooldown
            },
            new AbilityValidator.HasEnoughConsumableResources
            {
                UseConsumableResources = request.UseConsumableResources,
                ConsumableCost = ConsumableCost,
                Player = OwnerActor.Player
            },
            new AbilityValidator.EffectsValidatorsPass
            {
                Effects = GetEffects()
            }
        ])
        .Validate();

    protected override IList<Payment> ReserveResources(ActivationRequest request) => ReserveResources();

    protected override PreProcessingResult CreatePreProcessingResult(ActivationRequest request,
        AbilityReservationResult reservation) => new()
    {
        Reservation = reservation
    };
    
    protected override Focus CreateFocus(ActivationRequest activationRequest, PreProcessingResult preProcessingResult)
        => new Focus
        {
            Requeued = false,
            Reservation = preProcessingResult.Reservation
        };

    protected override bool ExecutePostPaymentAndDetermineIfPaymentCompleted(Focus focus) 
        => Registry.IsPaymentComplete(NonConsumableCost, focus.NonConsumableResourcesPaidSoFar);

    protected override void ExecuteFocus(Focus focus)
    {
        var effects = GetEffects().ToList();
        var validationResult = AbilityValidator
            .With([new AbilityValidator.EffectsValidatorsPass { Effects = effects }])
            .Validate();
        
        if (Log.DebugEnabled)
            Log.Info(nameof(InstantNode), nameof(ExecuteFocus), 
                $"{this} executing focus '{JsonConvert.SerializeObject(focus)}' effects with validation " +
                $"result '{JsonConvert.SerializeObject(validationResult)}'");
        
        if (validationResult.IsValid)
        {
            foreach (var effect in effects)
            {
                effect.ExecuteLast();
            }
            
            RemainingCooldown.ResetDuration();
        }
    }
    
    private IEnumerable<Effects> GetEffects() => Effects
        .Select(effectId => new Effects(effectId, [OwnerActor], OwnerActor.Player, OwnerActor));

    public class ActivationRequest : IConsumableAbilityActivationRequest
    {
        public bool IsRequeued { get; init; }
        public required bool UseConsumableResources { get; init; }
    }

    public class PreProcessingResult : IActiveAbilityActivationPreProcessingResult
    {
        public AbilityReservationResult? Reservation { get; init; }
    }

    public class Focus : IActiveAbilityFocus
    {
        public bool Requeued { get; set; }
        public required AbilityReservationResult Reservation { get; set; }
        public IList<Payment> NonConsumableResourcesPaidSoFar { get; set; } = [];
        
        public IConsumableAbilityActivationRequest ToActivationRequest() => new ActivationRequest
        {
            IsRequeued = false,
            UseConsumableResources = true,
        };

        public IConsumableAbilityActivationRequest ToActivationRequestForRequeue() => new ActivationRequest
        {
            IsRequeued = true,
            UseConsumableResources = true,
        };

        private bool Equals(Focus other) => true;

        public bool Equals(IActiveAbilityFocus? other) => Equals((object?)other);

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Focus)obj);
        }

        public override int GetHashCode() => 0.GetHashCode();
    }
}
