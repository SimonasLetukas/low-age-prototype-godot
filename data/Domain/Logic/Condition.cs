using LowAgeData.Domain.Common.Flags;

namespace LowAgeData.Domain.Logic
{
    /// <summary>
    /// Base condition for a target.
    /// </summary>
    public class Condition
    {
        public Condition(ConditionFlag conditionFlag)
        {
            ConditionFlag = conditionFlag;
        }

        /// <summary>
        /// Used to specify how the target is conditioned.
        /// </summary>
        public ConditionFlag ConditionFlag { get; }
    }
}
