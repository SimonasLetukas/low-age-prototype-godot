using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Behaviours
{
    /// Used for entity to be moved by the source entity
    public class Tether : Behaviour
    {
        public Tether(
            BehaviourName name,
            string displayName,
            string description,
            Location? source = null,
            bool? extendsSource = null,
            int? maximumLeashRange = null,
            bool? calculatedForSourcePathfinding = null) : base(name, $"{nameof(Behaviour)}.{nameof(Tether)}", displayName, description)
        {
            Source = source ?? Location.Inherited;
            ExtendsSource = extendsSource ?? false;
            MaximumLeashRange = maximumLeashRange ?? 1;
            CalculatedForSourcePathfinding = calculatedForSourcePathfinding ?? true;
        }

        public Location Source { get; }
        public bool ExtendsSource { get; } // IF true, interacting with the tethered entity will also interact with the source
                                           // entity (selecting, attacking, collision detection)
        public int MaximumLeashRange { get; }
        public bool CalculatedForSourcePathfinding { get; } // If true, source entity to which this tether is attached to
                                                            // considers the attached entity for its pathfinding
    }
}
