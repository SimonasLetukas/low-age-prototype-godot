using Godot;
using System;
using low_age_data.Domain.Behaviours;

public class BehaviourNode : Node2D, INodeFromBlueprint<Behaviour>
{
    public event Action<BehaviourNode> Ended = delegate { };
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? OwnerActorId { get; protected set; }
    public EndsAtNode CurrentDuration { get; protected set; }
    
    private Behaviour Blueprint { get; set; }
    
    public void SetBlueprint(Behaviour blueprint)
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
