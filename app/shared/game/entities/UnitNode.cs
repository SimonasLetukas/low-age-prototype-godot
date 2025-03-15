using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using MultipurposePathfinding;

public sealed partial class UnitNode : ActorNode, INodeFromBlueprint<Unit>
{
    private const string ScenePath = @"res://app/shared/game/entities/UnitNode.tscn";
    public static UnitNode Instance() => (UnitNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static UnitNode InstantiateAsChild(Unit blueprint, Node parentNode, Player player,
        Func<Vector2Int, bool, Tiles.TileInstance?> getTile, 
        Func<IList<Vector2Int>, IList<Tiles.TileInstance?>> getHighestTiles)
    {
        var unit = Instance();
        parentNode.AddChild(unit);
        unit.SetBlueprint(blueprint);
        unit.Player = player;
        unit.GetTile = getTile;
        unit.GetHighestTiles = getHighestTiles;
        
        return unit;
    }
    
    public bool IsOnHighGround { get; private set; } = false;
    public float Movement { get; private set; }
    public override IList<Tiles.TileInstance> EntityOccupyingTiles => IsOnHighGround
        ? GetHighestTiles(EntityOccupyingPositions).WhereNotNull().ToList()
        : base.EntityOccupyingTiles; 

    private Unit Blueprint { get; set; } = null!;
    
    public void SetBlueprint(Unit blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        EntitySize = Vector2Int.One * Blueprint.Size;
        Movement = HasStat(StatType.Movement)
            ? Stats.First(x => x.StatType.Equals(StatType.Movement)).CurrentAmount 
              + Constants.Pathfinding.SearchIncrement
            : 0;
                                  
        Renderer.Initialize(this, true);
        UpdateSprite();
        UpdateVitalsPosition();
    }

    public override bool CanBeMovedThroughAt(Point point, Team forTeam) 
        => Player.Team.IsAllyTo(forTeam) && base.CanBeMovedThroughAt(point, forTeam);

    public override List<Tiles.TileInstance> GetMeleeAttackTargetTiles(Vector2Int mapSize, 
        IEnumerable<Point> availablePoints)
    {
        var availablePointIds = availablePoints.Select(x => x.Id).ToHashSet();
        var tiles = base.GetMeleeAttackTargetTiles(mapSize, []);
        return IsOnHighGround 
            ? tiles.Where(tile => availablePointIds.Contains(tile.Point.Id)).ToList()
            : tiles.Where(tile => tile.Point.IsLowGround 
                                  || (tile.Point.IsHighGround && availablePointIds.Contains(tile.Point.Id)))
                .ToList();
    }

    public override List<Tiles.TileInstance> GetRangedAttackTargetTiles(Vector2Int mapSize)
    {
        var tiles = base.GetRangedAttackTargetTiles(mapSize);
        var entitiesBelow = IsOnHighGround ? GetEntitiesBelow() : [];
        var filteredTiles = new HashSet<Tiles.TileInstance>();
        foreach (var tile in entitiesBelow.SelectMany(entity => entity.EntityOccupyingPositions, 
                     (_, position) => GetTile(position, false)).OfType<Tiles.TileInstance>()) 
            filteredTiles.Add(tile);

        return tiles.Except(filteredTiles).ToList();
    }

    protected override void UpdateVisuals()
    {
        base.UpdateVisuals();

        if (Selected || Hovered)
            return;
        
        if (EntityState is State.Completed && ClientState.Instance.Flattened)
            SetTransparency(true);
    }

    public override void MoveUntilFinished(List<Vector2> globalPositionPath, Point resultingPoint)
    {
        IsOnHighGround = resultingPoint.IsHighGround;
        UpdateVitalsPosition();
        
        base.MoveUntilFinished(globalPositionPath, resultingPoint);
        
        Renderer.UpdateElevation(
            IsOnHighGround, 
            GetTile(resultingPoint.Position, resultingPoint.IsHighGround)?.YSpriteOffset, 
            GetEntitiesBelow().OrderByDescending(x => x.Renderer.ZIndex).FirstOrDefault());
    }
    
    public float GetReach()
    {
        return Movement;
        
        var blueprint = GetActorBlueprint();
        
        // TODO: take from current max range (in case it is modified by behaviours)
        
        var meleeAttack = (AttackStat)blueprint.Statistics.FirstOrDefault(x =>
            x is AttackStat attackStat
            && attackStat.AttackType.Equals(LowAgeData.Domain.Common.AttackType.Melee));
        var meleeDistance = meleeAttack?.MaximumDistance ?? 0;
        
        var rangedAttack = (AttackStat)blueprint.Statistics.FirstOrDefault(x =>
            x is AttackStat attackStat
            && attackStat.AttackType.Equals(LowAgeData.Domain.Common.AttackType.Ranged));
        var rangedDistance = rangedAttack?.MaximumDistance ?? 0;

        var rangedReach = rangedDistance > 0 ? rangedDistance + 1 : 0;
        var meleeReach = meleeDistance > 0 ? meleeDistance + Movement : 0;
        return rangedReach > meleeReach ? rangedReach : meleeReach;
        
        // TODO logic (outside of this method too) needs to be made more intelligent, because right now this doesn't
        // take into account targeting logic (however complex it would be) and everything gets calculated through
        // pathfinding.
    }

    private List<EntityNode> GetEntitiesBelow()
    {
        var entities = new List<EntityNode>();
        foreach (var position in EntityOccupyingPositions)
        {
            var lowGroundTile = GetTile(position, false);
            var entity = lowGroundTile?.Occupants.FirstOrDefault();
            if (entity != null && entities.Any(x => x.InstanceId.Equals(entity.InstanceId)) is false)
                entities.Add(entity);
        }

        return entities;
    }
}