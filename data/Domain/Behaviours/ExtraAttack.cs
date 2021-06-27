using System.Collections.Generic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    public class ExtraAttack : Behaviour
    {
        public ExtraAttack(
            BehaviourName name,
            string displayName, 
            string description,
            IList<Attacks> attackTypes,
            EndsAt? endsAt = null,
            bool canStack = false) : base(name, $"{nameof(Behaviour)}.{nameof(ExtraAttack)}", displayName, description, endsAt ?? EndsAt.Death)
        {
            AttackTypes = attackTypes;
            CanStack = canStack;
        }

        public IList<Attacks> AttackTypes { get; }
        public bool CanStack { get; }
    }
}
