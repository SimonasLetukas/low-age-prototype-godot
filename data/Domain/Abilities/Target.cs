using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;
using low_age_data.Domain.Shared.Shape;

namespace low_age_data.Domain.Abilities
{
    public class Target : Ability
    {
        public Target(
            AbilityName name,
            TurnPhase turnPhase,
            string displayName,
            string description,
            Shape targetArea,
            IList<EffectName> effects,
            IList<ResearchName>? researchNeeded = null,
            EndsAt? cooldown = null,
            IList<Attacks>? overridesAttacks = null,
            bool? fallbackToAttack = false,
            IList<Cost>? cost = null,
            Shape? leashArea = null)
            : base(
                name,
                $"{nameof(Ability)}.{nameof(Target)}",
                turnPhase,
                researchNeeded ?? new List<ResearchName>(),
                true,
                displayName,
                description,
                cooldown,
                cost)
        {
            TargetArea = targetArea;
            Effects = effects;
            OverridesAttacks = overridesAttacks ?? new List<Attacks>();
            FallbackToAttack = fallbackToAttack ?? false;
            LeashArea = leashArea ?? new Map();
        }

        public Shape TargetArea { get; }
        public IList<EffectName> Effects { get; }

        /// <summary>
        /// Attacking using any of the listed <see cref="Attacks"/> will execute this <see cref="Target"/> ability
        /// instead. Having any <see cref="Attacks"/> in this list will automatically change button of
        /// <see cref="Target"/> to be <see cref="Passive"/>. <see cref="Attacks"/> could then target ground, depending
        /// on underlying <see cref="Effects"/>.
        /// </summary>
        public IList<Attacks> OverridesAttacks { get; }

        /// <summary>
        /// If true, when <see cref="Target"/> cannot override an attack listed in <see cref="OverridesAttacks"/> (e.g.
        /// when <see cref="TargetArea"/>, <see cref="Ability.Cooldown"/>, <see cref="ResearchName"/> or any
        /// <see cref="Validator"/> in underlying <see cref="Effects"/> is not satisfied), the attack will be executed
        /// instead. 
        /// </summary>
        public bool FallbackToAttack { get; }
        
        /// <summary>
        /// If the execution of this <see cref="Ability"/> is ever delayed (e.g. when <see cref="Ability.Cost"/> is
        /// being paid), <see cref="LeashArea"/> defines what is the biggest possible area for the targeted
        /// <see cref="Actor"/>s to be inside when the execution starts. In other words, if the targeted
        /// <see cref="Actor"/> leaves the <see cref="LeashArea"/> while the execution is delayed, the ability is
        /// cancelled and the <see cref="Ability.Cost"/> is refunded.
        ///
        /// By default, the <see cref="LeashArea"/> has the size of the map. 
        /// </summary>
        public Shape LeashArea { get; }
    }
}