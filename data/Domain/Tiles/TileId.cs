using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Tiles
{
    [JsonConverter(typeof(TileIdJsonConverter))]
    public class TileId : Id
    {
        [JsonConstructor]
        public TileId(string value, bool addPrefix = false) : base(addPrefix ? $"tile-{value}" : value)
        {
        }

        public static TileId Grass => new TileId(nameof(Grass).ToLower(), true);
        public static TileId Mountains => new TileId(nameof(Mountains).ToLower(), true);
        public static TileId Marsh => new TileId(nameof(Marsh).ToLower(), true);
        public static TileId Scraps => new TileId(nameof(Scraps).ToLower(), true);
        public static TileId Celestium => new TileId(nameof(Celestium).ToLower(), true);
        public static TileId HighGround => new TileId(nameof(HighGround).ToLower(), true);
        
        private class TileIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TileId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (TileId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new TileId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
