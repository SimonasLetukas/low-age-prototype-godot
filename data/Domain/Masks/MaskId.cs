using System;
using low_age_data.Common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Masks
{
    [JsonConverter(typeof(MaskIdJsonConverter))]
    public class MaskId : Id
    {
        private MaskId(string value) : base($"mask-{value}")
        {
        }

        public static MaskId Power => new MaskId($"{nameof(Power)}".ToKebabCase());
        
        private class MaskIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(MaskId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (MaskId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new MaskId(value ?? throw new InvalidOperationException());
            }
        }
    }
}