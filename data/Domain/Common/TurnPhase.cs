using System;
using System.Collections.Generic;
using low_age_data.Common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Shared
{
    [JsonConverter(typeof(TurnPhaseJsonConverter))]
    public class TurnPhase : ValueObject<TurnPhase>
    {
        public override string ToString()
        {
            return $"{nameof(TurnPhase)}.{Value}";
        }

        public static TurnPhase Passive => new TurnPhase(TurnPhases.Passive);
        public static TurnPhase Planning => new TurnPhase(TurnPhases.Planning);
        public static TurnPhase Action => new TurnPhase(TurnPhases.Action);
        
        public string ToDisplayValue() => Value.ToString();

        private TurnPhase(TurnPhases @enum)
        {
            Value = @enum;
        }
        
        private TurnPhase(string? from)
        {
            if (from is null)
            {
                Value = TurnPhases.Passive;
                return;
            }
            
            from = from.Substring($"{nameof(TurnPhase)}.".Length);
            Value = Enum.TryParse(from, out TurnPhases terrainsEnum) 
                ? terrainsEnum 
                : TurnPhases.Passive;
        }

        private TurnPhases Value { get; }

        private enum TurnPhases
        {
            Passive,
            Planning,
            Action
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
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

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new TurnPhase(value);
            }
        }
    }
}
