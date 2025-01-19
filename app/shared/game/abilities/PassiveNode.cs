using Godot;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Behaviours;

public partial class PassiveNode : AbilityNode, INodeFromBlueprint<Passive>
{
    public const string ScenePath = @"res://app/shared/game/abilities/PassiveNode.tscn";
    public static PassiveNode Instance() => (PassiveNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static PassiveNode InstantiateAsChild(Passive blueprint, Node parentNode)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.SetBlueprint(blueprint);
        return ability;
    }
    
    private Passive Blueprint { get; set; }
    
    public void SetBlueprint(Passive blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }

    public BehaviourId GetOnBuildBehaviourOrDefault()
    {
        return Blueprint.OnBuildBehaviour;
    }
}
