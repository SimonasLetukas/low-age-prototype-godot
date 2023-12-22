using low_age_data.Domain.Common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Entities
{
    public abstract class Entity : IDisplayable
    {
        protected Entity(EntityId id, string displayName, string description, string sprite)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Sprite = sprite;
        }

        [JsonProperty(Order = -3)]
        public EntityId Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string? Sprite { get; }
    }
}
