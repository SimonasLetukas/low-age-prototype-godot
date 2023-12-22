using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Resources
{
    [JsonConverter(typeof(ResourceIdJsonConverter))]
    public class ResourceId : Id
    {
        [JsonConstructor]
        public ResourceId(string value, bool usePrefix = false) : base(usePrefix ? $"resource-{value}" : value)
        {
        }

        public static ResourceId Scraps => new ResourceId($"{nameof(Scraps)}".ToKebabCase(), true);
        public static ResourceId Celestium => new ResourceId($"{nameof(Celestium)}".ToKebabCase(), true);
        public static ResourceId WeaponStorage => new ResourceId($"{nameof(WeaponStorage)}".ToKebabCase(), true);
        public static ResourceId MeleeWeapon => new ResourceId($"{nameof(MeleeWeapon)}".ToKebabCase(), true);
        public static ResourceId RangedWeapon => new ResourceId($"{nameof(RangedWeapon)}".ToKebabCase(), true);
        public static ResourceId SpecialWeapon => new ResourceId($"{nameof(SpecialWeapon)}".ToKebabCase(), true);
        public static ResourceId Population => new ResourceId($"{nameof(Population)}".ToKebabCase(), true);
        public static ResourceId Faith => new ResourceId($"{nameof(Faith)}".ToKebabCase(), true);
        
        private class ResourceIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ResourceId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (ResourceId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new ResourceId(value ?? throw new InvalidOperationException());
            }
        }
    }
}