using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common
{
    [JsonConverter(typeof(ResearchIdJsonConverter))]
    public class ResearchId : Id
    {
        private ResearchId(string value) : base($"research-{value}")
        {
        }

        public static class Revelators
        {
            public static ResearchId PoisonedSlits => new ResearchId($"{nameof(Revelators)}{nameof(PoisonedSlits)}".ToKebabCase());
            public static ResearchId SpikedRope => new ResearchId($"{nameof(Revelators)}{nameof(SpikedRope)}".ToKebabCase());
            public static ResearchId QuestionableCargo => new ResearchId($"{nameof(Revelators)}{nameof(QuestionableCargo)}".ToKebabCase());
            public static ResearchId HumanfleshRations => new ResearchId($"{nameof(Revelators)}{nameof(HumanfleshRations)}".ToKebabCase());
            public static ResearchId AdaptiveDigestion => new ResearchId($"{nameof(Revelators)}{nameof(AdaptiveDigestion)}".ToKebabCase());
        }

        public static class Uee
        {
            public static ResearchId FusionCoreUpgrade => new ResearchId($"{nameof(Uee)}{nameof(FusionCoreUpgrade)}".ToKebabCase());
            public static ResearchId CelestiumCoreUpgrade => new ResearchId($"{nameof(Uee)}{nameof(CelestiumCoreUpgrade)}".ToKebabCase());
            public static ResearchId HeightenedConductivity => new ResearchId($"{nameof(Uee)}{nameof(HeightenedConductivity)}".ToKebabCase());
            public static ResearchId HoverboardReignition => new ResearchId($"{nameof(Uee)}{nameof(HoverboardReignition)}".ToKebabCase());
            public static ResearchId ExplosiveShrapnel => new ResearchId($"{nameof(Uee)}{nameof(ExplosiveShrapnel)}".ToKebabCase());
            public static ResearchId MdPractice => new ResearchId($"{nameof(Uee)}{nameof(MdPractice)}".ToKebabCase());
            public static ResearchId CelestiumCoatedMaterials => new ResearchId($"{nameof(Uee)}{nameof(CelestiumCoatedMaterials)}".ToKebabCase());
            public static ResearchId HardenedMatrix => new ResearchId($"{nameof(Uee)}{nameof(HardenedMatrix)}".ToKebabCase());
        }
        
        private class ResearchIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ResearchId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (ResearchId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new ResearchId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
