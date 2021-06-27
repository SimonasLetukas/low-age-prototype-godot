using low_age_data.Domain.Shared.Durations;
using Newtonsoft.Json;

namespace low_age_data.Domain.Behaviours
{
    public class Behaviour
    {
        protected Behaviour(BehaviourName name, string type, string displayName, string description, EndsAt endsAt)
        {
            Name = name;
            Type = type;
            DisplayName = displayName;
            Description = description;
            EndsAt = endsAt;
        }

        [JsonProperty(Order = -3)]
        public BehaviourName Name { get; }
        [JsonProperty(Order = -2)]
        public string Type { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public EndsAt EndsAt { get; }
    }
}
