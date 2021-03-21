using System;
using Newtonsoft.Json;

namespace low_age_data.Domain.Abilities
{
    public class Ability
    {
        protected Ability(AbilityName name, string type, string displayName, string description)
        {
            Name = name;
            Type = type;
            DisplayName = displayName;
            Description = description;
        }

        [JsonProperty(Order = -3)]
        public AbilityName Name { get; }
        [JsonProperty(Order = -2)]
        public string Type { get; }
        public string DisplayName { get; }
        public string Description { get; }
    }
}
