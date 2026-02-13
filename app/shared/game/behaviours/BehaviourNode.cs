using Godot;
using System;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;

public partial class BehaviourNode : Node2D, INodeFromBlueprint<Behaviour>, IBehaviour
{
    public event Action<BehaviourNode> Destroyed = delegate { };
    
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public string Description { get; protected set; } = null!;
    public Alignment Alignment { get; protected set; } = null!;
    public Guid? OwnerActorId { get; protected set; }
    public EndsAtNode CurrentDuration { get; protected set; } = null!;

    protected bool DebugEnabled => false;
    protected EntityNode Parent { get; set; } = null!;
    protected Effects History { get; set; } = null!;

    private Behaviour Blueprint { get; set; } = null!;
    
    public void SetBlueprint(Behaviour blueprint)
    {
        Blueprint = blueprint;
        Description = Blueprint.Description;
        Alignment = Blueprint.Alignment;
        CurrentDuration = EndsAtNode.InstantiateAsChild(blueprint.EndsAt, this, Parent);
        CurrentDuration.Completed += OnDurationEnded;
        
        CurrentDuration.ResetDuration();
    }

    public override void _ExitTree()
    {
        CurrentDuration.Completed -= OnDurationEnded;
        base._ExitTree();
    }
    
    public EntityNode GetParentEntity() => Parent;

    public bool IsParentEntity(EntityNode entity) => Parent.InstanceId.Equals(entity.InstanceId);
    
    protected virtual void OnDurationEnded(EndsAtNode duration)
    {
        Destroy();
    }

    protected void Destroy()
    {
        Destroyed(this);
        QueueFree();
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}
