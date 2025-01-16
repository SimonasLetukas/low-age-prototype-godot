using System;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common
{
    [JsonConverter(typeof(AlignmentJsonConverter))]
    public class Alignment : EnumValueObject<Alignment, Alignment.Alignments>
    {
        public static Alignment Positive => new Alignment(Alignments.Positive);
        public static Alignment Neutral => new Alignment(Alignments.Neutral);
        public static Alignment Negative => new Alignment(Alignments.Negative);

        private Alignment(Alignments @enum) : base(@enum) { }
        
        private Alignment(string? from) : base(from) { }
        
        public enum Alignments
        {
            Positive,
            Neutral,
            Negative
        }

        private class AlignmentJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Alignment);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (Alignment)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new Alignment(value);
            }
        }
    }
}
