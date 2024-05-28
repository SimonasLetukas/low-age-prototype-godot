using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Abilities;

public class Abilities : Node2D
{
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
                    BuildNode.InstantiateAsChild(buildBlueprint, this);
                    break;
                case Passive passiveBlueprint:
                    PassiveNode.InstantiateAsChild(passiveBlueprint, this);
                    break;
                default:
                    break;
            }
        }
    }

    public void OnActorBirth(EntityNode actor)
    {
        foreach (var passive in GetPassives()) 
            passive.OnActorBirth(actor);
    }
}
