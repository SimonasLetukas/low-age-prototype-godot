using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    [JsonConverter(typeof(AttacksJsonConverter))]
    public class AttackType : EnumValueObject<AttackType, AttackType.AttacksEnum>
    {
        public static AttackType Melee => new(AttacksEnum.Melee);
        public static AttackType Ranged => new(AttacksEnum.Ranged);

        private AttackType(AttacksEnum @enum) : base(@enum) { }
        
        private AttackType(string? from) : base(from) { }
        
        public enum AttacksEnum
        {
            Melee,
            Ranged 
        }
        
        private class AttacksJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(AttackType);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (AttackType)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new AttackType(value);
            }
        }
    }
}
