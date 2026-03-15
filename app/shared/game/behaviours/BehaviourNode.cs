using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;

public partial class BehaviourNode : Node2D, INodeFromBlueprint<Behaviour>, IBehaviour
{
    public event Action<BehaviourNode> Destroyed = delegate { };

    public BehaviourId BlueprintId => Blueprint.Id;
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public string Description { get; protected set; } = null!;
    public string? SpriteLocation { get; private set; }
    public Alignment Alignment { get; protected set; } = null!;
    public bool CanResetDuration { get; private set; }
    public Guid? OwnerActorId { get; protected set; }
    public EndsAtNode CurrentDuration { get; protected set; } = null!;
    public HashSet<BehaviourNode> Stack = [];

    protected EntityNode Parent { get; set; } = null!;
    protected Effects History { get; set; } = null!;
    
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

    public void OnBehaviourAdded(BehaviourNode addedBehaviour)
    {
        if (addedBehaviour.Equals(this) || addedBehaviour.Blueprint.Id.Equals(Blueprint.Id) is false)
            return;

        if (Blueprint.CanStack)
        {
            Stack.Add(addedBehaviour);
            addedBehaviour.Destroyed += OnStackedBehaviourDestroyed;
            
            if (Log.DebugEnabled)
                Log.Info(nameof(BehaviourNode), nameof(OnBehaviourAdded), 
                    $"Behaviour '{Blueprint.Id}' on {Parent}: added to stack (behaviour on " +
                    $"{addedBehaviour.Parent}). Current stack: {Stack.Count}.");
        }
        else
        {
            addedBehaviour.Destroy();
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

    protected virtual void EndBehaviour() => Destroy();
    
    protected virtual void OnDurationEnded(EndsAtNode duration) => EndBehaviour();
    
    private void Destroy()
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(Destroy), 
                $"Behaviour '{Blueprint.Id}' on {Parent} destroy called.");

        foreach (var stackedBehaviour in Stack)
        {
            stackedBehaviour.Destroyed -= OnStackedBehaviourDestroyed;
        }
        
        Destroyed(this);
        QueueFree();
    }

    private void InitializeTriggers()
    {
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
        foreach (var triggerHandler in _triggerHandlers)
        {
            triggerHandler.Triggered -= OnTriggered;
            triggerHandler.Dispose();
        }
        _triggerHandlers.Clear();
    }

    private void InitializeStackedBehaviours()
    {
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
                $"Behaviour '{Blueprint.Id}' on {Parent} stack initialized. Current stack: {Stack.Count}.");
    }
    
    private void OnTriggered()
    {
        if (Blueprint.RemoveOnConditionsMet)
            Destroy();
    }

    private void OnEntityDestroyed(EntityNode entity, EntityNode? source)
    {
        if (entity.Equals(Parent) is false)
            return;
        
        EndBehaviour();
    }

    private void OnStackedBehaviourDestroyed(BehaviourNode stackedBehaviour)
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(BehaviourNode), nameof(OnStackedBehaviourDestroyed), 
                $"Behaviour '{Blueprint.Id}' on {Parent} stack destroyed (behaviour on " +
                $"{stackedBehaviour.Parent}). Current stack: {Stack.Count}.");
        
        Stack.Remove(stackedBehaviour);
        stackedBehaviour.Destroyed -= OnStackedBehaviourDestroyed;
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}
