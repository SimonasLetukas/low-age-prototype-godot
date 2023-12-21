using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Entities
{
    [JsonConverter(typeof(EntityIdJsonConverter))]
    public class EntityId : Id
    {
        protected EntityId(string value) : base(value)
        {
        }
        
        private class EntityIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(EntityId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (EntityId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new EntityId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
