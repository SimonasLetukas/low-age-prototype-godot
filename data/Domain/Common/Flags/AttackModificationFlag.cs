using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags;

[JsonConverter(typeof(AttackModificationFlagJsonConverter))]
public class AttackModificationFlag : EnumValueObject<AttackModificationFlag, AttackModificationFlag.AttackModificationFlags>
{
    public static AttackModificationFlag IgnoreArmour => new(AttackModificationFlags.IgnoreArmour);

    private AttackModificationFlag(AttackModificationFlags @enum) : base(@enum) { }
        
    private AttackModificationFlag(string? from) : base(from) { }
        
    public enum AttackModificationFlags
    {
        IgnoreArmour
    }

    private class AttackModificationFlagJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AttackModificationFlag);
        }
            
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var id = (AttackModificationFlag)value!;
            serializer.Serialize(writer, id.ToString());
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) 
                return null;
                
            var value = serializer.Deserialize<string>(reader);
            return new AttackModificationFlag(value);
        }
    }
}