using System;
using low_age_data.Common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Resources
{
    [JsonConverter(typeof(ResourceIdJsonConverter))]
    public class ResourceId : Id
    {
        private ResourceId(string value) : base($"resource-{value}")
        {
        }

        public static ResourceId Scraps => new ResourceId($"{nameof(Scraps)}".ToKebabCase());
        public static ResourceId Celestium => new ResourceId($"{nameof(Celestium)}".ToKebabCase());
        public static ResourceId WeaponStorage => new ResourceId($"{nameof(WeaponStorage)}".ToKebabCase());
        public static ResourceId MeleeWeapon => new ResourceId($"{nameof(MeleeWeapon)}".ToKebabCase());
        public static ResourceId RangedWeapon => new ResourceId($"{nameof(RangedWeapon)}".ToKebabCase());
        public static ResourceId SpecialWeapon => new ResourceId($"{nameof(SpecialWeapon)}".ToKebabCase());
        public static ResourceId Population => new ResourceId($"{nameof(Population)}".ToKebabCase());
        public static ResourceId Faith => new ResourceId($"{nameof(Faith)}".ToKebabCase());
        
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

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new ResourceId(value ?? throw new InvalidOperationException());
            }
        }
    }
}