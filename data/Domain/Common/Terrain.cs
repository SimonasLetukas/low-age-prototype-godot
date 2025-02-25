using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    [JsonConverter(typeof(TerrainJsonConverter))]
    public class Terrain : EnumValueObject<Terrain, Terrain.TerrainsEnum>
    {
        public static Terrain Grass => new(TerrainsEnum.Grass);
        public static Terrain Mountains => new(TerrainsEnum.Mountains);
        public static Terrain Marsh => new(TerrainsEnum.Marsh);
        public static Terrain Scraps => new(TerrainsEnum.Scraps);
        public static Terrain Celestium => new(TerrainsEnum.Celestium);
        public static Terrain HighGround => new(TerrainsEnum.HighGround);
        public static Terrain FromIndex(int index) => new((TerrainsEnum)index);

        public int ToIndex() => (int)Value;
        public string ToDisplayValue() => Value.ToString();

        private Terrain(TerrainsEnum @enum) : base(@enum) { }

        private Terrain(string? from) : base(from) { }

        public enum TerrainsEnum
        {
            Grass = 0,
            Mountains = 1,
            Marsh = 2,
            Scraps = 3,
            Celestium = 4,
            HighGround = -2,
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

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new Terrain(value);
            }
        }
    }
}
