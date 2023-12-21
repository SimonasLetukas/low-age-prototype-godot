using low_age_data.Domain.Common.Flags;

namespace low_age_data.Domain.Logic
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
