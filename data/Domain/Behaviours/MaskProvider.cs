using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Durations;
using low_age_data.Domain.Common.Shape;
using low_age_data.Domain.Masks;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// <see cref="Behaviour"/> that creates a <see cref="Mask"/> in a certain <see cref="IShape"/>.
    /// </summary>
    public class MaskProvider : Behaviour
    {
        public MaskProvider(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            MaskId maskCreated,
            IShape maskShape,
            EndsAt? endsAt = null,
            bool? ownerAllowed = null,
            bool? hasSameInstanceForAllOwners = null) 
            : base(
                id, 
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
        /// The <see cref="IShape"/> and size of the <see cref="Mask"/> created.
        /// </summary>
        public IShape MaskShape { get; }
    }
}