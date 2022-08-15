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
            Location? behaviourOwner = null,
            IList<Validator>? validators = null) 
            : base(
                name, 
                $"{nameof(Effect)}.{nameof(RemoveBehaviour)}", 
                validators ?? new List<Validator>())
        {
            BehavioursToRemove = behavioursToRemove;
            Location = location ?? Location.Inherited;
            FilterFlags = filterFlags ?? new List<Flag>();
            BehaviourOwner = behaviourOwner;
        }

        public IList<BehaviourName> BehavioursToRemove { get; }
        public Location Location { get; }
        public IList<Flag> FilterFlags { get; }
        
        /// <summary>
        /// If <see cref="Behaviour.OwnerAllowed"/> is true, this property can be used to specify the owner and only
        /// remove the owner's behaviour.
        /// </summary>
        public Location? BehaviourOwner { get; }
    }
}