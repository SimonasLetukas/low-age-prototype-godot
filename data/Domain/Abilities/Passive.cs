﻿using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;

namespace LowAgeData.Domain.Abilities
{
    public class Passive : Ability
    {
        public Passive(
            AbilityId id,
            string displayName,
            string description,
            string? sprite,
            bool hasButton = false,
            EffectId? periodicEffect = null,
            IList<ResearchId>? researchNeeded = null,
            IList<EffectId>? onHitEffects = null,
            IList<AttackType>? onHitAttackTypes = null,
            IList<EffectId>? onBirthEffects = null,
            BehaviourId? onBuildBehaviour = null,
            IList<Payment>? cost = null)
            : base(
                id,
                TurnPhase.Passive,
                researchNeeded ?? new List<ResearchId>(),
                hasButton,
                displayName,
                description,
                sprite,
                EndsAt.Instant,
                cost)
        {
            PeriodicEffect = periodicEffect;
            OnHitEffects = onHitEffects ?? new List<EffectId>();
            OnHitAttackTypes = onHitAttackTypes ?? new List<AttackType>();
            OnBirthEffects = onBirthEffects ?? new List<EffectId>();
            OnBuildBehaviour = onBuildBehaviour;
        }

        /// <summary>
        /// Executes effect as often as possible
        /// </summary>
        public EffectId? PeriodicEffect { get; }

        /// <summary>
        /// Executes effects on each attack
        /// </summary>
        public IList<EffectId> OnHitEffects { get; }

        /// <summary>
        /// Determines which attack types trigger the on-hit effect
        /// </summary>
        public IList<AttackType> OnHitAttackTypes { get; }

        /// <summary>
        /// Executes effect upon birth or upon research completion on an entity with this ability.
        /// </summary>
        public IList<EffectId> OnBirthEffects { get; }

        /// <summary>
        /// <see cref="Buildable"/> behaviour checked before and applied after <see cref="Build"/> start.
        /// </summary>
        public BehaviourId? OnBuildBehaviour { get; }
    }
}