using low_age_data.Domain.Entities;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// <see cref="Condition"/> to target specific <see cref="Entity"/>s.
    /// </summary>
    public class EntityCondition : Condition
    {
        public EntityCondition(
            Flag conditionFlag,
            EntityName conditionedEntity,
            int? amountOfEntitiesRequired = null) : base($"{nameof(Condition)}.{nameof(EntityCondition)}", conditionFlag)
        {
            ConditionedEntity = conditionedEntity;
            AmountOfEntitiesRequired = amountOfEntitiesRequired ?? 1;
        }
        
        /// <summary>
        /// Specify the <see cref="Entity"/> to be targeted by this <see cref="EntityCondition"/>.
        /// </summary>
        public EntityName ConditionedEntity { get; }
        
        /// <summary>
        /// How many entities should be found for this condition to return true. 
        /// </summary>
        public int AmountOfEntitiesRequired { get; }
    }
}