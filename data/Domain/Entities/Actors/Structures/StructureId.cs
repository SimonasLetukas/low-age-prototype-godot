using System;
using low_age_data.Common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Entities.Actors.Structures
{
    [JsonConverter(typeof(StructureIdJsonConverter))]
    public class StructureId : EntityId
    {
        private StructureId(string value) : base($"structure-{value}")
        {
        }

        public static StructureId Citadel => new StructureId(nameof(Citadel).ToKebabCase());
        public static StructureId Hut => new StructureId(nameof(Hut).ToKebabCase());
        public static StructureId Obelisk => new StructureId(nameof(Obelisk).ToKebabCase());
        public static StructureId Shack => new StructureId(nameof(Shack).ToKebabCase());
        public static StructureId Smith => new StructureId(nameof(Smith).ToKebabCase());
        public static StructureId Fletcher => new StructureId(nameof(Fletcher).ToKebabCase());
        public static StructureId Alchemy => new StructureId(nameof(Alchemy).ToKebabCase());
        public static StructureId Depot => new StructureId(nameof(Depot).ToKebabCase());
        public static StructureId Workshop => new StructureId(nameof(Workshop).ToKebabCase());
        public static StructureId Outpost => new StructureId(nameof(Outpost).ToKebabCase());
        public static StructureId Barricade => new StructureId(nameof(Barricade).ToKebabCase());

        public static StructureId BatteryCore => new StructureId(nameof(BatteryCore).ToKebabCase());
        public static StructureId FusionCore => new StructureId(nameof(FusionCore).ToKebabCase());
        public static StructureId CelestiumCore => new StructureId(nameof(CelestiumCore).ToKebabCase());
        public static StructureId Collector => new StructureId(nameof(Collector).ToKebabCase());
        public static StructureId Extractor => new StructureId(nameof(Extractor).ToKebabCase());
        public static StructureId PowerPole => new StructureId(nameof(PowerPole).ToKebabCase());
        public static StructureId Temple => new StructureId(nameof(Temple).ToKebabCase());
        public static StructureId MilitaryBase => new StructureId(nameof(MilitaryBase).ToKebabCase());
        public static StructureId Factory => new StructureId(nameof(Factory).ToKebabCase());
        public static StructureId Laboratory => new StructureId(nameof(Laboratory).ToKebabCase());
        public static StructureId Armoury => new StructureId(nameof(Armoury).ToKebabCase());
        public static StructureId Wall => new StructureId(nameof(Wall).ToKebabCase());
        public static StructureId Stairs => new StructureId(nameof(Stairs).ToKebabCase());
        public static StructureId Gate => new StructureId(nameof(Gate).ToKebabCase());
        public static StructureId Watchtower => new StructureId(nameof(Watchtower).ToKebabCase());
        public static StructureId Bastion => new StructureId(nameof(Bastion).ToKebabCase());
        
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

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new StructureId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
