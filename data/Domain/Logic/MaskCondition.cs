using low_age_data.Domain.Common.Flags;
using low_age_data.Domain.Masks;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// <see cref="Condition"/> to target a specific <see cref="Mask"/>.
    /// </summary>
    public class MaskCondition : Condition
    {
        public MaskCondition(
            ConditionFlag conditionFlag,
            MaskId conditionedMask) : base(conditionFlag)
        {
            ConditionedMask = conditionedMask;
        }
        
        /// <summary>
        /// Used to check for a specific <see cref="Mask"/>
        /// </summary>
        public MaskId ConditionedMask { get; }
    }
}