using System.Collections.Generic;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;
using Newtonsoft.Json;

namespace low_age_data.Domain.Abilities
{
    public class Ability
    {
        protected Ability(
            AbilityId id,
            string type,
            TurnPhase turnPhase,
            IList<ResearchId> researchNeeded,
            bool hasButton,
            string displayName,
            string description,
            EndsAt? cooldown = null,
            IList<Payment>? cost = null)
        {
            Id = id;
            Type = type;
            TurnPhase = turnPhase;
            ResearchNeeded = researchNeeded;
            HasButton = hasButton;
            Cooldown = cooldown ?? EndsAt.Instant;
            Cost = cost ?? new List<Payment>();
            DisplayName = displayName;
            Description = description;
        }

        [JsonProperty(Order = -5)] public AbilityId Id { get; }
        [JsonProperty(Order = -4)] public string Type { get; }
        
        /// <summary>
        /// At which part of the round the button for this ability can be pressed.
        /// </summary>
        [JsonProperty(Order = -3)] public TurnPhase TurnPhase { get; }
        
        /// <summary>
        /// Required for all to be researched so that the <see cref="Ability"/> (and its button) would become active.
        /// </summary>
        [JsonProperty(Order = -2)] public IList<ResearchId> ResearchNeeded { get; }
        
        /// <summary>
        /// If true, the button is available when the <see cref="Actor"/> is selected. Can be useful to hide certain
        /// functionalities.
        /// </summary>
        public bool HasButton { get; }
        
        /// <summary>
        /// For how long after activation does the ability stay inactive.
        /// </summary>
        public EndsAt Cooldown { get; }
        
        /// <summary>
        /// Has to be paid in full for the <see cref="Ability"/> to start. Triggers once the button is pressed. The
        /// ability is considered inactive while the <see cref="Cost"/> is not fully paid. The timing of the payments
        /// depends on <see cref="TurnPhase"/>: <see cref="Domain.Shared.TurnPhase.Passive"/> - always paid at the
        /// start of the action phase; <see cref="Domain.Shared.TurnPhase.Planning"/> - always paid at the start of the
        /// action phase; <see cref="Domain.Shared.TurnPhase.Action"/> - first paid when activated during the
        /// <see cref="Actor"/>'s action, then at the start of the action phase.
        /// </summary>
        public IList<Payment> Cost { get; }
        
        public string DisplayName { get; }
        public string Description { get; }
    }
}