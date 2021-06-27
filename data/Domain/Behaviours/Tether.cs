using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    /// Used for entity to be moved by the source entity (automatically removed if the source is destroyed)
    public class Tether : Behaviour
    {
        public Tether(
            BehaviourName name,
            string displayName,
            string description,
            Location? source = null,
            bool? extendsSelection = null,
            bool? sharedDamage = null,
            int? maximumLeashRange = null,
            bool? calculatedForSourcePathfinding = null,
            EndsAt? endsAt = null) : base(name, $"{nameof(Behaviour)}.{nameof(Tether)}", displayName, description, endsAt ?? EndsAt.Death)
        {
            Source = source ?? Location.Inherited;
            ExtendsSelection = extendsSelection ?? false;
            SharedDamage = sharedDamage ?? false;
            MaximumLeashRange = maximumLeashRange ?? 1;
            CalculatedForSourcePathfinding = calculatedForSourcePathfinding ?? true;
        }

        public Location Source { get; }
        public bool ExtendsSelection { get; } // If true, selecting the tethered entity will also select the source entity
        public bool SharedDamage { get; } // If true, any damage done to either tethered or source entity is also done to the other one
        public int MaximumLeashRange { get; }
        public bool CalculatedForSourcePathfinding { get; } // If true, source entity to which this tether is attached to
                                                            // considers the attached entity for its pathfinding
    }
}
