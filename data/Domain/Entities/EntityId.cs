using System;
using low_age_data.Common;
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

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new EntityId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
