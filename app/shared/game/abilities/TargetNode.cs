using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Shape;
using LowAgeData.Domain.Effects;
using MultipurposePathfinding;
using Newtonsoft.Json;

public partial class TargetNode : ActiveAbilityNode<
        TargetNode.ActivationRequest, 
        TargetNode.PreProcessingResult, 
        TargetNode.Focus>,
    INodeFromBlueprint<Target>, IAbilityHasTargetArea
{
    private const string ScenePath = @"res://app/shared/game/abilities/TargetNode.tscn";
    private static TargetNode Instance() => (TargetNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static TargetNode InstantiateAsChild(Target blueprint, Node parentNode, ActorNode owner)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.OwnerActor = owner;
        ability.SetBlueprint(blueprint);
        return ability;
    }
    
    private Target Blueprint { get; set; } = null!;
    private IShape TargetArea { get; set; } = null!;
    private IList<EffectId> Effects { get; set; } = null!;
    
    public void SetBlueprint(Target blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        TargetArea = Blueprint.TargetArea;
        Effects = Blueprint.Effects;
    }
    
    public bool WholeMapIsTargeted() => TargetArea is LowAgeData.Domain.Common.Shape.Map;

    public IEnumerable<Vector2Int> GetTargetPositions(EntityNode caster)
    {
        var mapSize = Registry.MapSize;
        return TargetArea.ToPositions(caster, mapSize);
    }

    public IEnumerable<Vector2> GetGlobalPositionsOfFocusedTargets()
    {
        var globalPositions = new HashSet<Vector2>();
        foreach (var focus in FocusQueue)
        {
            var request = focus.ToActivationRequest();
            var effects = GetEffects((ActivationRequest)request);
            foreach (var effect in effects)
            {
                var targets = effect.Last.FoundTargets;
                foreach (var target in targets)
                {
                    var globalPosition = target switch
                    {
                        Tiles.TileInstance tile => Registry.GetGlobalPositionFromMapPosition(tile.Position),
                        EntityNode entity => entity.GlobalPosition,
                        _ => throw new NotSupportedException($"Unknown {nameof(ITargetable)} type {target.GetType()}")
                    };
                    
                    globalPositions.Add(globalPosition);
                }
            }
        }

        return globalPositions;
    }

    protected override void CancelActivation(ActivationRequest request)
    {
        var focus = FocusQueue.FirstOrDefault(f => Equals(f.TargetEntity, request.EntityToTarget?.InstanceId) 
                                                   && Equals(f.TargetTile, request.TileToTarget?.Point));
        
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(TargetNode), nameof(CancelActivation), 
                $"Found focus '{JsonConvert.SerializeObject(focus)}' for requested entity " +
                $"'{request.EntityToTarget}' and tile '{request.TileToTarget}' in queue " +
                $"'{JsonConvert.SerializeObject(FocusQueue)}'");
        
        if (focus is not null)
        {
            request.EntityToTarget?.TargetedBy.Remove(this);
            request.TileToTarget?.TargetedBy.Remove(this);
            
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
            new AbilityValidator.TargetWithinArea
            {
                AvailablePositions = GetTargetPositions(OwnerActor),
                TargetPositions = GetTargetedPositions(request)
            },
            new AbilityValidator.HasEnoughConsumableResources
            {
                UseConsumableResources = request.UseConsumableResources,
                ConsumableCost = ConsumableCost,
                Player = OwnerActor.Player
            },
            new AbilityValidator.EffectsValidatorsPass
            {
                Effects = GetEffects(request)
            }
        ])
        .Validate();

    protected override AbilityReservationResult HandleReservation(ActivationRequest request)
    {
        request.EntityToTarget?.TargetedBy.Add(this);
        request.TileToTarget?.TargetedBy.Add(this);
        
        return base.HandleReservation(request);
    }

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
            Reservation = preProcessingResult.Reservation,
            TargetEntity = activationRequest.EntityToTarget?.InstanceId,
            TargetTile = activationRequest.TileToTarget?.Point
        };

    protected override bool ExecutePostPaymentAndDetermineIfPaymentCompleted(Focus focus)
    {
        if (focus.Reservation.IsReservedFor(Players.Instance.Current) is false)
        {
            if (focus.TargetEntity is not null)
                Registry.GetEntityById(focus.TargetEntity.Value)?.TargetedBy.Add(this);
            
            if (focus.TargetTile is not null)
                Registry.GetTileFromPoint(focus.TargetTile)?.TargetedBy.Add(this);
        }
        
        return Registry.IsPaymentComplete(NonConsumableCost, focus.NonConsumableResourcesPaidSoFar);
    }

    protected override void ExecuteFocus(Focus focus)
    {
        if (focus.TargetEntity is not null)
            Registry.GetEntityById(focus.TargetEntity.Value)?.TargetedBy.Remove(this);
            
        if (focus.TargetTile is not null)
            Registry.GetTileFromPoint(focus.TargetTile)?.TargetedBy.Remove(this);
        
        // Must be after clearing TargetedBy because validation may use this ability to check for
        // targeted ability condition.
        var effects = GetEffects((ActivationRequest)focus.ToActivationRequest()).ToList();
        var validationResult = AbilityValidator
            .With([new AbilityValidator.EffectsValidatorsPass { Effects = effects }])
            .Validate();
        
        if (Log.DebugEnabled)
            Log.Info(nameof(TargetNode), nameof(ExecuteFocus), 
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

    private IEnumerable<Vector2Int> GetTargetedPositions(ActivationRequest request)
    {
        var positions = new HashSet<Vector2Int>();
        
        if (request.TileToTarget is not null)
            positions.Add(request.TileToTarget.Position);

        if (request.EntityToTarget is not null)
        {
            foreach (var position in request.EntityToTarget.EntityOccupyingPositions)
            {
                positions.Add(position);
            }
        }
        
        return positions;
    }

    private IEnumerable<Effects> GetEffects(ActivationRequest request)
    {
        var initialTargets = new List<ITargetable>();
        
        if (request.TileToTarget is not null)
            initialTargets.Add(request.TileToTarget);
        
        if (request.EntityToTarget is not null)
            initialTargets.Add(request.EntityToTarget);
        
        foreach (var effectId in Effects)
        {
            var effects = new Effects(effectId, initialTargets, OwnerActor.Player, OwnerActor);
            yield return effects;
        }
    }
    
    public class ActivationRequest : IConsumableAbilityActivationRequest
    {
        public bool IsRequeued { get; init; }
        public bool UseConsumableResources { get; init; } = true;
        public required Tiles.TileInstance? TileToTarget { get; init; }
        public required EntityNode? EntityToTarget { get; init; }
    }

    public class PreProcessingResult : IActiveAbilityActivationPreProcessingResult
    {
        public AbilityReservationResult Reservation { get; init; } = null!;
    }

    public class Focus : IActiveAbilityFocus
    {
        public bool Requeued { get; set; }
        public required AbilityReservationResult Reservation { get; set; }
        public IList<Payment> NonConsumableResourcesPaidSoFar { get; set; } = [];
        public required Guid? TargetEntity { get; init; }
        public required Point? TargetTile { get; init; }

        public IConsumableAbilityActivationRequest ToActivationRequest() => new ActivationRequest
        {
            IsRequeued = false,
            UseConsumableResources = true,
            TileToTarget = TargetTile is null ? null : GlobalRegistry.Instance.GetTileFromPoint(TargetTile),
            EntityToTarget = TargetEntity is null ? null : GlobalRegistry.Instance.GetEntityById(TargetEntity.Value),
        };

        public IConsumableAbilityActivationRequest ToActivationRequestForRequeue() => new ActivationRequest
        {
            IsRequeued = true,
            UseConsumableResources = true,
            TileToTarget = TargetTile is null ? null : GlobalRegistry.Instance.GetTileFromPoint(TargetTile),
            EntityToTarget = TargetEntity is null ? null : GlobalRegistry.Instance.GetEntityById(TargetEntity.Value),
        };

        private bool Equals(Focus other) => Equals(TargetEntity, other.TargetEntity) 
                                            && Equals(TargetTile, other.TargetTile);

        public bool Equals(IActiveAbilityFocus? other) => Equals((object?)other);

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Focus)obj);
        }

        public override int GetHashCode() 
            => TargetEntity?.GetHashCode() ?? 0.GetHashCode() 
                ^ TargetTile?.GetHashCode() ?? 0.GetHashCode();
    }
}