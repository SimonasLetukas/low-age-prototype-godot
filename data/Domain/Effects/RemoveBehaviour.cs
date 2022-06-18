using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Effects
{
    public class RemoveBehaviour : Effect
    {
        public RemoveBehaviour(
            EffectName name,
            IList<BehaviourName> behavioursToRemove,
            Location? location = null,
            IList<Flag>? filterFlags = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(RemoveBehaviour)}", validators ?? new List<Validator>())
        {
            BehavioursToRemove = behavioursToRemove;
            Location = location ?? Location.Inherited;
            FilterFlags = filterFlags ?? new List<Flag>();
        }

        public IList<BehaviourName> BehavioursToRemove { get; }
        public Location Location { get; }
        public IList<Flag> FilterFlags { get; }
    }
}