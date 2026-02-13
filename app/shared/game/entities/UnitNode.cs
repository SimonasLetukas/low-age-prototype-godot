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
    public static UnitNode InstantiateAsChild(Unit blueprint, Node parentNode, Player player)
    {
        var unit = Instance();
        parentNode.AddChild(unit);
        unit.SetBlueprint(blueprint);
        unit.Player = player;
        
        return unit;
    }
    
    public bool IsOnHighGround { get; private set; } = false;
    public CombatStatNode Movement => Stats.Single(x => x.StatType == StatType.Movement);
    public override Tiles.TileInstance EntityPrimaryTile => GetHighestTiles([EntityPrimaryPosition])
        .WhereNotNull().Single();
    public override IList<Tiles.TileInstance> EntityOccupyingTiles => IsOnHighGround
        ? GetHighestTiles(EntityOccupyingPositions).WhereNotNull().ToList()
        : base.EntityOccupyingTiles; 

    private Unit Blueprint { get; set; } = null!;
    
    public void SetBlueprint(Unit blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        EntitySize = Vector2Int.One * Blueprint.Size;

        RestoreMovement();
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

    public override void DropDownToLowGround()
    {
        base.DropDownToLowGround();

        IsOnHighGround = false;
        UpdateVitalsPosition();
        
        Renderer.UpdateElevation(IsOnHighGround, GetTile(EntityPrimaryPosition, false)?.YSpriteOffset, null);

        var vitalsAmount = HasShields 
            ? Shields!.MaxAmount + Health!.MaxAmount
            : Health?.MaxAmount ?? 0;
        var (damage, _) = GetDamage(this, vitalsAmount / 2, DamageType.Pure);
        ReceiveDamage(this, damage, false);
    }

    public override void MoveUntilFinished(IList<Vector2> globalPositionPath, IList<Point> path)
    {
        var resultingPoint = path.Last();

        var movementCost = CalculateMovementCostFrom(path);
        Movement.CurrentAmount -= movementCost;
        ActionEconomy.Moved(movementCost, Movement.CurrentAmount >= 1);
        
        IsOnHighGround = resultingPoint.IsHighGround;
        
        base.MoveUntilFinished(globalPositionPath, path);
        
        Renderer.UpdateElevation(
            IsOnHighGround, 
            GetTile(resultingPoint.Position, resultingPoint.IsHighGround)?.YSpriteOffset, 
            GetEntitiesBelow().OrderByDescending(x => x.Renderer.ZIndex).FirstOrDefault());
        
        UpdateVitalsPosition();
    }
    
    public float GetReach()
    {
        return Movement.MaxAmount + Constants.Pathfinding.SearchIncrement;
        
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
        var meleeReach = meleeDistance > 0 ? meleeDistance + Movement.CurrentAmount : 0;
        return rangedReach > meleeReach ? rangedReach : meleeReach;
        
        // TODO logic (outside of this method too) needs to be made more intelligent, because right now this doesn't
        // take into account targeting logic (however complex it would be) and everything gets calculated through
        // pathfinding.
    }
    
    protected override void UpdateVisuals()
    {
        base.UpdateVisuals();

        if (Selected || Hovered)
            return;
        
        if (EntityState is State.Completed && ClientState.Instance.Flattened)
            SetTransparency(true);
    }

    protected override void OnPhaseStarted(int turn, TurnPhase phase)
    {
        RestoreMovement();
        base.OnPhaseStarted(turn, phase);
    }
    
    private void RestoreMovement()
    {
        Movement.CurrentAmount = Movement.MaxAmount + Constants.Pathfinding.SearchIncrement;
    }
    
    private static float CalculateMovementCostFrom(IList<Point> path)
    {
        if (path.Count <= 1)
            return 0f;

        var cost = 0f;
        for (var i = 1; i < path.Count; i++)
        {
            var previousPoint = path[i - 1];
            var currentPoint = path[i];

            var length = previousPoint.Position.IsDiagonalTo(currentPoint.Position)
                ? Constants.Pathfinding.DiagonalCost
                : 1f;
            
            cost += length * currentPoint.Weight;
        }
        
        return cost;
    }

    private List<EntityNode> GetEntitiesBelow()
    {
        var entities = new List<EntityNode>();
        foreach (var position in EntityOccupyingPositions)
        {
            var lowGroundTile = GetTile(position, false);
            var entity = lowGroundTile?.GetFirstOccupantOrNull();
            if (entity != null && entities.Any(x => x.InstanceId.Equals(entity.InstanceId)) is false)
                entities.Add(entity);
        }

        return entities;
    }
}