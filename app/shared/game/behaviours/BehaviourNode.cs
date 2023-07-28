using Godot;
using System;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Shared.Durations;

public abstract class BehaviourNode : Node2D
{
    public event Action<BehaviourNode> Ended = delegate { };
    
    public abstract Guid Id { get; protected set; }
    public abstract Behaviour Blueprint { get; protected set; }
    public abstract Guid? OwnerActorId { get; protected set; }
    public abstract EndsAtNode CurrentDuration { get; protected set; }
    
    protected readonly PackedScene EndsAtNodeScene = GD.Load<PackedScene>(EndsAtNode.ScenePath);

    public virtual void SetBlueprint(Behaviour blueprint)
    {
        Blueprint = blueprint;
        CurrentDuration = InstantiateEndsAtNode(blueprint.EndsAt);
        CurrentDuration.Completed += OnDurationEnded;
    }

    public override void _ExitTree()
    {
        CurrentDuration.Completed -= OnDurationEnded;
        base._ExitTree();
    }

    protected virtual EndsAtNode InstantiateEndsAtNode(EndsAt blueprint)
    {
        var endsAt = (EndsAtNode) EndsAtNodeScene.Instance();
        AddChild(endsAt);
        endsAt.SetBlueprint(blueprint);
        return endsAt;
    }

    protected virtual void OnDurationEnded()
    {
        Ended(this);
    }
}
