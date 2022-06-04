using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Logic
{
    public class Condition
    {
        public Condition(Flag conditionFlag, BehaviourName? conditionedBehaviour = null)
        {
            Type = $"{nameof(Condition)}";
            ConditionFlag = conditionFlag;
            ConditionedBehaviour = conditionedBehaviour;
        }
        
        public string Type { get; }
        public Flag ConditionFlag { get; }
        
        /// <summary>
        /// Used if the condition flag requires to check for a specific behaviour
        /// </summary>
        public BehaviourName? ConditionedBehaviour { get; }
    }
}
