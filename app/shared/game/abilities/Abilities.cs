using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;

public partial class Abilities : Node2D
{
    public event Action<ActorNode, IAbilityNode, IAbilityFocus> ExecutionRequested = delegate { };
    
    private ActorNode Parent { get; set; } = null!;
    
    public override void _Ready()
    {
        base._Ready();
        Parent = (ActorNode)GetParent();
    }

    public IAbilityNode? GetById(AbilityId id) => GetChildren()
        .OfType<IAbilityNode>()
        .FirstOrDefault(a => a.Id.Equals(id));

    public IList<IAbilityNode> GetForPlanningPhase() => GetChildren()
        .OfType<IAbilityNode>()
        .Where(a => a.TurnPhase.Equals(TurnPhase.Planning))
        .ToList();

    public IList<IAbilityNode> GetForActionPhase() => GetChildren()
        .OfType<IAbilityNode>()
        .Where(a => a.TurnPhase.Equals(TurnPhase.Action))
        .ToList();
    
    public IList<PassiveNode> GetPassives() => GetChildren().OfType<PassiveNode>().ToList();
    
    public void PopulateFromBlueprint(IEnumerable<AbilityId> abilities)
    {
        var allAbilities = Data.Instance.Blueprint.Abilities;
        foreach (var abilityBlueprint in allAbilities.Join(abilities.Distinct(), ability => ability.Id, 
                     id => id, (ability, id) => ability))
        {
            switch (abilityBlueprint)
            {
                case Build buildBlueprint:
                    var build = BuildNode.InstantiateAsChild(buildBlueprint, this, Parent);
                    build.ExecutionRequested += OnExecutionRequested;
                    break;
                case Passive passiveBlueprint:
                    var passive = PassiveNode.InstantiateAsChild(passiveBlueprint, this, Parent);
                    passive.ExecutionRequested += OnExecutionRequested;
                    break;
                default:
                    break;
            }
        }
    }

    public void OnActorBirth()
    {
        foreach (var passive in GetPassives()) 
            passive.OnActorBirth(Parent);
    }
    
    private void OnExecutionRequested<TActivationRequest, TPreProcessingResult, TFocus>(
        AbilityNode<TActivationRequest, TPreProcessingResult, TFocus> abilityNode, TFocus focus) 
        where TActivationRequest : IAbilityActivationRequest 
        where TPreProcessingResult : IAbilityActivationPreProcessingResult 
        where TFocus : IAbilityFocus
    {
        ExecutionRequested(Parent, abilityNode, focus);
    }
}
