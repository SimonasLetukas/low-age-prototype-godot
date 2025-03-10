﻿using LowAgeData.Domain.Logic;
using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags
{
    [JsonConverter(typeof(ConditionFlagJsonConverter))]
    public class ConditionFlag : EnumValueObject<ConditionFlag, ConditionFlag.ConditionFlags>
    {
        /// <summary>
        /// Used in <see cref="MaskCondition"/>, <see cref="EntityCondition"/> or <see cref="BehaviourCondition"/>.
        /// </summary>
        public static ConditionFlag Exists => new ConditionFlag(ConditionFlags.Exists);
        
        /// <summary>
        /// Used in <see cref="MaskCondition"/>, <see cref="EntityCondition"/> or <see cref="BehaviourCondition"/>.
        /// </summary>
        public static ConditionFlag DoesNotExist => new ConditionFlag(ConditionFlags.DoesNotExist);
        
        public static ConditionFlag TargetDoesNotHaveFullHealth => new ConditionFlag(ConditionFlags.TargetDoesNotHaveFullHealth);
        public static ConditionFlag NoActorsFoundFromEffect => new ConditionFlag(ConditionFlags.NoActorsFoundFromEffect);
        public static ConditionFlag TargetIsLowGround => new ConditionFlag(ConditionFlags.TargetIsLowGround);
        public static ConditionFlag TargetIsHighGround => new ConditionFlag(ConditionFlags.TargetIsHighGround);
        public static ConditionFlag TargetIsUnoccupied => new ConditionFlag(ConditionFlags.TargetIsUnoccupied);
        public static ConditionFlag TargetIsDifferentTypeThanOrigin => new ConditionFlag(ConditionFlags.TargetIsDifferentTypeThanOrigin);

        private ConditionFlag(ConditionFlags @enum) : base(@enum) { }
        
        private ConditionFlag(string? from) : base(from) { }
        
        public enum ConditionFlags
        {
            Exists,
            DoesNotExist,
            TargetDoesNotHaveFullHealth,
            NoActorsFoundFromEffect,
            TargetIsLowGround,
            TargetIsHighGround,
            TargetIsUnoccupied,
            TargetIsDifferentTypeThanOrigin
        }

        private class ConditionFlagJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ConditionFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (ConditionFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new ConditionFlag(value);
            }
        }
    }
}