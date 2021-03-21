using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;

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
                    new List<EffectName>
                    {
                        EffectName.Leader.AllForOnePlayerLoses
                    })
            };
        }
    }
}
