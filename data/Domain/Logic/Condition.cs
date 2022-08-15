using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Logic
{
    public class Condition
    {
        public Condition(
            Flag conditionFlag, 
            BehaviourName? conditionedBehaviour = null, 
            Location? behaviourOwner = null)
        {
            Type = $"{nameof(Condition)}";
            ConditionFlag = conditionFlag;
            ConditionedBehaviour = conditionedBehaviour;
            BehaviourOwner = behaviourOwner;
        }
        
        public string Type { get; }
        public Flag ConditionFlag { get; }
        
        /// <summary>
        /// Used if the condition flag requires to check for a specific behaviour
        /// </summary>
        public BehaviourName? ConditionedBehaviour { get; }
        
        /// <summary>
        /// Used if the <see cref="ConditionedBehaviour"/> has to be owned by a specific <see cref="Actor"/>.
        /// </summary>
        public Location? BehaviourOwner { get; }
    }
}
