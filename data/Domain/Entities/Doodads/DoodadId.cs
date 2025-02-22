using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Entities.Doodads
{
    [JsonConverter(typeof(DoodadIdJsonConverter))]
    public class DoodadId : EntityId
    {
        [JsonConstructor]
        public DoodadId(string value, bool addPrefix = false) : base(addPrefix ? $"doodad-{value}" : value)
        {
        }

        public static DoodadId ShamanWondrousGoo => new DoodadId(nameof(ShamanWondrousGoo).ToKebabCase(), true);
        public static DoodadId PyreCargo => new DoodadId(nameof(PyreCargo).ToKebabCase(), true);
        public static DoodadId PyreFlames => new DoodadId(nameof(PyreFlames).ToKebabCase(), true);
        public static DoodadId CannonHeatUpDangerZone => new DoodadId(nameof(CannonHeatUpDangerZone).ToKebabCase(), true);
        public static DoodadId RadarResonatingSweep => new DoodadId(nameof(RadarResonatingSweep).ToKebabCase(), true);
        public static DoodadId RadarRedDot => new DoodadId(nameof(RadarRedDot).ToKebabCase(), true);
        public static DoodadId VesselFortification => new DoodadId(nameof(VesselFortification).ToKebabCase(), true);
        public static DoodadId OmenRendition => new DoodadId(nameof(OmenRendition).ToKebabCase(), true);
        
        private class DoodadIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(DoodadId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (DoodadId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new DoodadId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
