using low_age_data.Common;

namespace low_age_data.Domain.Resources
{
    public class ResourceName : Name
    {
        private ResourceName(string value) : base($"resource-{value}")
        {
        }

        public static ResourceName Scraps => new($"{nameof(Scraps)}".ToKebabCase());
        public static ResourceName Celestium => new($"{nameof(Celestium)}".ToKebabCase());
        public static ResourceName WeaponStorage => new($"{nameof(WeaponStorage)}".ToKebabCase());
        public static ResourceName MeleeWeapon => new($"{nameof(MeleeWeapon)}".ToKebabCase());
        public static ResourceName RangedWeapon => new($"{nameof(RangedWeapon)}".ToKebabCase());
        public static ResourceName SpecialWeapon => new($"{nameof(SpecialWeapon)}".ToKebabCase());
        public static ResourceName Population => new($"{nameof(Population)}".ToKebabCase());
    }
}