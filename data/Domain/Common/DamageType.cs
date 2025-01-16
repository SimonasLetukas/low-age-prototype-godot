using System;
using low_age_data.Domain.Entities.Actors;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common
{
    [JsonConverter(typeof(DamageTypeJsonConverter))]
    public class DamageType : EnumValueObject<DamageType, DamageType.DamageTypes>
    {
        /// <summary>
        /// Deals damage subtracted by melee armour of the target.
        /// </summary>
        public static DamageType Melee => new DamageType(DamageTypes.Melee);
        
        /// <summary>
        /// Deals damage subtracted by ranged armour of the target.
        /// </summary>
        public static DamageType Ranged => new DamageType(DamageTypes.Ranged);
        
        /// <summary>
        /// Deals damage directly to health.
        /// </summary>
        public static DamageType Pure => new DamageType(DamageTypes.Pure);
        
        /// <summary>
        /// Any damage amount is added to the source's current melee attack damage (includes bonus). Source is the
        /// first valid <see cref="Actor"/> in the chain.
        /// </summary>
        public static DamageType CurrentMelee => new DamageType(DamageTypes.CurrentMelee);
        
        /// <summary>
        /// Any damage amount is added to the source's current ranged attack damage (includes bonus). Source is the
        /// first valid <see cref="Actor"/> in the chain.
        /// </summary>
        public static DamageType CurrentRanged => new DamageType(DamageTypes.CurrentRanged);
        
        /// <summary>
        /// Any damage amount is overwritten by the source's current melee attack damage (includes bonus).
        /// </summary>
        public static DamageType OverrideMelee => new DamageType(DamageTypes.OverrideMelee);
        
        /// <summary>
        /// Any damage amount is overwritten by the source's current ranged attack damage (includes bonus).
        /// </summary>
        public static DamageType OverrideRanged => new DamageType(DamageTypes.OverrideRanged);

        /// <summary>
        /// Any damage amount is overwritten by the target's current melee attack damage (includes bonus).
        /// </summary>
        public static DamageType TargetMelee => new DamageType(DamageTypes.TargetMelee);

        /// <summary>
        /// Any damage amount is overwritten by the target's current ranged attack damage (includes bonus).
        /// </summary>
        public static DamageType TargetRanged => new DamageType(DamageTypes.TargetRanged);

        private DamageType(DamageTypes @enum) : base(@enum) { }
        
        private DamageType(string? from) : base(from) { }
        
        public enum DamageTypes
        {
            Melee,
            Ranged,
            Pure,
            CurrentMelee, 
            CurrentRanged, 
            OverrideMelee, 
            OverrideRanged, 
            TargetMelee, 
            TargetRanged 
        }

        private class DamageTypeJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(DamageType);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (DamageType)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new DamageType(value);
            }
        }
    }
}
