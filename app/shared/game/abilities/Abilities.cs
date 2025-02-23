using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Abilities;

public partial class Abilities : Node2D
{
    private ActorNode Parent { get; set; } = null!;
    
    public override void _Ready()
    {
        base._Ready();
        Parent = (ActorNode)GetParent();
    }
    
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
                    BuildNode.InstantiateAsChild(buildBlueprint, this, Parent);
                    break;
                case Passive passiveBlueprint:
                    PassiveNode.InstantiateAsChild(passiveBlueprint, this, Parent);
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
}
