using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Abilities
{
    public class Passive : Ability
    {
        public Passive(
            AbilityName name,
            string displayName,
            string description,
            bool hasButton = false,
            EffectName? periodicEffect = null,
            IList<ResearchName>? researchNeeded = null,
            IList<EffectName>? onHitEffects = null,
            IList<Attacks>? onHitAttackTypes = null,
            EffectName? onBirthEffect = null,
            BehaviourName? onBuildBehaviour = null,
            IList<Cost>? cost = null)
            : base(
                name,
                $"{nameof(Ability)}.{nameof(Passive)}",
                TurnPhase.Passive,
                researchNeeded ?? new List<ResearchName>(),
                hasButton,
                displayName,
                description,
                EndsAt.Instant,
                cost)
        {
            PeriodicEffect = periodicEffect;
            OnHitEffects = onHitEffects ?? new List<EffectName>();
            OnHitAttackTypes = onHitAttackTypes ?? new List<Attacks>();
            OnBirthEffect = onBirthEffect;
            OnBuildBehaviour = onBuildBehaviour;
        }

        /// <summary>
        /// Executes effect as often as possible
        /// </summary>
        public EffectName? PeriodicEffect { get; }

        /// <summary>
        /// Executes effects on each attack
        /// </summary>
        public IList<EffectName> OnHitEffects { get; }

        /// <summary>
        /// Determines which attack types trigger the on-hit effect
        /// </summary>
        public IList<Attacks> OnHitAttackTypes { get; }

        /// <summary>
        /// Executes effect upon birth or upon research completion on an entity with this ability.
        /// </summary>
        public EffectName? OnBirthEffect { get; }

        /// <summary>
        /// <see cref="Buildable"/> behaviour checked before and applied after <see cref="Build"/> start.
        /// </summary>
        public BehaviourName? OnBuildBehaviour { get; }
    }
}