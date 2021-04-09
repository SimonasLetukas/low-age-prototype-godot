using System.Collections.Generic;
using low_age_data.Domain.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Abilities
{
    public class Ability
    {
        protected Ability(
            AbilityName name, 
            string type, 
            TurnPhase turnPhase, 
            IList<Research> researchNeeded,
            bool hasButton,
            string displayName, 
            string description)
        {
            Name = name;
            Type = type;
            TurnPhase = turnPhase;
            ResearchNeeded = researchNeeded;
            HasButton = hasButton;
            DisplayName = displayName;
            Description = description;
        }

        [JsonProperty(Order = -5)]
        public AbilityName Name { get; }
        [JsonProperty(Order = -4)]
        public string Type { get; }
        [JsonProperty(Order = -3)]
        public TurnPhase TurnPhase { get; }
        [JsonProperty(Order = -2)]
        public IList<Research> ResearchNeeded { get; }
        public bool HasButton { get; }
        public string DisplayName { get; }
        public string Description { get; }
    }
}
