using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Entities.Actors.Structures
{
    [JsonConverter(typeof(StructureIdJsonConverter))]
    public class StructureId : EntityId
    {
        [JsonConstructor]
        public StructureId(string value, bool usePrefix = false) : base(usePrefix ? $"structure-{value}" : value)
        {
        }

        public static StructureId Citadel => new StructureId(nameof(Citadel).ToKebabCase(), true);
        public static StructureId Hut => new StructureId(nameof(Hut).ToKebabCase(), true);
        public static StructureId Obelisk => new StructureId(nameof(Obelisk).ToKebabCase(), true);
        public static StructureId Shack => new StructureId(nameof(Shack).ToKebabCase(), true);
        public static StructureId Smith => new StructureId(nameof(Smith).ToKebabCase(), true);
        public static StructureId Fletcher => new StructureId(nameof(Fletcher).ToKebabCase(), true);
        public static StructureId Alchemy => new StructureId(nameof(Alchemy).ToKebabCase(), true);
        public static StructureId Depot => new StructureId(nameof(Depot).ToKebabCase(), true);
        public static StructureId Workshop => new StructureId(nameof(Workshop).ToKebabCase(), true);
        public static StructureId Outpost => new StructureId(nameof(Outpost).ToKebabCase(), true);
        public static StructureId Barricade => new StructureId(nameof(Barricade).ToKebabCase(), true);

        public static StructureId BatteryCore => new StructureId(nameof(BatteryCore).ToKebabCase(), true);
        public static StructureId FusionCore => new StructureId(nameof(FusionCore).ToKebabCase(), true);
        public static StructureId CelestiumCore => new StructureId(nameof(CelestiumCore).ToKebabCase(), true);
        public static StructureId Collector => new StructureId(nameof(Collector).ToKebabCase(), true);
        public static StructureId Extractor => new StructureId(nameof(Extractor).ToKebabCase(), true);
        public static StructureId PowerPole => new StructureId(nameof(PowerPole).ToKebabCase(), true);
        public static StructureId Temple => new StructureId(nameof(Temple).ToKebabCase(), true);
        public static StructureId MilitaryBase => new StructureId(nameof(MilitaryBase).ToKebabCase(), true);
        public static StructureId Factory => new StructureId(nameof(Factory).ToKebabCase(), true);
        public static StructureId Laboratory => new StructureId(nameof(Laboratory).ToKebabCase(), true);
        public static StructureId Armoury => new StructureId(nameof(Armoury).ToKebabCase(), true);
        public static StructureId Wall => new StructureId(nameof(Wall).ToKebabCase(), true);
        public static StructureId Stairs => new StructureId(nameof(Stairs).ToKebabCase(), true);
        public static StructureId Gate => new StructureId(nameof(Gate).ToKebabCase(), true);
        public static StructureId Watchtower => new StructureId(nameof(Watchtower).ToKebabCase(), true);
        public static StructureId Bastion => new StructureId(nameof(Bastion).ToKebabCase(), true);
        
        private class StructureIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(StructureId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (StructureId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new StructureId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
