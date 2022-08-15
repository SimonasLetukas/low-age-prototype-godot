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
            Location? behaviourOwner = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(ApplyBehaviour)}", validators ?? new List<Validator>())
        {
            BehavioursToApply = behavioursToApply;
            Location = location ?? Location.Inherited;
            FilterFlags = filterFlags ?? new List<Flag>();
            BehaviourOwner = behaviourOwner;
        }

        public IList<BehaviourName> BehavioursToApply { get; }
        public Location Location { get; }
        public IList<Flag> FilterFlags { get; }
        
        /// <summary>
        /// If <see cref="Behaviour.OwnerAllowed"/> is true, this property can be used to specify the owner.
        /// </summary>
        public Location? BehaviourOwner { get; }
    }
}
