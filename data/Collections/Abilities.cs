using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Effects;

namespace low_age_data.Collections
{
    public static class Abilities
    {
        public static List<Ability> Get()
        {
            return new List<Ability>
            {
                new Passive(
                    AbilityName.Leader.AllForOne,
                    nameof(AbilityName.Leader.AllForOne).CamelCaseToWords(),
                    "Revelators faction loses if Leader dies.",
                    EffectName.Leader.AllForOneApplyBehaviour),
                new Passive(
                    AbilityName.Leader.MenacingPresence,
                    nameof(AbilityName.Leader.MenacingPresence).CamelCaseToWords(),
                    "All friendly and enemy units that enter 6 Attack Distance around Leader " +
                    "have their Melee Damage and Ranged Damage reduced by 2 (total minimum of 1).",
                    EffectName.Leader.MenacingPresenceSearch)
            };
        }
    }
}
