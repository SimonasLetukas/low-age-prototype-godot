using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags
{
    [JsonConverter(typeof(ModificationFlagJsonConverter))]
    public class EntityModificationFlag : EnumValueObject<EntityModificationFlag, EntityModificationFlag.EntityModificationFlags>
    {
        public static EntityModificationFlag CannotBeHealed => new(EntityModificationFlags.CannotBeHealed);
        public static EntityModificationFlag AbilitiesDisabled => new(EntityModificationFlags.AbilitiesDisabled);
        public static EntityModificationFlag ClimbsDown => new(EntityModificationFlags.ClimbsDown);
        public static EntityModificationFlag MovesThroughEnemyUnits => new(EntityModificationFlags.MovesThroughEnemyUnits);
        public static EntityModificationFlag MustExecuteAttack => new(EntityModificationFlags.MustExecuteAttack);
        public static EntityModificationFlag CannotAttack => new(EntityModificationFlags.CannotAttack);
        public static EntityModificationFlag MovementDisabled => new(EntityModificationFlags.MovementDisabled);
        public static EntityModificationFlag CanAttackAnyTeam => new(EntityModificationFlags.CanAttackAnyTeam);
        public static EntityModificationFlag ProvidesVision => new(EntityModificationFlags.ProvidesVision);
        public static EntityModificationFlag OnlyVisibleToAllies => new(EntityModificationFlags.OnlyVisibleToAllies);
        public static EntityModificationFlag OnlyVisibleInFogOfWar => new(EntityModificationFlags.OnlyVisibleInFogOfWar);
            
        /// <summary>
        /// Includes <see cref="MovementDisabled"/>, <see cref="CannotAttack"/>, <see cref="AbilitiesDisabled"/>.
        /// Action turn is always skipped.
        /// </summary>
        public static EntityModificationFlag FullyDisabled => new(EntityModificationFlags.FullyDisabled);

        private EntityModificationFlag(EntityModificationFlags @enum) : base(@enum) { }
        
        private EntityModificationFlag(string? from) : base(from) { }
        
        public enum EntityModificationFlags
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
                return objectType == typeof(EntityModificationFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (EntityModificationFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new EntityModificationFlag(value);
            }
        }
    }
}