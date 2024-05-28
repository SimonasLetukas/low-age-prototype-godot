using Godot;
using low_age_data.Domain.Behaviours;

public class AscendableNode : BehaviourNode, INodeFromBlueprint<Ascendable>, IPathfindingUpdatable
{
    public const string ScenePath = @"res://app/shared/game/behaviours/AscendableNode.tscn";
    public static AscendableNode Instance() => (AscendableNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static AscendableNode InstantiateAsChild(Ascendable blueprint, Node parentNode, Effects history)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }
    
    private Ascendable Blueprint { get; set; }
    
    public void SetBlueprint(Ascendable blueprint)
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
