using System;
using System.Collections.Generic;
using low_age_data.Common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Shared
{
    [JsonConverter(typeof(TerrainJsonConverter))]
    public class Terrain : ValueObject<Terrain>
    {
        public override string ToString()
        {
            return $"{nameof(Terrain)}.{Value}";
        }

        public static Terrain Grass => new Terrain(TerrainsEnum.Grass);
        public static Terrain Mountains => new Terrain(TerrainsEnum.Mountains);
        public static Terrain Marsh => new Terrain(TerrainsEnum.Marsh);
        public static Terrain Scraps => new Terrain(TerrainsEnum.Scraps);
        public static Terrain Celestium => new Terrain(TerrainsEnum.Celestium);
        public static Terrain FromIndex(int index) => new Terrain((TerrainsEnum)index);

        public int ToIndex() => (int)Value;
        public string ToDisplayValue() => Value.ToString();

        private Terrain(TerrainsEnum @enum)
        {
            Value = @enum;
        }

        private Terrain(string? from)
        {
            if (from is null)
            {
                Value = TerrainsEnum.Grass;
                return;
            }
            
            from = from.Substring($"{nameof(Terrain)}.".Length);
            Value = Enum.TryParse(from, out TerrainsEnum terrainsEnum) 
                ? terrainsEnum 
                : TerrainsEnum.Grass;
        }

        private TerrainsEnum Value { get; }

        private enum TerrainsEnum
        {
            Grass = 0,
            Mountains = 1,
            Marsh = 2,
            Scraps = 3,
            Celestium = 4,
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        
        private class TerrainJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Terrain);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (Terrain)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new Terrain(value);
            }
        }
    }
}
