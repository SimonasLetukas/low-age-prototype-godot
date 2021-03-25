using System;
using low_age_data.Domain.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Abilities
{
    public class Ability
    {
        protected Ability(AbilityName name, string type, TurnPhase turnPhase, string displayName, string description)
        {
            Name = name;
            Type = type;
            TurnPhase = turnPhase;
            DisplayName = displayName;
            Description = description;
        }

        [JsonProperty(Order = -4)]
        public AbilityName Name { get; }
        [JsonProperty(Order = -3)]
        public string Type { get; }
        [JsonProperty(Order = -2)]
        public TurnPhase TurnPhase { get; }
        public string DisplayName { get; }
        public string Description { get; }
    }
}
