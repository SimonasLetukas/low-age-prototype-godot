using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Masks
{
    [JsonConverter(typeof(MaskIdJsonConverter))]
    public class MaskId : Id
    {
        [JsonConstructor]
        public MaskId(string value, bool addPrefix = false) : base(addPrefix ? $"mask-{value}" : value)
        {
        }

        public static MaskId Power => new MaskId($"{nameof(Power)}".ToKebabCase(), true);
        
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

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new MaskId(value ?? throw new InvalidOperationException());
            }
        }
    }
}