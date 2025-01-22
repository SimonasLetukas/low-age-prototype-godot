﻿using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;

namespace LowAgeData.Domain.Behaviours
{
    public class ExtraAttack : Behaviour
    {
        public ExtraAttack(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            IList<Attacks> attackTypes,
            EndsAt? endsAt = null,
            bool canStack = false) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                endsAt ?? EndsAt.Death,
                Alignment.Positive,
                canStack)
        {
            AttackTypes = attackTypes;
        }

        public IList<Attacks> AttackTypes { get; }
    }
}
