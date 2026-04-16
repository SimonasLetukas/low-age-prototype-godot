using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Effects;

public partial class BehaviourNode : Node2D, INodeFromBlueprint<Behaviour>, IBehaviour
{
    public event Action<BehaviourNode> Destroyed = delegate { };

    public BehaviourId BlueprintId => Blueprint.Id;
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public string Description { get; protected set; } = null!;
    public string? SpriteLocation { get; private set; }
    public Alignment Alignment { get; private set; } = null!;
    public bool CanResetDuration { get; private set; }
    public Guid? OwnerActorId { get; protected set; }
    public EndsAtNode CurrentDuration { get; private set; } = null!;
    public HashSet<BehaviourNode> Stack { get; private set; } = [];
    public Effects History { get; protected set; } = null!;

    protected EntityNode Parent { get; set; } = null!;
    protected bool IsBeingDestroyed { get; private set; } 
    
    private Behaviour Blueprint { get; set; } = null!;
    
    private readonly IList<TriggerHandler> _triggerHandlers = [];
    
    public void SetBlueprint(Behaviour blueprint)
    {
        Blueprint = blueprint;
        Description = Blueprint.Description;
        SpriteLocation = Blueprint.Sprite;
        Alignment = Blueprint.Alignment;
        CanResetDuration = Blueprint.CanResetDuration;
        CurrentDuration = EndsAtNode.InstantiateAsChild(blueprint.EndsAt, this, Parent);
        InitializeTriggers();
        
        CurrentDuration.Completed += OnDurationEnded;
        EventBus.Instance.EntityDestroyed += OnEntityDestroyed;

        InitializeStackedBehaviours();
        CurrentDuration.ResetDuration();
    }

    public override void _ExitTree()
    {
        CurrentDuration.Completed -= OnDurationEnded;
        EventBus.Instance.EntityDestroyed -= OnEntityDestroyed;
        
        DisposeTriggers();

        base._ExitTree();
    }
    
    public EntityNode GetParentEntity() => Parent;

    public bool IsParentEntity(EntityNode entity) => Parent.InstanceId.Equals(entity.InstanceId);

    public void Remove() => EndBehaviour(true);
    
    protected virtual void EndBehaviour(bool triggersOnDeathBehaviours) => Destroy();

