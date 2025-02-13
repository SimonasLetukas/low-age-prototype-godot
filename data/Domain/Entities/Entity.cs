using LowAgeData.Domain.Common;
using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Entities
{
    public abstract class Entity : IDisplayable
    {
        protected Entity(EntityId id, string displayName, string description, string sprite, Vector2Int centerOffset)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Sprite = sprite;
            CenterOffset = centerOffset;
        }

        [JsonProperty(Order = -3)]
        public EntityId Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string? Sprite { get; }
        public Vector2Int CenterOffset { get; }
    }
}
