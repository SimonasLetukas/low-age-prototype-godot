using low_age_data.Common;

namespace low_age_data.Domain.Entities.Features
{
    public class FeatureId : EntityId
    {
        private FeatureId(string value) : base($"feature-{value}")
        {
        }

        public static FeatureId ShamanWondrousGoo => new FeatureId(nameof(ShamanWondrousGoo).ToKebabCase());
        public static FeatureId PyreCargo => new FeatureId(nameof(PyreCargo).ToKebabCase());
        public static FeatureId PyreFlames => new FeatureId(nameof(PyreFlames).ToKebabCase());
        public static FeatureId CannonHeatUpDangerZone => new FeatureId(nameof(CannonHeatUpDangerZone).ToKebabCase());
        public static FeatureId RadarResonatingSweep => new FeatureId(nameof(RadarResonatingSweep).ToKebabCase());
        public static FeatureId RadarRedDot => new FeatureId(nameof(RadarRedDot).ToKebabCase());
        public static FeatureId VesselFortification => new FeatureId(nameof(VesselFortification).ToKebabCase());
        public static FeatureId OmenRendition => new FeatureId(nameof(OmenRendition).ToKebabCase());
    }
}
