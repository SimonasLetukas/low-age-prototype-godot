using Newtonsoft.Json;

namespace low_age_data.Domain.Entities
{
    public abstract class Entity
    {
        protected Entity(EntityId id, string displayName, string description)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
        }

        [JsonProperty(Order = -3)]
        public EntityId Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
    }
}
