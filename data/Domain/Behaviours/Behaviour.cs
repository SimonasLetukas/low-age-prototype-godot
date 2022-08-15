using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Shared.Durations;
using Newtonsoft.Json;

namespace low_age_data.Domain.Behaviours
{
    public class Behaviour
    {
        protected Behaviour(
            BehaviourName name, 
            string type, 
            string displayName, 
            string description, 
            EndsAt endsAt, 
            bool? ownerAllowed = null, 
            bool? hasSameInstanceForAllOwners = null)
        {
            Name = name;
            Type = type;
            DisplayName = displayName;
            Description = description;
            EndsAt = endsAt;
            OwnerAllowed = ownerAllowed ?? false;
            HasSameInstanceForAllOwners = hasSameInstanceForAllOwners ?? false;
        }

        [JsonProperty(Order = -3)]
        public BehaviourName Name { get; }
        [JsonProperty(Order = -2)]
        public string Type { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public EndsAt EndsAt { get; }
        
        /// <summary>
        /// If true, <see cref="Behaviour"/> can be owned by <see cref="Actor"/> instance which by default allows to
        /// have multiple behaviour instances (all tracked separately) of the same type on the same entity for each
        /// owner instance. This can also be useful for validation. 
        /// </summary>
        public bool OwnerAllowed { get; }
        
        /// <summary>
        /// If true, <see cref="Behaviour"/>s added and owned by different <see cref="Actor"/>s are overwritten and 
        /// reset (in the context of <see cref="Buff.CanStack"/> and <see cref="Buff.CanResetDuration"/>) each time.
        /// This means that only one instance of behaviour can co-exist among any number of owners. 
        /// </summary>
        public bool HasSameInstanceForAllOwners { get; }
    }
}
