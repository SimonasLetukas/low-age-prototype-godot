using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Factions
{
    [JsonConverter(typeof(FactionIdJsonConverter))]
    public class FactionId : Id
    {
        public FactionId(string value, bool usePrefix = false) : base(usePrefix ? $"faction-{value}" : value)
        {
        }

        public static FactionId Uee => new FactionId($"{nameof(Uee)}".ToKebabCase(), true);
        public static FactionId Revelators => new FactionId($"{nameof(Revelators)}".ToKebabCase(), true);
        
        private class FactionIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(FactionId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (FactionId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new FactionId(value ?? throw new InvalidOperationException());
            }
        }
    }
}