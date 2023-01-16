using low_age_data.Common;
using low_age_data.Domain.Masks;
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
            BehaviourName name,
            string displayName, 
            string description,
            MaskName maskCreated,
            Shape maskShape,
            EndsAt? endsAt = null,
            bool? ownerAllowed = null,
            bool? hasSameInstanceForAllOwners = null) 
            : base(
                name, 
                $"{nameof(Behaviour)}.{nameof(MaskProvider)}", 
                displayName,
                description, 
                endsAt ?? EndsAt.Death,
                ownerAllowed, 
                hasSameInstanceForAllOwners)
        {
            MaskCreated = maskCreated;
            MaskShape = maskShape;
        }
        
        /// <summary>
        /// Specifies what kind of <see cref="Mask"/> is created by this <see cref="MaskProvider"/>.
        /// </summary>
        public MaskName MaskCreated { get; }
        
        /// <summary>
        /// The <see cref="Shape"/> and size of the <see cref="Mask"/> created.
        /// </summary>
        public Shape MaskShape { get; }
    }
}