using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags
{
    [JsonConverter(typeof(AmountFlagJsonConverter))]
    public class AmountFlag : EnumValueObject<AmountFlag, AmountFlag.AmountFlags>
    {
        public static AmountFlag FromMissingHealth => new AmountFlag(AmountFlags.FromMissingHealth);

        private AmountFlag(AmountFlags @enum) : base(@enum) { }

        private AmountFlag(string? from) : base(from) { }
        
        public enum AmountFlags
        {
            FromMissingHealth
        }
        
        private class AmountFlagJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(AmountFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (AmountFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new AmountFlag(value);
            }
        }
    }
}