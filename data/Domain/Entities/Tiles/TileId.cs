using System;
using Newtonsoft.Json;

namespace low_age_data.Domain.Entities.Tiles
{
    [JsonConverter(typeof(TileIdJsonConverter))]
    public class TileId : EntityId
    {
        private TileId(string value) : base($"tile-{value}")
        {
        }

        public static TileId Grass => new TileId(nameof(Grass).ToLower());
        public static TileId Mountains => new TileId(nameof(Mountains).ToLower());
        public static TileId Marsh => new TileId(nameof(Marsh).ToLower());
        public static TileId Scraps => new TileId(nameof(Scraps).ToLower());
        public static TileId Celestium => new TileId(nameof(Celestium).ToLower());
        
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

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new TileId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
