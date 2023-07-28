using low_age_data.Domain.Entities;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Used for <see cref="Entity"/> to be moved by the <see cref="Source"/> <see cref="Entity"/> (automatically
    /// removed if the <see cref="Source"/> is destroyed)
    /// </summary>
    public class Tether : Behaviour
    {
        public Tether(
            BehaviourId id,
            string displayName,
            string description,
            string sprite,
            Location? source = null,
            bool? extendsSelection = null,
            bool? sharedDamage = null,
            int? maximumLeashRange = null,
            bool? calculatedForSourcePathfinding = null,
            EndsAt? endsAt = null) 
            : base(
                id, 
                $"{nameof(Behaviour)}.{nameof(Tether)}", 
                displayName, 
                description, 
                sprite,
                endsAt ?? EndsAt.Death,
                Alignment.Neutral)
        {
            Source = source ?? Location.Inherited;
            ExtendsSelection = extendsSelection ?? false;
            SharedDamage = sharedDamage ?? false;
            MaximumLeashRange = maximumLeashRange ?? 1;
            CalculatedForSourcePathfinding = calculatedForSourcePathfinding ?? true;
        }

        public Location Source { get; }
        
        /// <summary>
        /// If true, selecting the tethered <see cref="Entity"/> will also select the <see cref="Source"/>
        /// <see cref="Entity"/>. False by default.
        /// </summary>
        public bool ExtendsSelection { get; }
        
        /// <summary>
        /// If true, any damage done to either tethered or source entity is also done to the other one. False by
        /// default.
        /// </summary>
        public bool SharedDamage { get; }
        
        public int MaximumLeashRange { get; }
        
        /// <summary>
        /// If true, source entity, to which this tether is attached to, considers the attached entity for its
        /// pathfinding. True by default.
        /// </summary>
        public bool CalculatedForSourcePathfinding { get; }
    }
}
