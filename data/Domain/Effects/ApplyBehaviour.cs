using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Conditions;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Effects
{
    public class ApplyBehaviour : Effect
    {
        public ApplyBehaviour(
            EffectName name,
            IList<BehaviourName> behavioursToApply,
            Location? location = null,
            IList<Flag>? filterFlags = null,
            IList<Condition>? conditions = null) : base(name, $"{nameof(Effect)}.{nameof(ApplyBehaviour)}")
        {
            BehavioursToApply = behavioursToApply;
            Location = location ?? Location.Inherited;
            FilterFlags = filterFlags ?? new List<Flag>();
            Conditions = conditions ?? new List<Condition>();
        }

        public IList<BehaviourName> BehavioursToApply { get; }
        public Location Location { get; }
        public IList<Flag> FilterFlags { get; }
        public IList<Condition> Conditions { get; }
    }
}
