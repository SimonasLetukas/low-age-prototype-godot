using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Abilities;

public class Abilities : Node2D
{
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
                default:
                    break;
            }
        }
    }
}
