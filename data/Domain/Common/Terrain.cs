﻿using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common
{
    [JsonConverter(typeof(TerrainJsonConverter))]
    public class Terrain : EnumValueObject<Terrain, Terrain.TerrainsEnum>
    {
        public static Terrain Grass => new Terrain(TerrainsEnum.Grass);
        public static Terrain Mountains => new Terrain(TerrainsEnum.Mountains);
        public static Terrain Marsh => new Terrain(TerrainsEnum.Marsh);
        public static Terrain Scraps => new Terrain(TerrainsEnum.Scraps);
        public static Terrain Celestium => new Terrain(TerrainsEnum.Celestium);
        public static Terrain FromIndex(int index) => new Terrain((TerrainsEnum)index);

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
