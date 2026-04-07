using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Researches;

[JsonConverter(typeof(ResearchIdJsonConverter))]
public class ResearchId : Id
{
    [JsonConstructor]
    public ResearchId(string value, bool addPrefix = false) : base(addPrefix ? $"research-{value}" : value)
    {
    }

    public static class Revelators
    {
        public static ResearchId PoisonedSlits => new($"{nameof(Revelators)}{nameof(PoisonedSlits)}".ToKebabCase(), true);
        public static ResearchId RadioactiveChyme => new($"{nameof(Revelators)}{nameof(RadioactiveChyme)}".ToKebabCase(), true);
        public static ResearchId SpikedRope => new($"{nameof(Revelators)}{nameof(SpikedRope)}".ToKebabCase(), true);
        public static ResearchId QuestionableCargo => new($"{nameof(Revelators)}{nameof(QuestionableCargo)}".ToKebabCase(), true);
        public static ResearchId HumanfleshRations => new($"{nameof(Revelators)}{nameof(HumanfleshRations)}".ToKebabCase(), true);
        public static ResearchId AdaptiveDigestion => new($"{nameof(Revelators)}{nameof(AdaptiveDigestion)}".ToKebabCase(), true);
    }

    public static class Uee
    {
        public static ResearchId FusionCoreUpgrade => new($"{nameof(Uee)}{nameof(FusionCoreUpgrade)}".ToKebabCase(), true);
        public static ResearchId CelestiumCoreUpgrade => new($"{nameof(Uee)}{nameof(CelestiumCoreUpgrade)}".ToKebabCase(), true);
        public static ResearchId HeightenedConductivity => new($"{nameof(Uee)}{nameof(HeightenedConductivity)}".ToKebabCase(), true);
        public static ResearchId HoverboardReignition => new($"{nameof(Uee)}{nameof(HoverboardReignition)}".ToKebabCase(), true);
        public static ResearchId ExplosiveShrapnel => new($"{nameof(Uee)}{nameof(ExplosiveShrapnel)}".ToKebabCase(), true);
        public static ResearchId MdPractice => new($"{nameof(Uee)}{nameof(MdPractice)}".ToKebabCase(), true);
        public static ResearchId CelestiumCoatedMaterials => new($"{nameof(Uee)}{nameof(CelestiumCoatedMaterials)}".ToKebabCase(), true);
        public static ResearchId HardenedMatrix => new($"{nameof(Uee)}{nameof(HardenedMatrix)}".ToKebabCase(), true);
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