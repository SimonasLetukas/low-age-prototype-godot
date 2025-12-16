using Godot;
using System;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;

public partial class BehaviourNode : Node2D, INodeFromBlueprint<Behaviour>, IBehaviour
{
    public event Action<BehaviourNode> Ended = delegate { };
    
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public string Description { get; protected set; }
    public Alignment Alignment { get; protected set; }
    public Guid? OwnerActorId { get; protected set; }
    public EndsAtNode CurrentDuration { get; protected set; }
    
    protected EntityNode Parent { get; set; }
    protected Effects History { get; set; }
    
    private Behaviour Blueprint { get; set; }
    
    public void SetBlueprint(Behaviour blueprint)
    {
        Blueprint = blueprint;
        Description = Blueprint.Description;
        Alignment = Blueprint.Alignment;
        CurrentDuration = EndsAtNode.InstantiateAsChild(blueprint.EndsAt, this, Parent);
        CurrentDuration.Completed += OnDurationEnded;
    }

    public override void _ExitTree()
    {
        CurrentDuration.Completed -= OnDurationEnded;
        base._ExitTree();
    }
    
    public EntityNode GetParentEntity() => Parent;

    public bool IsParentEntity(EntityNode entity) => Parent.InstanceId.Equals(entity.InstanceId);
    
    protected virtual void OnDurationEnded()
    {
        Ended(this);
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}
