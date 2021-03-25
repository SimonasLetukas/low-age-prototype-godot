using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;

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
                    EffectName.Leader.MenacingPresenceSearch),

                new Target(
                    AbilityName.Leader.OneForAll,
                    TurnPhase.Planning,
                    nameof(AbilityName.Leader.OneForAll).CamelCaseToWords(),
                    "Select an adjacent Obelisk and sap its energy to give all friendly units " +
                    "+2 Health. This Obelisk cannot be sapped again for 10 turns.",
                    1,
                    EffectName.Leader.OneForAllApplyBehaviourObelisk)
            };
        }
    }
}
