using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities.Actors.Units;

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
                    BehaviourName.Leader.AllForOneBuff,
                    UnitName.Leader,
                    UnitName.Leader)
            };
        }
    }
}
