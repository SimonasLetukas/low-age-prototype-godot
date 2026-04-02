using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Behaviours;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Common.Shape;
using LowAgeData.Domain.Effects;
using MultipurposePathfinding;
using Newtonsoft.Json;

public partial class PassiveNode : AbilityNode<
        PassiveNode.ActivationRequest, 
        PassiveNode.PreProcessingResult, 
        PassiveNode.Focus>, 
    INodeFromBlueprint<Passive>, IAbilityHasTargetArea
{
    private const string ScenePath = @"res://app/shared/game/abilities/PassiveNode.tscn";
    private static PassiveNode Instance() => (PassiveNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static PassiveNode InstantiateAsChild(Passive blueprint, Node parentNode, ActorNode owner)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.OwnerActor = owner;
        ability.SetBlueprint(blueprint);
        return ability;
    }
    
    private Passive Blueprint { get; set; } = null!;

    private IShape? _targetArea;
    private HashSet<EntityNode> _periodicEffectTrackedEntities = [];
    
    public void SetBlueprint(Passive blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        _targetArea = GetPeriodicSearchEffect()?.Shape;

        EventBus.Instance.UnitMoved += OnUnitMoved;
        EventBus.Instance.ActionEnded += OnActionEnded;
        EventBus.Instance.PhaseStarted += OnPhaseStarted;
        EventBus.Instance.PhaseEnded += OnPhaseEnded;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.UnitMoved -= OnUnitMoved;
        EventBus.Instance.ActionEnded -= OnActionEnded;
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        EventBus.Instance.PhaseEnded -= OnPhaseEnded;
        
        base._ExitTree();
    }
    
    public bool WholeMapIsTargeted() => _targetArea is LowAgeData.Domain.Common.Shape.Map;
    
    public IEnumerable<Vector2Int> GetTargetPositions(EntityNode caster)
    {
        var mapSize = Registry.MapSize;
        return _targetArea?.ToPositions(caster, mapSize) ?? [];
    }

    public IEnumerable<Vector2> GetGlobalPositionsOfFocusedTargets() 
        => _periodicEffectTrackedEntities.Select(entity => entity.GlobalPosition);

    public BehaviourId? GetOnBuildBehaviourOrDefault()
    {
        return Blueprint.OnBuildBehaviour;
    }
    
    protected override ValidationResult ValidateActivation(ActivationRequest request) => AbilityValidator.With([
            // TODO missing validations: research
            new AbilityValidator.EffectsValidatorsPass
            {
                Effects = [GetEffect(request)]
            }
        ])
        .Validate();

    protected override Focus CreateFocus(ActivationRequest activationRequest, PreProcessingResult? preProcessingResult)
        => new()
        {
            TargetEntities = activationRequest.Targets
                .Where(t => t is EntityNode)
                .Cast<EntityNode>()
                .Select(e => e.InstanceId)
                .ToList(),
            TargetTiles = activationRequest.Targets
                .Where(t => t is Tiles.TileInstance)
                .Cast<Tiles.TileInstance>()
                .Select(t => t.Point)
                .ToList(),
            EffectToExecute = activationRequest.EffectToExecute
        };

    protected override void OnExecutionRequested(Focus focus)
    {
        ExecuteFocus(focus);
        FocusQueue.Remove(focus);
    }

    protected override void ExecuteFocus(Focus focus)
    {
        var effect = GetEffect(focus.ToActivationRequest());
        var validationResult = AbilityValidator
            .With([new AbilityValidator.EffectsValidatorsPass { Effects = [effect] }])
            .Validate();

        if (Log.DebugEnabled)
            Log.Info(nameof(PassiveNode), nameof(ExecuteFocus),
                $"{this} executing focus '{JsonConvert.SerializeObject(focus)}' effects with validation " +
                $"result '{JsonConvert.SerializeObject(validationResult)}'");

        if (validationResult.IsValid is false)
            return;
        
        effect.ExecuteLast();

        HandlePeriodicEffectExecution(effect);
    }

    private void HandlePeriodicEffectExecution(Effects effect)
    {
        if (effect.Last.Id.Equals(Blueprint.PeriodicSearchEffect) is false
            || effect.Last is not SearchNode searchNode)
            return;

        var newTrackedEntities = new HashSet<EntityNode>();

        foreach (var entity in searchNode.FoundTargets
                     .Where(t => t is EntityNode)
                     .Cast<EntityNode>())
        {
            newTrackedEntities.Add(entity);
        }
        
        if (Log.DebugEnabled)
            Log.Info(nameof(PassiveNode), nameof(HandlePeriodicEffectExecution), 
                $"Found new tracked entities '{string.Join(", ", newTrackedEntities.Select(e => 
                    e.ToString()))}', current tracked entities '{string.Join(", ", _periodicEffectTrackedEntities
                    .Select(e => e.ToString()))}'.");

        foreach (var trackedEntity in _periodicEffectTrackedEntities)
        {
            if (newTrackedEntities.Contains(trackedEntity)) 
                continue;
            
            var behavioursToRemove = trackedEntity.Behaviours.GetAll()
                .Where(b => b.History.First.Id.Equals(searchNode.Id));
            
            if (Log.DebugEnabled)
                Log.Info(nameof(PassiveNode), nameof(HandlePeriodicEffectExecution), 
                    $"Removing behaviours: {string.Join(", ", behavioursToRemove.Select(b => 
                        b.ToString()))}");
                
            foreach (var behaviour in behavioursToRemove) 
                behaviour.Remove();
        }

        _periodicEffectTrackedEntities = newTrackedEntities;
    }

    private void HandlePeriodicEffectActivation()
    {
        var effectId = Blueprint.PeriodicSearchEffect;
        if (effectId is null)
            return;
        
        var activationRequest = new ActivationRequest
        {
            Targets = [OwnerActor],
            EffectToExecute = effectId
        };
            
        if (Activate(activationRequest).IsValid) 
            RequestExecution();
    }

    private SearchNode? GetPeriodicSearchEffect()
    {
        if (Blueprint.PeriodicSearchEffect is null)
            return null;

        return new Effects(Blueprint.PeriodicSearchEffect, [OwnerActor], OwnerActor.Player, OwnerActor).Last 
            as SearchNode;
    }

    private Effects GetEffect(ActivationRequest request)
        => new(request.EffectToExecute, request.Targets, OwnerActor.Player, OwnerActor);

    private bool PassiveCanBeTriggered() => GlobalRegistry.Instance.GetLoadingSavedGame() is false
                                            && IsActorOwnedByCurrentPlayer();

    private bool IsActorOwnedByCurrentPlayer() => OwnerActor.Player.Equals(Players.Instance.Current);

    public void OnAttackExecuted(AttackType attackType, EntityNode targetEntity)
    {
        if (PassiveCanBeTriggered() is false)
            return;

        if (Blueprint.OnHitAttackTypes.Contains(attackType) is false)
            return;
        
        var onHitEffects = Blueprint.OnHitEffects;
        if (onHitEffects.IsEmpty())
            return;
        
        foreach (var effectId in onHitEffects)
        {
            var activationRequest = new ActivationRequest
            {
                Targets = [targetEntity],
                EffectToExecute = effectId
            };
            
            if (Activate(activationRequest).IsValid) 
                RequestExecution();
        }
    }
    
    public void OnActorBirth(EntityNode actor)
    {
        if (PassiveCanBeTriggered() is false)
            return;
        
        var onBirthEffects = Blueprint.OnBirthEffects;
        if (onBirthEffects.IsEmpty())
            return;

        foreach (var effectId in onBirthEffects)
        {
            var activationRequest = new ActivationRequest
            {
                Targets = [actor],
                EffectToExecute = effectId
            };
            
            if (Activate(activationRequest).IsValid) 
                RequestExecution();
        }
    }
    
    private void OnUnitMoved(UnitNode unit, float movementSpent)
    {
        if (PassiveCanBeTriggered() is false)
            return;
        
        var search = GetPeriodicSearchEffect();
        if (search is null)
            return;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(PassiveNode), nameof(OnUnitMoved), 
                $"{nameof(search.TriggerFlags)} '{JsonConvert.SerializeObject(search.TriggerFlags)}', " +
                $"{nameof(unit)} '{unit}', {nameof(_periodicEffectTrackedEntities)} '{string.Join(", ", 
                    _periodicEffectTrackedEntities.Select(e => e.ToString()))}', validation " +
                $"message '{search.Validate().Message}'");
        
        if (search.TriggerFlags.Any(t => t.Equals(SearchTriggerFlag.AppliedOnEnter)) 
            && _periodicEffectTrackedEntities.Contains(unit) is false
            && search.FoundTargets.Contains(unit) 
            && search.Validate().IsValid)
        {
            HandlePeriodicEffectActivation();
        }
        
        if (search.TriggerFlags.Any(t => t.Equals(SearchTriggerFlag.RemovedOnExit))
            && _periodicEffectTrackedEntities.Contains(unit)
            && search.FoundTargets.Contains(unit) is false
            && search.Validate().IsValid)
        {
            HandlePeriodicEffectActivation();
        }
    }
    
    private void OnActionEnded(ActorNode actor)
    {
        if (PassiveCanBeTriggered() is false)
            return;
        
        var search = GetPeriodicSearchEffect();
        if (search is null)
            return;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(PassiveNode), nameof(OnActionEnded), 
                $"{nameof(search.TriggerFlags)} '{JsonConvert.SerializeObject(search.TriggerFlags)}', " +
                $"{nameof(actor)} '{actor}', validation message '{search.Validate().Message}'");
        
        if (search.TriggerFlags.Any(t => t.Equals(SearchTriggerFlag.AppliedOnEveryAction))
            && search.Validate().IsValid)
        {
            HandlePeriodicEffectActivation();
        }

        if (search.TriggerFlags.Any(t => t.Equals(SearchTriggerFlag.AppliedOnSourceAction))
            && actor.Equals(OwnerActor)
            && search.Validate().IsValid)
        {
            HandlePeriodicEffectActivation();
        }
    }

    private void OnPhaseStarted(int turn, TurnPhase phase)
    {
        if (PassiveCanBeTriggered() is false)
            return;
        
        var search = GetPeriodicSearchEffect();
        if (search is null)
            return;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(PassiveNode), nameof(OnPhaseStarted), 
                $"{nameof(search.TriggerFlags)} '{JsonConvert.SerializeObject(search.TriggerFlags)}', " +
                $"{nameof(phase)} '{phase}', validation message '{search.Validate().Message}'");
        
        if (search.TriggerFlags.Any(t => t.Equals(SearchTriggerFlag.AppliedOnPlanningPhaseStart))
            && phase.Equals(TurnPhase.Planning)
            && search.Validate().IsValid)
        {
            HandlePeriodicEffectActivation();
        }
        
        if (search.TriggerFlags.Any(t => t.Equals(SearchTriggerFlag.AppliedOnActionPhaseStart))
            && phase.Equals(TurnPhase.Action)
            && search.Validate().IsValid)
        {
            HandlePeriodicEffectActivation();
        }
    }
    
    private void OnPhaseEnded(int turn, TurnPhase phase)
    {
        if (PassiveCanBeTriggered() is false)
            return;
        
        var search = GetPeriodicSearchEffect();
        if (search is null)
            return;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(PassiveNode), nameof(OnPhaseEnded), 
                $"{nameof(search.TriggerFlags)} '{JsonConvert.SerializeObject(search.TriggerFlags)}', " +
                $"{nameof(phase)} '{phase}', validation message '{search.Validate().Message}'");
        
        if (search.TriggerFlags.Any(t => t.Equals(SearchTriggerFlag.AppliedOnPlanningPhaseEnd))
            && phase.Equals(TurnPhase.Planning)
            && search.Validate().IsValid)
        {
            HandlePeriodicEffectActivation();
        }
        
        if (search.TriggerFlags.Any(t => t.Equals(SearchTriggerFlag.AppliedOnActionPhaseEnd))
            && phase.Equals(TurnPhase.Action)
            && search.Validate().IsValid)
        {
            HandlePeriodicEffectActivation();
        }
    }

    public class ActivationRequest : IAbilityActivationRequest
    {
        public required IList<ITargetable> Targets { get; init; }
        public required EffectId EffectToExecute { get; init; }
    }

    public class PreProcessingResult : IAbilityActivationPreProcessingResult;

    public class Focus : IAbilityFocus, IEquatable<Focus>
    {
        public required IList<Guid> TargetEntities { get; init; }
        public required IList<Point> TargetTiles { get; init; }
        public required EffectId EffectToExecute { get; init; }
        
        public ActivationRequest ToActivationRequest() => new()
        {
            Targets = TargetEntities
                .Select(id => GlobalRegistry.Instance.GetEntityById(id))
                .WhereNotNull()
                .Cast<ITargetable>()
                .Concat(TargetTiles
                    .Select(p => GlobalRegistry.Instance.GetTileFromPoint(p))
                    .WhereNotNull())
                .ToList(),
            EffectToExecute = EffectToExecute
        };

        public bool Equals(Focus? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EffectToExecute.Equals(other.EffectToExecute);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Focus)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TargetEntities, TargetTiles, EffectToExecute);
        }
    }
}
