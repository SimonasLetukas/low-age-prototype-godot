using System;
using low_age_data.Shared;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Entities.Actors.Units
{
    [JsonConverter(typeof(UnitIdJsonConverter))]
    public class UnitId : EntityId
    {
        [JsonConstructor]
        public UnitId(string value, bool addPrefix = false) : base(addPrefix ? $"unit-{value}" : value)
        {
        }

        public static UnitId Slave => new UnitId(nameof(Slave).ToKebabCase(), true);
        public static UnitId Leader => new UnitId(nameof(Leader).ToKebabCase(), true);
        public static UnitId Quickdraw => new UnitId(nameof(Quickdraw).ToKebabCase(), true);
        public static UnitId Gorger => new UnitId(nameof(Gorger).ToKebabCase(), true);
        public static UnitId Camou => new UnitId(nameof(Camou).ToKebabCase(), true);
        public static UnitId Shaman => new UnitId(nameof(Shaman).ToKebabCase(), true);
        public static UnitId Pyre => new UnitId(nameof(Pyre).ToKebabCase(), true);
        public static UnitId BigBadBull => new UnitId(nameof(BigBadBull).ToKebabCase(), true);
        public static UnitId Mummy => new UnitId(nameof(Mummy).ToKebabCase(), true);
        public static UnitId Roach => new UnitId(nameof(Roach).ToKebabCase(), true);
        public static UnitId Parasite => new UnitId(nameof(Parasite).ToKebabCase(), true);

        public static UnitId Horrior => new UnitId(nameof(Horrior).ToKebabCase(), true);
        public static UnitId Surfer => new UnitId(nameof(Surfer).ToKebabCase(), true);
        public static UnitId Marksman => new UnitId(nameof(Marksman).ToKebabCase(), true);
        public static UnitId Mortar => new UnitId(nameof(Mortar).ToKebabCase(), true);
        public static UnitId Hawk => new UnitId(nameof(Hawk).ToKebabCase(), true);
        public static UnitId Engineer => new UnitId(nameof(Engineer).ToKebabCase(), true);
        public static UnitId Cannon => new UnitId(nameof(Cannon).ToKebabCase(), true);
        public static UnitId Ballista => new UnitId(nameof(Ballista).ToKebabCase(), true);
        public static UnitId Radar => new UnitId(nameof(Radar).ToKebabCase(), true);
        public static UnitId Vessel => new UnitId(nameof(Vessel).ToKebabCase(), true);
        public static UnitId Omen => new UnitId(nameof(Omen).ToKebabCase(), true);
        
        private class UnitIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(UnitId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (UnitId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new UnitId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
