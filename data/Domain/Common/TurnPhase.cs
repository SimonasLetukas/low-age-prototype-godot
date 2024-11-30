using System;
using low_age_data.Shared;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common
{
    [JsonConverter(typeof(TurnPhaseJsonConverter))]
    public class TurnPhase : EnumValueObject<TurnPhase, TurnPhase.TurnPhases>
    {
        public static TurnPhase Passive => new TurnPhase(TurnPhases.Passive);
        public static TurnPhase Planning => new TurnPhase(TurnPhases.Planning);
        public static TurnPhase Action => new TurnPhase(TurnPhases.Action);
        
        public string ToDisplayValue() => Value.ToString();

        private TurnPhase(TurnPhases @enum) : base(@enum) { }
        
        private TurnPhase(string? from) : base(from) { }
        
        public enum TurnPhases
        {
            Passive,
            Planning,
            Action
        }
        
        private class TurnPhaseJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TurnPhase);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (TurnPhase)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new TurnPhase(value);
            }
        }
    }
}
