using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class ModifyAbility : Effect
    {
        public ModifyAbility(
            EffectName name,
            AbilityName abilityToModify,
            AbilityName modifiedAbility,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(ModifyAbility)}", validators ?? new List<Validator>())
        {
            AbilityToModify = abilityToModify;
            ModifiedAbility = modifiedAbility;
        }

        public AbilityName AbilityToModify { get; }
        public AbilityName ModifiedAbility { get; }
    }
}
