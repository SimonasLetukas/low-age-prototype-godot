using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities.Actors.Units;

public class UnitNode : ActorNode, INodeFromBlueprint<Unit>
{
    public const string ScenePath = @"res://app/shared/game/entities/UnitNode.tscn";
    public static UnitNode Instance() => (UnitNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static UnitNode InstantiateAsChild(Unit blueprint, Node parentNode)
    {
        var unit = Instance();
        parentNode.AddChild(unit);
        unit.SetBlueprint(blueprint);
        return unit;
    }
    
    public bool IsOnHighGround { get; protected set; } = false;
    public float Movement { get; protected set; }
    
    private Unit Blueprint { get; set; }
    
    public void SetBlueprint(Unit blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        EntitySize = Vector2.One * Blueprint.Size;
        Movement = CurrentStats.First(x => 
                x.Blueprint is CombatStat combatStat
                && combatStat.CombatType.Equals(StatType.Movement))
            .CurrentValue + 0.5f; // TODO 0.5 is added for smoother corners to align with circles from IShape,
                                  // decide if this is needed. Argument against: not moving straight diagonally is 
                                  // more cost effective, which is not intuitive for the player. This bonus could be
                                  // added for units with 1 movement only.
                                  
        Renderer.Initialize(this, true);
        UpdateSprite();
        UpdateVitalsPosition();
    }

    public float GetReach()
    {
        return Movement;
        
        var blueprint = GetActorBlueprint();
        
        // TODO: take from current max range (in case it is modified by behaviours)
        
        var meleeAttack = (AttackStat)blueprint.Statistics.FirstOrDefault(x =>
            x is AttackStat attackStat
            && attackStat.AttackType.Equals(Attacks.Melee));
        var meleeDistance = meleeAttack?.MaximumDistance ?? 0;
        
        var rangedAttack = (AttackStat)blueprint.Statistics.FirstOrDefault(x =>
            x is AttackStat attackStat
            && attackStat.AttackType.Equals(Attacks.Ranged));
        var rangedDistance = rangedAttack?.MaximumDistance ?? 0;

        var rangedReach = rangedDistance > 0 ? rangedDistance + 1 : 0;
        var meleeReach = meleeDistance > 0 ? meleeDistance + Movement : 0;
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

    public override void MoveUntilFinished(List<Vector2> globalPositionPath, Point resultingPoint)
    {
        IsOnHighGround = resultingPoint.IsHighGround;
        UpdateVitalsPosition();
        
        base.MoveUntilFinished(globalPositionPath, resultingPoint);
        
        Renderer.UpdateElevation(
            IsOnHighGround, 
            GetTile(resultingPoint.Position, resultingPoint.IsHighGround).YSpriteOffset, 
            GetEntitiesBelow().OrderByDescending(x => x.Renderer.ZIndex).FirstOrDefault());
    }

    private IList<EntityNode> GetEntitiesBelow()
    {
        var entities = new List<EntityNode>();
        foreach (var position in EntityOccupyingPositions)
        {
            var lowGroundTile = GetTile(position, false);
            var entity = lowGroundTile.Occupants.FirstOrDefault();
            if (entity != null && entities.Any(x => x.InstanceId.Equals(entity.InstanceId)) is false)
                entities.Add(entity);
        }

        return entities;
    }
}