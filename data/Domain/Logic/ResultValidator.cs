﻿using System.Collections.Generic;
using low_age_data.Domain.Effects;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// Used to check a searched area for a condition on found targets.
    /// </summary>
    public class ResultValidator : Validator
    {
        public ResultValidator(EffectName searchEffect, IList<Condition> conditions) : base(conditions, $"{nameof(Validator)}.{nameof(ResultValidator)}")
        {
            SearchEffect = searchEffect;
        }
        
        /// <summary>
        /// <see cref="Search"/> to collect the found targets from.
        /// </summary>
        public EffectName SearchEffect { get; }
    }
}