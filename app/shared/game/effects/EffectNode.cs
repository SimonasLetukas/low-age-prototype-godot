using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Effects;

public abstract class EffectNode(
    Effects history, 
    ITargetable? initialTarget, 
    EntityNode? initiator)
    : INodeFromBlueprint<Effect>
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    
    public EntityNode? InitiatorEntity { get; } = initiator;

    protected ITargetable? InitialTarget { get; } = initialTarget;
    protected IList<ITargetable> FoundTargets { get; private set; } = null!;
    protected Effects History { get; } = history;
    protected bool IsValidated { get; private set; }

    private Effect Blueprint { get; set; } = null!;

    public void SetBlueprint(Effect blueprint)
    {
        Blueprint = blueprint;
        
        FoundTargets = GetTargets(Blueprint.Target, InitialTarget, InitiatorEntity);
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
    
    protected abstract IEnumerable<ITargetable> GetInheritedTargets(ITargetable? initialTarget, EntityNode? initiator);

    protected virtual IList<ITargetable> GetSelfTargets(EntityNode initiator) => [initiator];
    
    protected virtual IList<ITargetable> GetEntityTargets(EntityNode initialTarget) => [initialTarget];
    
    protected virtual IList<ITargetable> GetPointTargets(Tiles.TileInstance initialTarget) => [initialTarget];
    
    protected virtual IList<ITargetable> GetSourceTargets(EntityNode sourceEntity) => [sourceEntity];
    
    protected virtual IList<ITargetable> GetOriginTargets(EntityNode originEntity) => [originEntity];
    
    protected static IList<EntityNode> GetAllEntities() => GlobalRegistry.Instance.GetEntities().ToList();
    
    private IList<ITargetable> GetTargets(Location location, ITargetable? initialTarget, EntityNode? initiator)
    {
        return location switch
        {
            _ when location.Equals(Location.Inherited) => GetInheritedTargets(initialTarget, initiator).ToList(),
            
            _ when location.Equals(Location.Self) 
                   && initiator is not null => GetSelfTargets(initiator),
            
            _ when location.Equals(Location.Entity) 
                   && initialTarget is EntityNode entityInitialTarget => GetEntityTargets(entityInitialTarget),
            
            _ when location.Equals(Location.Point) 
                   && initialTarget is Tiles.TileInstance pointInitialTarget => GetPointTargets(pointInitialTarget),
            
            _ when location.Equals(Location.Source) 
                   && History.SourceEntityOrNull is not null => GetSourceTargets(History.SourceEntityOrNull),
            
            _ when location.Equals(Location.Origin) 
                   && History.OriginEntityOrNull is not null => GetOriginTargets(History.OriginEntityOrNull),
            
            _ => []
        };
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}
