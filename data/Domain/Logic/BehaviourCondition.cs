using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Entities.Actors;

namespace LowAgeData.Domain.Logic
{
    /// <summary>
    /// <see cref="Condition"/> to target a specific <see cref="Behaviour"/>.
    /// </summary>
    public class BehaviourCondition : Condition
    {
        public BehaviourCondition(
            ConditionFlag conditionFlag,
            BehaviourId conditionedBehaviour,
            Location? behaviourOwner = null) : base(conditionFlag)
        {
            ConditionedBehaviour = conditionedBehaviour;
            BehaviourOwner = behaviourOwner;
        }
        
        /// <summary>
        /// Used to check for a specific <see cref="Behaviour"/>.
        /// </summary>
        public BehaviourId ConditionedBehaviour { get; }
        
        /// <summary>
        /// Used if the <see cref="ConditionedBehaviour"/> has to be owned by a specific <see cref="Actor"/>.
        /// </summary>
        public Location? BehaviourOwner { get; }
    }
}