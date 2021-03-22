using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Behaviours.Buffs;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Collections
{
    public static class Behaviours
    {
        public static List<Behaviour> Get()
        {
            return new List<Behaviour>
            {
                new Buff(
                    BehaviourName.Leader.AllForOneBuff,
                    nameof(BehaviourName.Leader.AllForOneBuff).CamelCaseToWords(),
                    "Revelators faction loses when this unit dies.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Leader.AllForOnePlayerLoses
                    }),
                new Buff(
                    BehaviourName.Leader.MenacingPresenceBuff,
                    nameof(BehaviourName.Leader.MenacingPresenceBuff).CamelCaseToWords(),
                    "Melee and Range Damage for this unit is reduced by 2.",
                    new List<Modification>
                    {
                        new AttackModification(
                            Change.Remove, 
                            2f,
                            Attacks.Melee, 
                            AttackAttribute.MaxAmount),
                        new AttackModification(
                            Change.Remove, 
                            2f,
                            Attacks.Ranged,
                            AttackAttribute.MaxAmount)
                    },
                    null,
                    null,
                    EndsAt.EndOf.This.Action,
                    Alignment.Negative)
            };
        }
    }
}
