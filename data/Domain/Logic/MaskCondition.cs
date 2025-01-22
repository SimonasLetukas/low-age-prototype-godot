using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Masks;

namespace LowAgeData.Domain.Logic
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