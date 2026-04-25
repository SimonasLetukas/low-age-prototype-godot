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
    private readonly HashSet<EntityNode> _periodicEffectTrackedEntities = [];
    
    public void SetBlueprint(Passive blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        _targetArea = GetPeriodicSearchEffect()?.Shape;

        if (Blueprint.PeriodicSearchEffect is not null)
            Registry.TrackedEntitiesByPassiveAbility[this] = _periodicEffectTrackedEntities;

        EventBus.Instance.EntityDestroyed += OnEntityDestroyed;
        EventBus.Instance.UnitMoved += OnUnitMoved;
        EventBus.Instance.ActionEnded += OnActionEnded;
        EventBus.Instance.PhaseStarted += OnPhaseStarted;
        EventBus.Instance.PhaseEnded += OnPhaseEnded;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.EntityDestroyed -= OnEntityDestroyed;
        EventBus.Instance.UnitMoved -= OnUnitMoved;
        EventBus.Instance.ActionEnded -= OnActionEnded;
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        EventBus.Instance.PhaseEnded -= OnPhaseEnded;

        if (Registry.TrackedEntitiesByPassiveAbility.ContainsKey(this))
            Registry.TrackedEntitiesByPassiveAbility.Remove(this);
        
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
    
    public IList<DamageNode> GetValidOnHitDamageEffects(AttackType attackType, EntityNode targetEntity)
    {
        if (Blueprint.OnHitAttackTypes.Contains(attackType) is false)
            return [];

        var result = new List<DamageNode>();
        foreach (var effectId in Blueprint.OnHitEffects)
        {
            var activationRequest = new ActivationRequest
            {
                Targets = [targetEntity],
                EffectToExecute = effectId
            };
            
            var effect = GetEffect(activationRequest);
            
            if (effect.Last is not DamageNode damage)
                continue;
            
            if (effect.ValidateLast().IsValid is false)
                continue;
            
            result.Add(damage);
        }
        
        return result;
    }

    public override void SetDisabled(bool disabled)
    {
        var previousState = Disabled;
        
        base.SetDisabled(disabled);

        if (previousState == Disabled)
            return;

        if (Disabled)
        {
            HandlePeriodicEffectReset();
            return;
        }

        HandlePeriodicEffectActivation();
    }

    protected override ValidationResult ValidateActivation(ActivationRequest request) => AbilityValidator.With([
            new AbilityValidator.PlayerHasResearch
            {
                Player = OwnerActor.Player,
                ResearchNeeded = Blueprint.ResearchNeeded,
            },
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

        HandlePostPeriodicEffectExecution(effect);
    }

    private void HandlePostPeriodicEffectExecution(Effects effect)
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
            Log.Info(nameof(PassiveNode), nameof(HandlePostPeriodicEffectExecution), 
                $"Found new tracked entities '{string.Join(", ", newTrackedEntities.Select(e => 
                    e.ToString()))}', current tracked entities '{string.Join(", ", _periodicEffectTrackedEntities
                    .Select(e => e.ToString()))}'.");

        UpdatePeriodicEffectTrackedEntities(newTrackedEntities, searchNode);
    }

    private void UpdatePeriodicEffectTrackedEntities(HashSet<EntityNode> newTrackedEntities, SearchNode searchNode)
    {
        foreach (var trackedEntity in _periodicEffectTrackedEntities)
        {
            if (newTrackedEntities.Contains(trackedEntity) 
                || EntityIsTrackedByAnotherPassive(trackedEntity)) 
                continue;
            
            var behavioursToRemove = trackedEntity.Behaviours.GetAll()
                .Where(b => b.History.First.Id.Equals(searchNode.Id))
                .ToList();
            
            if (Log.DebugEnabled)
                Log.Info(nameof(PassiveNode), nameof(HandlePostPeriodicEffectExecution), 
                    $"Removing behaviours: {string.Join(", ", behavioursToRemove.Select(b => 
                        b.ToString()))}");
                
            foreach (var behaviour in behavioursToRemove) 
                behaviour.Remove();
        }

        _periodicEffectTrackedEntities.Clear();
        _periodicEffectTrackedEntities.UnionWith(newTrackedEntities);
    }

    private bool EntityIsTrackedByAnotherPassive(EntityNode entity)
    {
        var passivesAlsoTrackingEntity = Registry.TrackedEntitiesByPassiveAbility
            .Where(kvp => kvp.Value.Contains(entity))
            .Select(kvp => kvp.Key)
            .ToList();

        if (passivesAlsoTrackingEntity.IsEmpty())
            return false;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(PassiveNode), nameof(EntityIsTrackedByAnotherPassive), 
                $"Entity '{entity}' is tracked by '{string.Join(", ", passivesAlsoTrackingEntity
                    .Select(p => JsonConvert.SerializeObject(new
                    {
                        p.Id, p.InstanceId, OwnerActor = p.OwnerActor.ToString() 
                    })))}'.");

        return passivesAlsoTrackingEntity
            .Any(passiveNode => passiveNode.Id.Equals(Id) 
                                && passiveNode.InstanceId.Equals(InstanceId) is false);
    }

    private void HandlePeriodicEffectReset()
    {
        var searchEffect = GetPeriodicSearchEffect();
        if (searchEffect is not null)
            UpdatePeriodicEffectTrackedEntities([], searchEffect);
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
        
        var (activationResult, focus) = Activate(activationRequest);
        if (activationResult.IsValid && focus is not null) 
            RequestExecution(focus);
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
                                            && IsActorOwnedByCurrentPlayer()
                                            && Disabled is false;

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
            
            var (activationResult, focus) = Activate(activationRequest);
            
            if (Log.DebugEnabled)
                Log.Info(nameof(PassiveNode), nameof(OnAttackExecuted), 
                    $"Activating on-hit effect '{effectId}', source '{OwnerActor}', target '{targetEntity}', " +
                    $"result '{activationResult.IsValid}', message '{activationResult.Message}'.");
            
            if (activationResult.IsValid && focus is not null) 
                RequestExecution(focus);
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
            
            var (activationResult, focus) = Activate(activationRequest);
            if (activationResult.IsValid && focus is not null) 
                RequestExecution(focus);
        }
    }
    
    private void OnEntityDestroyed(EntityNode entity, EntityNode? source, bool triggersOnDeathBehaviours)
    {
        if (entity.Equals(OwnerActor) is false)
            return;
        
        HandlePeriodicEffectReset();
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
