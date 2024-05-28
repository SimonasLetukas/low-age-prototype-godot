using Godot;
using System;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Common;

public class BehaviourNode : Node2D, INodeFromBlueprint<Behaviour>
{
    public event Action<BehaviourNode> Ended = delegate { };
    
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public string Description { get; protected set; }
    public Alignment Alignment { get; protected set; }
    public Guid? OwnerActorId { get; protected set; }
    public EndsAtNode CurrentDuration { get; protected set; }
    
    protected Effects History { get; set; }
    
    private Behaviour Blueprint { get; set; }
    
    public void SetBlueprint(Behaviour blueprint)
    {
        Blueprint = blueprint;
        Description = Blueprint.Description;
        Alignment = Blueprint.Alignment;
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
