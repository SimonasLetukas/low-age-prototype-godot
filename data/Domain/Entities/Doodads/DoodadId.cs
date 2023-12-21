using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Entities.Doodads
{
    [JsonConverter(typeof(DoodadIdJsonConverter))]
    public class DoodadId : EntityId
    {
        private DoodadId(string value) : base($"doodad-{value}")
        {
        }

        public static DoodadId ShamanWondrousGoo => new DoodadId(nameof(ShamanWondrousGoo).ToKebabCase());
        public static DoodadId PyreCargo => new DoodadId(nameof(PyreCargo).ToKebabCase());
        public static DoodadId PyreFlames => new DoodadId(nameof(PyreFlames).ToKebabCase());
        public static DoodadId CannonHeatUpDangerZone => new DoodadId(nameof(CannonHeatUpDangerZone).ToKebabCase());
        public static DoodadId RadarResonatingSweep => new DoodadId(nameof(RadarResonatingSweep).ToKebabCase());
        public static DoodadId RadarRedDot => new DoodadId(nameof(RadarRedDot).ToKebabCase());
        public static DoodadId VesselFortification => new DoodadId(nameof(VesselFortification).ToKebabCase());
        public static DoodadId OmenRendition => new DoodadId(nameof(OmenRendition).ToKebabCase());
        
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
