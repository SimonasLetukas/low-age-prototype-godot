using low_age_data.Common;

namespace low_age_data.Domain.Resources
{
    public class ResourceName : Name
    {
        private ResourceName(string value) : base($"resource-{value}")
        {
        }

        public static ResourceName Scraps => new ResourceName($"{nameof(Scraps)}".ToKebabCase());
        public static ResourceName Celestium => new ResourceName($"{nameof(Celestium)}".ToKebabCase());
        public static ResourceName WeaponStorage => new ResourceName($"{nameof(WeaponStorage)}".ToKebabCase());
        public static ResourceName MeleeWeapon => new ResourceName($"{nameof(MeleeWeapon)}".ToKebabCase());
        public static ResourceName RangedWeapon => new ResourceName($"{nameof(RangedWeapon)}".ToKebabCase());
        public static ResourceName SpecialWeapon => new ResourceName($"{nameof(SpecialWeapon)}".ToKebabCase());
        public static ResourceName Population => new ResourceName($"{nameof(Population)}".ToKebabCase());
        public static ResourceName Faith => new ResourceName($"{nameof(Faith)}".ToKebabCase());
    }
}