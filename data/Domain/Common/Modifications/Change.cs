using System;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common.Modifications
{
    [JsonConverter(typeof(ChangeJsonConverter))]
    public class Change : EnumValueObject<Change, Change.Changes>
    {
        public static Change AddMax => new Change(Changes.AddMax);
        public static Change AddCurrent => new Change(Changes.AddCurrent);
        public static Change SubtractMax => new Change(Changes.SubtractMax);
        public static Change SubtractCurrent => new Change(Changes.SubtractCurrent);
        public static Change SetMax => new Change(Changes.SetMax);
        public static Change SetCurrent => new Change(Changes.SetCurrent);
        
        /// <summary>
        /// The result should always be rounded up by using a ceiling function.
        /// </summary>
        public static Change MultiplyMax => new Change(Changes.MultiplyMax);
        public static Change MultiplyCurrent => new Change(Changes.MultiplyCurrent);

        private Change(Changes @enum) : base(@enum) { }
        
        private Change(string? from) : base(from) { }
        
        public enum Changes
        {
            AddMax,
            AddCurrent,
            SubtractMax,
            SubtractCurrent,
            SetMax,
            SetCurrent,
            MultiplyMax,
            MultiplyCurrent
        }

        private class ChangeJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Change);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (Change)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new Change(value);
            }
        }
    }
}
