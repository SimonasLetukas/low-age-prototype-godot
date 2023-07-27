using System;
using System.Collections.Generic;
using low_age_data.Common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Shared
{
    [JsonConverter(typeof(ActorAttributeJsonConverter))]
    public class ActorAttribute : ValueObject<ActorAttribute>
    {
        public override string ToString()
        {
            return $"{nameof(ActorAttribute)}.{Value}";
        }

        public static ActorAttribute Light => new ActorAttribute(Attributes.Light);
        public static ActorAttribute Armoured => new ActorAttribute(Attributes.Armoured);
        public static ActorAttribute Giant => new ActorAttribute(Attributes.Giant);
        public static ActorAttribute Biological => new ActorAttribute(Attributes.Biological);
        public static ActorAttribute Mechanical => new ActorAttribute(Attributes.Mechanical);
        public static ActorAttribute Celestial => new ActorAttribute(Attributes.Celestial);
        public static ActorAttribute Structure => new ActorAttribute(Attributes.Structure);
        public static ActorAttribute Ranged => new ActorAttribute(Attributes.Ranged);
        
        public string ToDisplayValue() => Value.ToString();

        private ActorAttribute(Attributes @enum)
        {
            Value = @enum;
        }
        
        private ActorAttribute(string? from)
        {
            if (from is null)
            {
                Value = Attributes.Light;
                return;
            }
            
            from = from.Substring($"{nameof(ActorAttribute)}.".Length);
            Value = Enum.TryParse(from, out Attributes terrainsEnum) 
                ? terrainsEnum 
                : Attributes.Light;
        }

        private Attributes Value { get; }

        private enum Attributes
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

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
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

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new ActorAttribute(value);
            }
        }
    }
}
