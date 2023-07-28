using low_age_data.Domain.Masks;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;
using low_age_data.Domain.Shared.Shape;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// <see cref="Behaviour"/> that creates a <see cref="Mask"/> in a certain <see cref="Shape"/>.
    /// </summary>
    public class MaskProvider : Behaviour
    {
        public MaskProvider(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            MaskId maskCreated,
            Shape maskShape,
            EndsAt? endsAt = null,
            bool? ownerAllowed = null,
            bool? hasSameInstanceForAllOwners = null) 
            : base(
                id, 
                $"{nameof(Behaviour)}.{nameof(MaskProvider)}", 
                displayName,
                description, 
                sprite,
                endsAt ?? EndsAt.Death,
                Alignment.Neutral,
                ownerAllowed: ownerAllowed, 
                hasSameInstanceForAllOwners: hasSameInstanceForAllOwners)
        {
            MaskCreated = maskCreated;
            MaskShape = maskShape;
        }
        
        /// <summary>
        /// Specifies what kind of <see cref="Mask"/> is created by this <see cref="MaskProvider"/>.
        /// </summary>
        public MaskId MaskCreated { get; }
        
        /// <summary>
        /// The <see cref="Shape"/> and size of the <see cref="Mask"/> created.
        /// </summary>
        public Shape MaskShape { get; }
    }
}