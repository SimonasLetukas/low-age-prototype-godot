using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags
{
    [JsonConverter(typeof(ModifyPlayerFlagJsonConverter))]
    public class PlayerModificationFlag : EnumValueObject<PlayerModificationFlag, PlayerModificationFlag.PlayerModificationFlags>
    {
        public static PlayerModificationFlag GameLost => new(PlayerModificationFlags.GameLost);

        private PlayerModificationFlag(PlayerModificationFlags @enum) : base(@enum) { }
        
        private PlayerModificationFlag(string? from) : base(from) { }
        
        public enum PlayerModificationFlags
        {
            GameLost
        }

        private class ModifyPlayerFlagJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(PlayerModificationFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (PlayerModificationFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new PlayerModificationFlag(value);
            }
        }
    }
}