using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Researches;

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
            EffectId? periodicSearchEffect = null,
            IList<ResearchId>? researchNeeded = null,
            IList<EffectId>? onHitEffects = null,
            IList<AttackType>? onHitAttackTypes = null,
            IList<EffectId>? onBirthEffects = null,
            BehaviourId? onBuildBehaviour = null)
            : base(
                id,
                TurnPhase.Passive,
                researchNeeded ?? new List<ResearchId>(),
                hasButton,
                false,
                displayName,
                description,
                sprite,
                EndsAt.Instant,
                new List<Payment>())
        {
            PeriodicSearchEffect = periodicSearchEffect;
            OnHitEffects = onHitEffects ?? new List<EffectId>();
            OnHitAttackTypes = onHitAttackTypes ?? new List<AttackType>();
            OnBirthEffects = onBirthEffects ?? new List<EffectId>();
            OnBuildBehaviour = onBuildBehaviour;
        }

        /// <summary>
        /// Executes <see cref="Search"/> effect as often as possible. <see cref="Search"/>'s 
        /// <see cref="Search.TriggerFlags"/> control the periodicity.
        /// </summary>
        public EffectId? PeriodicSearchEffect { get; }

        /// <summary>
        /// Executes effects just before each attack hits the target.
        /// </summary>
        public IList<EffectId> OnHitEffects { get; }

        /// <summary>
        /// Determines which attack types trigger the on-hit effect.
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