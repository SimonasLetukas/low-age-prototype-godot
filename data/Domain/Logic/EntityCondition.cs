using low_age_data.Domain.Common.Flags;
using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// <see cref="Condition"/> to target specific <see cref="Entity"/>s.
    /// </summary>
    public class EntityCondition : Condition
    {
        public EntityCondition(
            ConditionFlag conditionFlag,
            EntityId conditionedEntity,
            int? amountOfEntitiesRequired = null) : base(conditionFlag)
        {
            ConditionedEntity = conditionedEntity;
            AmountOfEntitiesRequired = amountOfEntitiesRequired ?? 1;
        }
        
        /// <summary>
        /// Specify the <see cref="Entity"/> to be targeted by this <see cref="EntityCondition"/>.
        /// </summary>
        public EntityId ConditionedEntity { get; }
        
        /// <summary>
        /// How many entities should be found for this condition to return true. 
        /// </summary>
        public int AmountOfEntitiesRequired { get; }
    }
}