using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Common.Modifications;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
{
    public class ModifyCounter : Effect
    {
        public ModifyCounter(
            EffectId id,
            IList<BehaviourId> countersToModify,
            Change change,
            int amount,
            Location? target = null,
            IList<IFilterItem>? filters = null,
            IList<Validator>? validators = null) 
            : base(
                id, 
                target ?? Location.Inherited, 
                validators ?? new List<Validator>())
        {
            CountersToModify = countersToModify;
            Change = change;
            Amount = amount;
            Filters = filters ?? new List<IFilterItem>();
        }
        
        /// <summary>
        /// Will affect any found <see cref="Counter"/>
        /// </summary>
        public IList<BehaviourId> CountersToModify { get; }
        public Change Change { get; }
        public int Amount { get; }
        public IList<IFilterItem> Filters { get; }
    }
}