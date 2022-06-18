using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;
using low_age_data.Domain.Shared.Modifications;

namespace low_age_data.Domain.Effects
{
    public class ModifyCounter : Effect
    {
        public ModifyCounter(
            EffectName name,
            IList<BehaviourName> countersToModify,
            Change change,
            int amount,
            Location? location = null,
            IList<Flag>? filterFlags = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(ModifyCounter)}", validators ?? new List<Validator>())
        {
            CountersToModify = countersToModify;
            Change = change;
            Amount = amount;
            Location = location ?? Location.Inherited;
            FilterFlags = filterFlags ?? new List<Flag>();
        }
        
        /// <summary>
        /// Will affect any found <see cref="Counter"/>
        /// </summary>
        public IList<BehaviourName> CountersToModify { get; }
        public Change Change { get; }
        public int Amount { get; }
        public Location Location { get; }
        public IList<Flag> FilterFlags { get; }
    }
}