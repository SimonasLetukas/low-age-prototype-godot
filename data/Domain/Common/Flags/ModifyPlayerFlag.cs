using System;
using LowAgeData.Shared;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags
{
    [JsonConverter(typeof(ModifyPlayerFlagJsonConverter))]
    public class ModifyPlayerFlag : EnumValueObject<ModifyPlayerFlag, ModifyPlayerFlag.ModifyPlayerFlags>
    {
        public static ModifyPlayerFlag GameLost => new ModifyPlayerFlag(ModifyPlayerFlags.GameLost);

        private ModifyPlayerFlag(ModifyPlayerFlags @enum) : base(@enum) { }
        
        private ModifyPlayerFlag(string? from) : base(from) { }
        
        public enum ModifyPlayerFlags
        {
            GameLost
        }

        private class ModifyPlayerFlagJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ModifyPlayerFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (ModifyPlayerFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new ModifyPlayerFlag(value);
            }
        }
    }
}