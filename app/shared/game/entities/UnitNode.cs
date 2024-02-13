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
        
        AdjustSpriteOffset();
    }
}