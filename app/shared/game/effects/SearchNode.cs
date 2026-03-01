using System.Collections.Generic;
using System.Linq;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Effects;

namespace LowAge.app.shared.game.effects;

public class SearchNode : EffectNode, INodeFromBlueprint<Search>
{
    private Search Blueprint { get; set; } = null!;
    
    public SearchNode(Search blueprint, Effects history, ITargetable? initialTarget, EntityNode? initiator) 
        : base(history, initialTarget, initiator)
    {
        SetBlueprint(blueprint);
    }
    
    public void SetBlueprint(Search blueprint)
    {
        Blueprint = blueprint;
        
        base.SetBlueprint(blueprint);
    }
    
    public override bool Execute()
    {
        if (base.Execute() is false)
            return false;

        var targets = FilterEvaluator.Apply(FoundTargets, Blueprint.Filters, new FilterContext
        {
            Initiator = InitiatorEntity,
            CurrentPlayer = Players.Instance.Current,
            Chain = History
        });
        
        foreach (var target in targets)
        {
            foreach (var effectId in Blueprint.Effects)
            {
                var chain = new Effects(History, effectId, target, InitiatorEntity);
                if (chain.ValidateLast())
                    chain.ExecuteLast();
            }
        }
        
        return true;
    }

    protected override IEnumerable<ITargetable> GetInheritedTargets(ITargetable? initialTarget, EntityNode? initiator)
    {
        if (Blueprint.Shape is LowAgeData.Domain.Common.Shape.Map 
            || initialTarget is not EntityNode initialEntityTarget)
            return GetAllEntities();
        
        return GetEntityTargets(initialEntityTarget);
    }
    
    protected override IList<ITargetable> GetSelfTargets(EntityNode initiator) 
        => GetTargetsAround(
                initiator.EntityPrimaryPosition, 
                initiator.EntitySize, 
                initiator is ActorNode actor 
                    ? actor.ActorRotation 
                    : IsometricRotation.BottomRight)
            .ToList();
    
    protected override IList<ITargetable> GetEntityTargets(EntityNode initialTarget)
        => GetTargetsAround(
                initialTarget.EntityPrimaryPosition, 
                initialTarget.EntitySize, 
                initialTarget is ActorNode actor 
                    ? actor.ActorRotation 
                    : IsometricRotation.BottomRight)
            .ToList();
    
    protected override IList<ITargetable> GetPointTargets(Tiles.TileInstance initialTarget)
        => GetTargetsAround(
                initialTarget.Position, 
                Vector2Int.One, 
                IsometricRotation.BottomRight)
            .ToList();
    
    protected override IList<ITargetable> GetSourceTargets(EntityNode sourceEntity)
        => GetTargetsAround(
                sourceEntity.EntityPrimaryPosition, 
                sourceEntity.EntitySize, 
                sourceEntity is ActorNode actor 
                    ? actor.ActorRotation 
                    : IsometricRotation.BottomRight)
            .ToList();
    
    protected override IList<ITargetable> GetOriginTargets(EntityNode originEntity)
        => GetTargetsAround(
                originEntity.EntityPrimaryPosition, 
                originEntity.EntitySize, 
                originEntity is ActorNode actor 
                    ? actor.ActorRotation 
                    : IsometricRotation.BottomRight)
            .ToList();

    private IEnumerable<ITargetable> GetTargetsAround(Vector2Int centerPoint, Vector2Int pointSize, 
        IsometricRotation pointRotation)
    {
        if (Blueprint.Shape is LowAgeData.Domain.Common.Shape.Map)
            return GetAllEntities();

        var registry = GlobalRegistry.Instance;
        var mapSize = registry.MapSize;
        var positions = Blueprint.Shape
            .ToPositions(centerPoint, mapSize, pointSize, pointRotation)
            .ToList();
        
        var tiles = Blueprint.Height switch
        {
            _ when Blueprint.Height.Equals(SearchHeight.All) => [
                .. registry.GetTiles(positions, true), 
                .. registry.GetTiles(positions, false)
            ],
            _ when Blueprint.Height.Equals(SearchHeight.OnlyLowGround) => registry.GetTiles(positions, false),
            _ when Blueprint.Height.Equals(SearchHeight.OnlyHighGround) => registry.GetTiles(positions, true),
            _ when Blueprint.Height.Equals(SearchHeight.HighestPossible) => registry.GetHighestTiles(positions),
            _ => []
        };

        var targetEntities = new HashSet<EntityNode>();
        foreach (var tile in tiles.WhereNotNull())
        {
            foreach (var occupant in tile.GetOccupants())
            {
                targetEntities.Add(occupant);
            }
        }

        return targetEntities;
    }
    
}