using System;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    [JsonConverter(typeof(AttacksJsonConverter))]
    public class Attacks : EnumValueObject<Attacks, Attacks.AttacksEnum>
    {
        public static Attacks Melee => new Attacks(AttacksEnum.Melee);
        public static Attacks Ranged => new Attacks(AttacksEnum.Ranged);

        private Attacks(AttacksEnum @enum) : base(@enum) { }
        
        private Attacks(string? from) : base(from) { }
        
        public enum AttacksEnum
        {
            Melee,
            Ranged 
        }
        
        private class AttacksJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Attacks);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (Attacks)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new Attacks(value);
            }
        }
    }
}
