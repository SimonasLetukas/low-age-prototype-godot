﻿using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    [JsonConverter(typeof(ActorAttributeJsonConverter))]
    public class ActorAttribute : EnumValueObject<ActorAttribute, ActorAttribute.Attributes>
    {
        public static ActorAttribute Light => new ActorAttribute(Attributes.Light);
        public static ActorAttribute Armoured => new ActorAttribute(Attributes.Armoured);
        public static ActorAttribute Giant => new ActorAttribute(Attributes.Giant);
        public static ActorAttribute Biological => new ActorAttribute(Attributes.Biological);
        public static ActorAttribute Mechanical => new ActorAttribute(Attributes.Mechanical);
        public static ActorAttribute Celestial => new ActorAttribute(Attributes.Celestial);
        public static ActorAttribute Structure => new ActorAttribute(Attributes.Structure);
        public static ActorAttribute Ranged => new ActorAttribute(Attributes.Ranged);
        
        public string ToDisplayValue() => Value.ToString();

        private ActorAttribute(Attributes @enum) : base(@enum) { }
        
        private ActorAttribute(string? from) : base(from) { }
        
        public enum Attributes
        {
            Light,
            Armoured,
            Giant,
            Biological,
            Mechanical,
            Celestial,
            Structure,
            Ranged
        }
        
        private class ActorAttributeJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ActorAttribute);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (ActorAttribute)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new ActorAttribute(value);
            }
        }
    }
}
