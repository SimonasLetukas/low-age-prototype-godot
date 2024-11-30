using Godot;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Behaviours;
using low_age_prototype_common.Extensions;

public class PassiveNode : AbilityNode, INodeFromBlueprint<Passive>
{
    public const string ScenePath = @"res://app/shared/game/abilities/PassiveNode.tscn";
    public static PassiveNode Instance() => (PassiveNode) GD.Load<PackedScene>(ScenePath).Instance();
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

    public void OnActorBirth(EntityNode actor)
    {
        var onBirthEffects = Blueprint.OnBirthEffects;
        if (onBirthEffects.IsEmpty())
            return;

        foreach (var effectId in onBirthEffects)
        {
            var effects = new Effects(effectId, actor);
            if (effects.ValidateLast())
                effects.ExecuteLast();
        }
    }
}
