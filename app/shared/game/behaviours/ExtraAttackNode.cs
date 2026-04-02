using Godot;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;

public partial class ExtraAttackNode : BehaviourNode, INodeFromBlueprint<ExtraAttack>
{
    private const string ScenePath = @"res://app/shared/game/behaviours/ExtraAttackNode.tscn";
    private static ExtraAttackNode Instance() => (ExtraAttackNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static ExtraAttackNode InstantiateAsChild(ExtraAttack blueprint, Node parentNode, Effects history, 
        EntityNode parentEntity)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.History = history;
        behaviour.Parent = parentEntity;
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }
    
    private ExtraAttack Blueprint { get; set; } = null!;
    
    public void SetBlueprint(ExtraAttack blueprint)
    {
        Blueprint = blueprint;
        base.SetBlueprint(blueprint);

        if (Parent is not ActorNode actor)
            return;

        var config = actor.ActionEconomy.Config;
        foreach (var attackType in Blueprint.AttackTypes)
        {
            if (attackType.Equals(AttackType.Melee))
                config.MaxMeleeAttackActions++;

            if (attackType.Equals(AttackType.Ranged))
                config.MaxRangedAttackActions++;
        }
    }

    public override void _ExitTree()
    {
        if (IsInstanceValid(Parent) is false || Parent is not ActorNode actor)
            return;

        var config = actor.ActionEconomy.Config;
        foreach (var attackType in Blueprint.AttackTypes)
        {
            if (attackType.Equals(AttackType.Melee))
                config.MaxMeleeAttackActions--;

            if (attackType.Equals(AttackType.Ranged))
                config.MaxRangedAttackActions--;
        }
        
        base._ExitTree();
    }
}