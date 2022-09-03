using low_age_data.Common;

namespace low_age_data.Domain.Entities.Features
{
    public class FeatureName : EntityName
    {
        private FeatureName(string value) : base($"feature-{value}")
        {
        }

        public static FeatureName ShamanWondrousGoo => new(nameof(ShamanWondrousGoo).ToKebabCase());
        public static FeatureName PyreCargo => new(nameof(PyreCargo).ToKebabCase());
        public static FeatureName PyreFlames => new(nameof(PyreFlames).ToKebabCase());
        public static FeatureName CannonHeatUpDangerZone => new(nameof(CannonHeatUpDangerZone).ToKebabCase());
        public static FeatureName RadarResonatingSweep => new(nameof(RadarResonatingSweep).ToKebabCase());
        public static FeatureName RadarRedDot => new(nameof(RadarRedDot).ToKebabCase());
        public static FeatureName VesselFortification => new(nameof(VesselFortification).ToKebabCase());
    }
}
