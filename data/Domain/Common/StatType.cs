using System;
using low_age_data.Shared;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common
{
    [JsonConverter(typeof(StatTypeJsonConverter))]
    public class StatType : EnumValueObject<StatType, StatType.StatTypeEnum>
    {
        public static StatType Health => new StatType(StatTypeEnum.Health);
        public static StatType Shields => new StatType(StatTypeEnum.Shields);
        public static StatType MeleeArmour => new StatType(StatTypeEnum.MeleeArmour);
        public static StatType RangedArmour => new StatType(StatTypeEnum.RangedArmour);
        public static StatType Movement => new StatType(StatTypeEnum.Movement);
        public static StatType Initiative => new StatType(StatTypeEnum.Initiative);
        public static StatType Vision => new StatType(StatTypeEnum.Vision);

        private StatType(StatTypeEnum @enum) : base(@enum) { }
        
        private StatType(string? from) : base(from) { }

        public enum StatTypeEnum
        {
            Health,
            Shields,
            MeleeArmour,
            RangedArmour,
            Movement,
            Initiative,
            Vision
        }
        
        private class StatTypeJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(StatType);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (StatType)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new StatType(value);
            }
        }
    }
}
