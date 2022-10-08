using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Resources : ValueObject<Resources>
    {
        public override string ToString()
        {
            return $"{nameof(Resources)}.{Value}";
        }

        // TODO: maybe let's not hardcode resource behaviour and have another Collection?
        // Same with factions and their starts
        public static Resources Scraps => new(ResourcesEnum.Scraps);
        public static Resources Celestium => new(ResourcesEnum.Celestium);
        public static Resources MeleeWeapon => new(ResourcesEnum.MeleeWeapon);
        public static Resources RangedWeapon => new(ResourcesEnum.RangedWeapon);
        public static Resources SpecialWeapon => new(ResourcesEnum.SpecialWeapon);
        public static Resources Population => new(ResourcesEnum.Population);

        private Resources(ResourcesEnum @enum)
        {
            Value = @enum;
        }

        private ResourcesEnum Value { get; }

        private enum ResourcesEnum
        {
            Scraps,
            Celestium,
            MeleeWeapon,
            RangedWeapon,
            SpecialWeapon,
            Population
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
