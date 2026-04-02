using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Effects;

public abstract class EffectNode(
    Effects history, 
    IList<ITargetable> initialTargets, 
    Player initiatorPlayer,
    EntityNode? initiatorEntity)
    : INodeFromBlueprint<Effect>
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();

    public EffectId Id { get; set; } = null!;
    public Player InitiatorPlayer { get; } = initiatorPlayer;
    public EntityNode? InitiatorEntity { get; } = initiatorEntity;
    public IList<ITargetable> FoundTargets { get; private set; } = null!;

    protected IList<ITargetable> InitialTarget { get; } = initialTargets;
    protected Effects History { get; } = history;
    protected bool IsValidated { get; private set; }

    private Effect Blueprint { get; set; } = null!;

    public void SetBlueprint(Effect blueprint)
    {
        Blueprint = blueprint;
        Id = Blueprint.Id;
    }

    public void UpdateFoundTargets() => FoundTargets = GetTargets(Blueprint.Target, InitialTarget, InitiatorEntity);

    public ValidationResult Validate()
    {
        var result = ValidationHandler
            .Validate(Blueprint.Validators)
            .With(FoundTargets)
            .With(History)
            .Handle();
        
        IsValidated = result.IsValid; 
        return result;
    }

    public abstract bool Execute();

    protected abstract IList<IFilterItem> GetFilters();
    
    protected virtual IEnumerable<ITargetable> GetInheritedTargets(IList<ITargetable> initialTargets, EntityNode? initiator)
        => initiator is null ? [] : GetSelfTargets(initiator);

    protected virtual IList<ITargetable> GetSelfTargets(EntityNode initiator) => [initiator];
    
    protected virtual IList<ITargetable> GetEntityTargets(EntityNode initialTarget) => [initialTarget];
    
    protected virtual IList<ITargetable> GetPointTargets(Tiles.TileInstance initialTarget) => [initialTarget];
    
    protected virtual IList<ITargetable> GetSourceTargets(EntityNode sourceEntity) => [sourceEntity];
    
    protected virtual IList<ITargetable> GetOriginTargets(EntityNode originEntity) => [originEntity];
    
    protected static IList<EntityNode> GetAllEntities() => GlobalRegistry.Instance.GetEntities().ToList();
    
    private IList<ITargetable> GetTargets(Location location, IList<ITargetable> initialTargets, EntityNode? initiator)
    {
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(EffectNode), nameof(GetTargets), 
                $"Getting {nameof(FoundTargets)} for effect '{Id}' with input: " +
                $"{nameof(location)} '{location}', {nameof(initialTargets)} " +
                $"'{string.Join(", ", initialTargets.Select(t => t.ToString()))}', " +
                $"{nameof(initiator)} '{initiator}', {nameof(History)} '{History}'");
        
        var foundTargets = location switch
        {
            _ when location.Equals(Location.Inherited) => History.PreviousOrNull is not null 
                ? History.PreviousOrNull.GetInheritedTargets(initialTargets, initiator).ToList() 
                : GetInheritedTargets(initialTargets, initiator).ToList(),
            
            _ when location.Equals(Location.Self) && initiator is not null 
                => GetSelfTargets(initiator),
            
            _ when location.Equals(Location.Entity) && initialTargets.Any(t => t is EntityNode) 
                => GetEntityTargets((EntityNode)initialTargets.First(t => t is EntityNode)),
            
            _ when location.Equals(Location.Point) && initialTargets.Any(t => t is Tiles.TileInstance) 
                => GetPointTargets((Tiles.TileInstance)initialTargets.First(t => t is Tiles.TileInstance)),
            
            _ when location.Equals(Location.Source) 
                   && History.SourceEntityOrNull is not null => GetSourceTargets(History.SourceEntityOrNull),
            
            _ when location.Equals(Location.Origin) 
                   && History.OriginEntityOrNull is not null => GetOriginTargets(History.OriginEntityOrNull),
            
            _ => []
        };
        
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(EffectNode), nameof(GetTargets), 
                $"Initial {nameof(FoundTargets)} for effect '{Blueprint.Id}': " +
                $"'{string.Join(", ", foundTargets.Select(t => t.ToString()))}'.");

        var filteredTargets = FilterEvaluator
            .Apply(foundTargets, GetFilters(), new FilterContext
            {
                Initiator = InitiatorEntity,
                InitiatorPlayer = InitiatorPlayer,
                Chain = History
            })
            .ToList();
        
        if (Log.DebugEnabled)
            Log.Info(nameof(EffectNode), nameof(GetTargets), 
                $"Filtered {nameof(FoundTargets)} for effect '{Id}': " +
                $"'{string.Join(", ", filteredTargets.Select(t => t.ToString()))}'.");
        
        return filteredTargets;
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}
