using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class ModifyAbility : Effect
    {
        public ModifyAbility(
            EffectId id,
            AbilityId abilityToModify,
            AbilityId modifiedAbility,
            IList<Validator>? validators = null) : base(id, $"{nameof(Effect)}.{nameof(ModifyAbility)}", validators ?? new List<Validator>())
        {
            AbilityToModify = abilityToModify;
            ModifiedAbility = modifiedAbility;
        }

        public AbilityId AbilityToModify { get; }
        public AbilityId ModifiedAbility { get; }
    }
}
