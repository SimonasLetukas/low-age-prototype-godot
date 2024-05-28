using Godot;
using low_age_data.Domain.Behaviours;

public class HighGroundNode : BehaviourNode, INodeFromBlueprint<HighGround>, IPathfindingUpdatable
{
    public const string ScenePath = @"res://app/shared/game/behaviours/HighGroundNode.tscn";
    public static HighGroundNode Instance() => (HighGroundNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static HighGroundNode InstantiateAsChild(HighGround blueprint, Node parentNode, Effects history)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }
    
    private HighGround Blueprint { get; set; }
    
    public void SetBlueprint(HighGround blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        
        EventBus.Instance.RaisePathfindingUpdated(this, true);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventBus.Instance.RaisePathfindingUpdated(this, false);
    }
}