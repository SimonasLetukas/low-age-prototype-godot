using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags
{
    [JsonConverter(typeof(AmountFlagJsonConverter))]
    public class AmountMultiplyOfFlag : EnumValueObject<AmountMultiplyOfFlag, AmountMultiplyOfFlag.AmountFlags>
    {
        public static AmountMultiplyOfFlag Vitals => new AmountMultiplyOfFlag(AmountFlags.Vitals);
        public static AmountMultiplyOfFlag Health => new AmountMultiplyOfFlag(AmountFlags.Health);
        public static AmountMultiplyOfFlag MissingVitals => new AmountMultiplyOfFlag(AmountFlags.MissingVitals);
        public static AmountMultiplyOfFlag MissingHealth => new AmountMultiplyOfFlag(AmountFlags.MissingHealth);

        private AmountMultiplyOfFlag(AmountFlags @enum) : base(@enum) { }

        private AmountMultiplyOfFlag(string? from) : base(from) { }
        
        public enum AmountFlags
        {
            Vitals,
            Health,
            MissingVitals,
            MissingHealth
        }
        
        private class AmountFlagJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(AmountMultiplyOfFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (AmountMultiplyOfFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new AmountMultiplyOfFlag(value);
            }
        }
    }
}