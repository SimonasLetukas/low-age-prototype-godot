using low_age_data.Common;

namespace low_age_data.Domain.Entities.Features
{
    public class FeatureName : EntityName
    {
        private FeatureName(string value) : base($"feature-{value}")
        {
        }

        public static FeatureName ShamanWondrousGoo => new FeatureName(nameof(ShamanWondrousGoo).ToKebabCase());
        public static FeatureName PyreCargo => new FeatureName(nameof(PyreCargo).ToKebabCase());
        public static FeatureName PyreFlames => new FeatureName(nameof(PyreFlames).ToKebabCase());
        public static FeatureName CannonHeatUpDangerZone => new FeatureName(nameof(CannonHeatUpDangerZone).ToKebabCase());
        public static FeatureName RadarResonatingSweep => new FeatureName(nameof(RadarResonatingSweep).ToKebabCase());
    }
}
