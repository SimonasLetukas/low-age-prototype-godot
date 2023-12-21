using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common.Flags
{
    [JsonConverter(typeof(ModificationFlagJsonConverter))]
    public class ModificationFlag : EnumValueObject<ModificationFlag, ModificationFlag.ModificationFlags>
    {
        public static ModificationFlag CannotBeHealed => new ModificationFlag(ModificationFlags.CannotBeHealed);
        public static ModificationFlag AbilitiesDisabled => new ModificationFlag(ModificationFlags.AbilitiesDisabled);
        public static ModificationFlag ClimbsDown => new ModificationFlag(ModificationFlags.ClimbsDown);
        public static ModificationFlag MovesThroughEnemyUnits => new ModificationFlag(ModificationFlags.MovesThroughEnemyUnits);
        public static ModificationFlag MustExecuteAttack => new ModificationFlag(ModificationFlags.MustExecuteAttack);
        public static ModificationFlag CannotAttack => new ModificationFlag(ModificationFlags.CannotAttack);
        public static ModificationFlag MovementDisabled => new ModificationFlag(ModificationFlags.MovementDisabled);
        public static ModificationFlag CanAttackAnyTeam => new ModificationFlag(ModificationFlags.CanAttackAnyTeam);
        public static ModificationFlag IgnoreArmour => new ModificationFlag(ModificationFlags.IgnoreArmour);
        public static ModificationFlag ProvidesVision => new ModificationFlag(ModificationFlags.ProvidesVision);
        public static ModificationFlag OnlyVisibleToAllies => new ModificationFlag(ModificationFlags.OnlyVisibleToAllies);
        public static ModificationFlag OnlyVisibleInFogOfWar => new ModificationFlag(ModificationFlags.OnlyVisibleInFogOfWar);
            
        /// <summary>
        /// Includes <see cref="MovementDisabled"/>, <see cref="CannotAttack"/>, <see cref="AbilitiesDisabled"/>.
        /// Action turn is always skipped.
        /// </summary>
        public static ModificationFlag FullyDisabled => new ModificationFlag(ModificationFlags.FullyDisabled);

        private ModificationFlag(ModificationFlags @enum) : base(@enum) { }
        
        private ModificationFlag(string? from) : base(from) { }
        
        public enum ModificationFlags
        {
            CannotBeHealed,
            AbilitiesDisabled,
            ClimbsDown,
            MovesThroughEnemyUnits,
            MustExecuteAttack,
            CannotAttack,
            MovementDisabled,
            CanAttackAnyTeam,
            IgnoreArmour,
            ProvidesVision,
            OnlyVisibleToAllies,
            OnlyVisibleInFogOfWar,
            FullyDisabled
        }

        private class ModificationFlagJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ModificationFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (ModificationFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new ModificationFlag(value);
            }
        }
    }
}