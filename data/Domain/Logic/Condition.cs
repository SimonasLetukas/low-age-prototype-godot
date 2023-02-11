using low_age_data.Domain.Shared.Flags;
using Newtonsoft.Json;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// Base condition for a target.
    /// </summary>
    public class Condition
    {
        internal Condition(
            string type,
            ConditionFlag conditionFlag)
        {
            Type = type;
            ConditionFlag = conditionFlag;
        }

        public Condition(
            ConditionFlag conditionFlag)
        {
            Type = $"{nameof(Condition)}";
            ConditionFlag = conditionFlag;
        }
        
        [JsonProperty(Order = -3)]
        public string Type { get; }
        
        /// <summary>
        /// Used to specify how the target is conditioned.
        /// </summary>
        public ConditionFlag ConditionFlag { get; }
    }
}
