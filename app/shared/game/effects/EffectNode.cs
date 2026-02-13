using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Effects;

public class EffectNode : INodeFromBlueprint<Effect>
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    
    public Player? InitiatorPlayer { get; }
    public EntityNode? InitiatorEntity { get; }
    public IList<EntityNode> Targets { get; protected set; } = null!;
    public Effects History { get; protected set; }
    
    protected bool IsValidated { get; private set; }

    private Effect Blueprint { get; set; } = null!;

    public EffectNode(Effects history, EntityNode initiator)
    {
        History = history;
        InitiatorEntity = initiator;
    }

    public EffectNode(Effects history, Player initiator)
    {
        History = history;
        InitiatorPlayer = initiator;
    }

    public void SetBlueprint(Effect blueprint)
    {
        Blueprint = blueprint;
        
        if (InitiatorEntity is not null)
            Targets = GetTargets(Blueprint.Target, InitiatorEntity);
        
        if (InitiatorPlayer is not null)
            Targets = GetTargets(Blueprint.Target, InitiatorPlayer);
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
    
    protected virtual IList<EntityNode> GetInheritedTargets(EntityNode initiator) => [initiator];
    
    protected virtual IList<EntityNode> GetInheritedTargets(Player initiator) => GetAllPlayerEntities(initiator);
    
    protected static IList<EntityNode> GetAllPlayerEntities(Player player) 
        => GlobalRegistry.Instance.GetEntitiesByPlayer(player).ToList();
    
    // TODO instead of these different overloads, try to introduce ITargetable to Tile, Player, Entity
    private IList<EntityNode> GetTargets(Location location, EntityNode initiator)
    {
        return location switch
        {
            _ when location.Equals(Location.Inherited) => GetInheritedTargets(initiator),
            _ when location.Equals(Location.Self) => [initiator],
            _ when location.Equals(Location.Source) => [History.SourceEntityOrNull ?? initiator],
            _ when location.Equals(Location.Origin) => [History.OriginEntityOrNull ?? initiator],
            _ => throw new NotImplementedException($"{nameof(Effects)}.{nameof(GetTargets)}: Could not find " +
                                                   $"{nameof(Location)} of value '{location}'")
        };
    }

    private IList<EntityNode> GetTargets(Location location, Player initiator)
    {
        return location switch
        {
            _ when location.Equals(Location.Inherited) => GetInheritedTargets(initiator),
            _ when location.Equals(Location.Self) => GetAllPlayerEntities(initiator),
            _ when location.Equals(Location.Source) => GetAllPlayerEntities(History.SourcePlayerOrNull 
                                                                            ?? History.SourceEntityOrNull?.Player 
                                                                            ?? initiator),
            _ when location.Equals(Location.Origin) => GetAllPlayerEntities(History.OriginPlayerOrNull 
                                                                            ?? History.OriginEntityOrNull?.Player 
                                                                            ?? initiator),
            _ => throw new NotImplementedException($"{nameof(Effects)}.{nameof(GetTargets)}: Could not find " +
                                                   $"{nameof(Location)} of value '{location}'")
        };
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}
