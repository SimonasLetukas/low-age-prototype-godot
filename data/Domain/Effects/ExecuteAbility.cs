﻿using System.Collections.Generic;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
{
    public class ExecuteAbility : Effect
    {
        public ExecuteAbility(
            EffectId id, 
            AbilityId abilityToExecute,
            Location? executingPlayer = null,
            bool? cancelSynchronised = null,
            IList<Validator>? validators = null) 
            : base(
                id, 
                validators ?? new List<Validator>())
        {
            AbilityToExecute = abilityToExecute;
            ExecutingPlayer = executingPlayer ?? Location.Inherited;
            CancelSynchronised = cancelSynchronised ?? false;
        }
        
        /// <summary>
        /// Immediately executes the <see cref="Ability"/>.
        /// </summary>
        public AbilityId AbilityToExecute { get; }
        
        /// <summary>
        /// Used to specify the player who has the control over the <see cref="AbilityToExecute"/>.
        /// </summary>
        public Location ExecutingPlayer { get; }
        
        /// <summary>
        /// If true, cancelling the <see cref="AbilityToExecute"/> will also cancel the previous <see cref="Ability"/>
        /// in the chain, if any. 
        /// </summary>
        public bool CancelSynchronised { get; }
    }
}