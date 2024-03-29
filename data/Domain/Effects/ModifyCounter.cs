﻿using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Filters;
using low_age_data.Domain.Common.Modifications;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class ModifyCounter : Effect
    {
        public ModifyCounter(
            EffectId id,
            IList<BehaviourId> countersToModify,
            Change change,
            int amount,
            Location? location = null,
            IList<IFilterItem>? filters = null,
            IList<Validator>? validators = null) : base(id, validators ?? new List<Validator>())
        {
            CountersToModify = countersToModify;
            Change = change;
            Amount = amount;
            Location = location ?? Location.Inherited;
            Filters = filters ?? new List<IFilterItem>();
        }
        
        /// <summary>
        /// Will affect any found <see cref="Counter"/>
        /// </summary>
        public IList<BehaviourId> CountersToModify { get; }
        public Change Change { get; }
        public int Amount { get; }
        public Location Location { get; }
        public IList<IFilterItem> Filters { get; }
    }
}