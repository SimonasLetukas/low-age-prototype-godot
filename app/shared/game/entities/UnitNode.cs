using Godot;
using low_age_data.Domain.Entities.Actors.Units;

public class UnitNode : ActorNode, INodeFromBlueprint<Unit>
{
    public const string ScenePath = @"res://app/shared/game/entities/UnitNode.tscn";
    public static UnitNode Instance() => (UnitNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static UnitNode InstantiateAsChild(Unit blueprint, Node parentNode)
    {
        var structure = Instance();
        parentNode.AddChild(structure);
        structure.SetBlueprint(blueprint);
        return structure;
    }
    
    private Unit Blueprint { get; set; }
    
    public void SetBlueprint(Unit blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }
}