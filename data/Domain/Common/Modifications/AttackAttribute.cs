using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common.Modifications
{
    [JsonConverter(typeof(AttackAttributeJsonConverter))]
    public class AttackAttribute : EnumValueObject<AttackAttribute, AttackAttribute.AttackAttributes>
    {
        public static AttackAttribute MaxAmount => new AttackAttribute(AttackAttributes.MaxAmount);
        public static AttackAttribute MaxDistance => new AttackAttribute(AttackAttributes.MaxDistance);

        private AttackAttribute(AttackAttributes @enum) : base(@enum) { }

        private AttackAttribute(string? from) : base(from) { }
        
        public enum AttackAttributes
        {
            MaxAmount,
            MinDistance,
            MaxDistance,
            BonusAmount,
        }

        private class AttackAttributeJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(AttackAttribute);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (AttackAttribute)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new AttackAttribute(value);
            }
        }
    }
}
