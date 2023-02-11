using low_age_data.Domain.Masks;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// <see cref="Condition"/> to target a specific <see cref="Mask"/>.
    /// </summary>
    public class MaskCondition : Condition
    {
        public MaskCondition(
            ConditionFlag conditionFlag,
            MaskName conditionedMask) : base($"{nameof(Condition)}.{nameof(MaskCondition)}", conditionFlag)
        {
            ConditionedMask = conditionedMask;
        }
        
        /// <summary>
        /// Used to check for a specific <see cref="Mask"/>
        /// </summary>
        public MaskName ConditionedMask { get; }
    }
}