using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;
using System.Collections.Generic;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class ApplyBehaviour : Effect
    {
        public ApplyBehaviour(
            EffectName name,
            IList<BehaviourName> behavioursToApply,
            Location? location = null,
            IList<Flag>? filterFlags = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(ApplyBehaviour)}", validators ?? new List<Validator>())
        {
            BehavioursToApply = behavioursToApply;
            Location = location ?? Location.Inherited;
            FilterFlags = filterFlags ?? new List<Flag>();
        }

        public IList<BehaviourName> BehavioursToApply { get; }
        public Location Location { get; }
        public IList<Flag> FilterFlags { get; }
    }
}
