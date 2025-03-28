﻿using LowAgeData.Domain.Effects;

namespace LowAgeData.Domain.Logic
{
    /// <summary>
    /// Used to check a searched area for a condition on found targets.
    /// </summary>
    public class ResultValidator : Validator
    {
        public ResultValidator(EffectId searchEffect, IList<Condition> conditions) : base(conditions)
        {
            SearchEffect = searchEffect;
        }
        
        /// <summary>
        /// <see cref="Search"/> to collect the found targets from.
        /// </summary>
        public EffectId SearchEffect { get; }
    }
}
