using Newtonsoft.Json;

namespace low_age_data.Domain.Entities
{
    public abstract class Entity
    {
        protected Entity(EntityName name, string displayName, string description)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }

        [JsonProperty(Order = -3)]
        public EntityName Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
    }
}