    private void Destroy()
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(Destroy), ToString());

        foreach (var stackedBehaviour in Stack)
        {
            stackedBehaviour.Destroyed -= OnStackedBehaviourDestroyed;
        }

        IsBeingDestroyed = true;
        Destroyed(this);
        QueueFree();
    }
    
    protected void HandleEffects(IList<EffectId> effects)
    {
        foreach (var effectId in effects)
        {
            var chain = new Effects(History, effectId, [Parent], Parent.Player, Parent);
            var validationResult = chain.ValidateLast();

            if (validationResult.IsValid is false)
            {
                if (Log.DebugEnabled)
                    Log.Info(nameof(BuffNode), nameof(HandleEffects), 
                        $"{this} failed to execute effect '{effectId}' because '{validationResult.Message}'.");
                
                continue;
            }
            
            chain.ExecuteLast();
        }
    }

    private void InitializeTriggers()
    {
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(InitializeTriggers), ToString());
        
        var highestTiles = GlobalRegistry.Instance
            .GetHighestTiles(Parent.EntityOccupyingPositions)
            .WhereNotNull()
            .ToList();
        
        foreach (var trigger in Blueprint.Triggers)
        {
            var triggerHandler = TriggerHandler
                .Setup(trigger)
                .With(Parent.Player)
                .With(highestTiles)
                .With(History)
                .With(Parent);

            triggerHandler.Triggered += OnTriggered;
            _triggerHandlers.Add(triggerHandler);
        }
    }

    private void DisposeTriggers()
    {
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(DisposeTriggers), ToString());
        
        foreach (var triggerHandler in _triggerHandlers)
        {
            triggerHandler.Triggered -= OnTriggered;
            triggerHandler.Dispose();
        }
        _triggerHandlers.Clear();
    }

    private void InitializeStackedBehaviours()
    {
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(InitializeStackedBehaviours), ToString());
        
        if (Blueprint.CanStack is false)
        {
            Stack = [this];
            return;
        }
        
        var behaviours = Parent.Behaviours.GetAll();
        foreach (var behaviour in behaviours)
        {
            if (behaviour.Blueprint.Id.Equals(Blueprint.Id) is false)
                continue;

            Stack.Add(behaviour);
            
            if (behaviour.Equals(this))
                continue;
            
            behaviour.Destroyed += OnStackedBehaviourDestroyed;
        }
        
        if (Log.DebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(InitializeStackedBehaviours), 
                $"{this} stack initialized. Current stack: " +
                $"'{string.Join(", ", Stack.Select(b => b.ToString()))}'.");
    }
    
    public void OnBehaviourAdded(BehaviourNode addedBehaviour)
    {
        if (IsBeingDestroyed)
            return;
        
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(OnBehaviourAdded), 
                $"Current: '{this}', added: '{addedBehaviour}'.");
        
        if (addedBehaviour.Equals(this) || addedBehaviour.Blueprint.Id.Equals(Blueprint.Id) is false)
            return;

        if (Blueprint.CanStack)
        {
            Stack.Add(addedBehaviour);
            addedBehaviour.Destroyed += OnStackedBehaviourDestroyed;
            
            if (Log.DebugEnabled)
                Log.Info(nameof(BehaviourNode), nameof(OnBehaviourAdded), 
                    $"{this} added item to stack ({addedBehaviour}). Current stack: " +
                    $"'{string.Join(", ", Stack.Select(b => b.ToString()))}'.");
        }
        else
        {
            addedBehaviour.EndBehaviour(false);
        }

        if (CanResetDuration is false) 
            return;
        
        if (Blueprint.CanStack)
        {
            foreach (var behaviour in Stack
                         .Where(behaviour => behaviour.CurrentDuration.DurationIsFullyReset() is false))
            {
                behaviour.CurrentDuration.ResetDuration();
            }
        }
        else
        {
            CurrentDuration.ResetDuration();
        }
    }
    
    protected virtual void OnDurationEnded(EndsAtNode duration)
    {
        if (IsBeingDestroyed)
            return;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(OnDurationEnded), ToString());
        
        EndBehaviour(true);
    }
    
    private void OnTriggered()
    {
        if (IsBeingDestroyed)
            return;
        
        var removeOnConditionsMet = Blueprint.RemoveOnConditionsMet;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(OnTriggered), 
                $"{this} conditions triggered ({nameof(removeOnConditionsMet)}: '{removeOnConditionsMet}').");
        
        HandleEffects(Blueprint.ConditionalEffects);
        
        if (removeOnConditionsMet)
            Destroy();
    }

    private void OnEntityDestroyed(EntityNode entity, EntityNode? source, bool triggersOnDeathBehaviours)
    {
        if (entity.Equals(Parent) is false)
            return;
     
        if (IsBeingDestroyed)
            return;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(OnEntityDestroyed), 
                $"{this} parent destroyed by {source}");
        
        EndBehaviour(triggersOnDeathBehaviours);
    }

    private void OnStackedBehaviourDestroyed(BehaviourNode stackedBehaviour)
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(OnStackedBehaviourDestroyed), 
                $"{this} stack item destroyed ({stackedBehaviour}). Current stack: " +
                $"'{string.Join(", ", Stack.Select(b => b.ToString()))}'.");
        
        Stack.Remove(stackedBehaviour);
        stackedBehaviour.Destroyed -= OnStackedBehaviourDestroyed;
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);

    public override string ToString() => $"{Blueprint.DisplayName} behaviour '{InstanceId}' on {Parent}";
}
