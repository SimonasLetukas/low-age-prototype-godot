using Godot;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Behaviours;
using LowAgeCommon.Extensions;

public partial class PassiveNode : AbilityNode<
        PassiveNode.ActivationRequest, 
        PassiveNode.PreProcessingResult, 
        PassiveNode.Focus>, 
    INodeFromBlueprint<Passive>
{
    private const string ScenePath = @"res://app/shared/game/abilities/PassiveNode.tscn";
    private static PassiveNode Instance() => (PassiveNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static PassiveNode InstantiateAsChild(Passive blueprint, Node parentNode, ActorNode owner)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.SetBlueprint(blueprint);
        ability.OwnerActor = owner;
        return ability;
    }
    
    private Passive Blueprint { get; set; } = null!;
    
    public void SetBlueprint(Passive blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }

    public BehaviourId? GetOnBuildBehaviourOrDefault()
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
    
    protected override ValidationResult ValidateActivation(ActivationRequest request)
    {
        throw new System.NotImplementedException();
    }

    protected override Focus CreateFocus(ActivationRequest activationRequest, PreProcessingResult? preProcessingResult)
    {
        throw new System.NotImplementedException();
    }
    
    protected override void Complete(Focus focus)
    {
        throw new System.NotImplementedException();
    }
    
    public class ActivationRequest : IAbilityActivationRequest
    {
    }

    public class PreProcessingResult : IAbilityActivationPreProcessingResult
    {
    }

    public class Focus : IAbilityFocus
    {
    }
}
