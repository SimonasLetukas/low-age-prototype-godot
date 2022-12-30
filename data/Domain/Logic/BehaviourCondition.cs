using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// <see cref="Condition"/> to target a specific <see cref="Behaviour"/>.
    /// </summary>
    public class BehaviourCondition : Condition
    {
        public BehaviourCondition(
            Flag conditionFlag,
            BehaviourName conditionedBehaviour,
            Location? behaviourOwner = null) : base($"{nameof(Condition)}.{nameof(BehaviourCondition)}", conditionFlag)
        {
            ConditionedBehaviour = conditionedBehaviour;
            BehaviourOwner = behaviourOwner;
        }
        
        /// <summary>
        /// Used to check for a specific <see cref="Behaviour"/>.
        /// </summary>
        public BehaviourName ConditionedBehaviour { get; }
        
        /// <summary>
        /// Used if the <see cref="ConditionedBehaviour"/> has to be owned by a specific <see cref="Actor"/>.
        /// </summary>
        public Location? BehaviourOwner { get; }
    }
}