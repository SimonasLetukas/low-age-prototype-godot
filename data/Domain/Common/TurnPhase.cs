using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common
{
    [JsonConverter(typeof(TurnPhaseJsonConverter))]
    public class TurnPhase : EnumValueObject<TurnPhase, TurnPhase.TurnPhases>
    {
        public static TurnPhase Passive => new(TurnPhases.Passive);
        public static TurnPhase Planning => new(TurnPhases.Planning);
        public static TurnPhase Action => new(TurnPhases.Action);
        
        public string ToDisplayValue() => Value.ToString();

        public TurnPhase Next()
        {
            return Value switch
            {
                TurnPhases.Planning => Action,
                TurnPhases.Action => Planning,
                TurnPhases.Passive => Passive,
                _ => Passive
            };
        }

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
