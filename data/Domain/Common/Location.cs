using System;
using low_age_data.Domain.Effects;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    [JsonConverter(typeof(LocationJsonConverter))]
    public class Location : EnumValueObject<Location, Location.Locations>
    {
        /// <summary>
        /// Allows a default <see cref="Location"/> to be set by the caller (e.g. <see cref="Search"/> sets
        /// <see cref="Location.Actor"/> by default). Otherwise applies the same location that was set before in the
        /// chain. Default = <see cref="Location.Self"/>.
        /// </summary>
        public static Location Inherited => new Location(Locations.Inherited);

        /// <summary>
        /// Targets the current actor in the chain
        /// </summary>
        public static Location Self => new Location(Locations.Self);

        /// <summary>
        /// Targets single actor
        /// </summary>
        public static Location Actor => new Location(Locations.Actor);

        /// <summary>
        /// Targets point on the ground (tile)
        /// </summary>
        public static Location Point => new Location(Locations.Point);

        /// <summary>
        /// Targets the previous actor in the chain
        /// </summary>
        public static Location Source => new Location(Locations.Source);

        /// <summary>
        /// Targets origin - the first actor from which the chain started
        /// </summary>
        public static Location Origin => new Location(Locations.Origin);

        private Location(Locations @enum) : base(@enum) { }
        
        private Location(string? from) : base(from) { }
        
        public enum Locations
        {
            Inherited,
            Self,
            Actor,
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
