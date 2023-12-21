using System.Collections.Generic;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Durations;

namespace low_age_data.Domain.Behaviours
{
    public class ExtraAttack : Behaviour
    {
        public ExtraAttack(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            IList<Attacks> attackTypes,
            EndsAt? endsAt = null,
            bool canStack = false) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                endsAt ?? EndsAt.Death,
                Alignment.Positive,
                canStack)
        {
            AttackTypes = attackTypes;
        }

        public IList<Attacks> AttackTypes { get; }
    }
}
