using System.Collections.Generic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    public class ExtraAttack : Behaviour
    {
        public ExtraAttack(
            BehaviourId id,
            string displayName, 
            string description,
            IList<Attacks> attackTypes,
            EndsAt? endsAt = null,
            bool canStack = false) 
            : base(
                id, 
                $"{nameof(Behaviour)}.{nameof(ExtraAttack)}", 
                displayName, 
                description, 
                endsAt ?? EndsAt.Death,
                Alignment.Positive,
                canStack)
        {
            AttackTypes = attackTypes;
        }

        public IList<Attacks> AttackTypes { get; }
    }
}
