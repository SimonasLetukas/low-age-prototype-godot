using LowAgeData.Domain.Effects;
using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    [JsonConverter(typeof(LocationJsonConverter))]
    public class Location : EnumValueObject<Location, Location.Locations>
    {
        /// <summary>
        /// Allows a default <see cref="Location"/> to be set by the caller (e.g. <see cref="Search"/> sets
        /// <see cref="Entity"/> by default). Otherwise applies the same location that was set before in the
        /// chain. Default = <see cref="Location.Self"/>.
        /// </summary>
        public static Location Inherited => new Location(Locations.Inherited);

        /// <summary>
        /// Targets the current entity in the chain
        /// </summary>
        public static Location Self => new Location(Locations.Self);

        /// <summary>
        /// Targets single entity
        /// </summary>
        public static Location Entity => new Location(Locations.Entity);

        /// <summary>
        /// Targets point on the ground (tile)
        /// </summary>
        public static Location Point => new Location(Locations.Point);

        /// <summary>
        /// Targets the previous entity in the chain
        /// </summary>
        public static Location Source => new Location(Locations.Source);

        /// <summary>
        /// Targets origin - the first entity from which the chain started
        /// </summary>
        public static Location Origin => new Location(Locations.Origin);

        private Location(Locations @enum) : base(@enum) { }
        
        private Location(string? from) : base(from) { }
        
        public enum Locations
        {
            Inherited,
            Self,
            Entity,
            Point,
            Source,
            Origin
        }

        private class LocationJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Location);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (Location)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new Location(value);
            }
        }
    }
}
