using System;
using LowAgeData.Domain.Effects;

public class EffectNode : INodeFromBlueprint<Effect>
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    
    public EntityNode Initiator { get; protected set; }
    public EntityNode Target { get; protected set; }
    public Effects History { get; protected set; }
    
    protected bool IsValidated { get; private set; }
    
    private Effect Blueprint { get; set; }

    public EffectNode(Effects history)
    {
        History = history;
    }

    public void SetBlueprint(Effect blueprint)
    {
        Blueprint = blueprint;
    }

    public void SetTarget(EntityNode entity)
    {
        Target = entity;
    }

    public virtual bool Validate()
    {
        IsValidated = true;
        return true;
    }

    public virtual bool Execute()
    {
        return IsValidated;
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}
