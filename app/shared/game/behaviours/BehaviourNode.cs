using Godot;
using System;
using low_age_data.Domain.Behaviours;

public abstract class BehaviourNode : Node2D, INodeFromBlueprint<Behaviour>
{
    public event Action<BehaviourNode> Ended = delegate { };
    
    public Guid Id { get; } = Guid.NewGuid();
    public abstract Behaviour Blueprint { get; protected set; }
    public abstract Guid? OwnerActorId { get; protected set; }
    public abstract EndsAtNode CurrentDuration { get; protected set; }
    
    public virtual void SetBlueprint(Behaviour blueprint)
    {
        Blueprint = blueprint;
        CurrentDuration = EndsAtNode.InstantiateAsChild(blueprint.EndsAt, this);
        CurrentDuration.Completed += OnDurationEnded;
    }

    public override void _ExitTree()
    {
        CurrentDuration.Completed -= OnDurationEnded;
        base._ExitTree();
    }

    protected virtual void OnDurationEnded()
    {
        Ended(this);
    }
}
