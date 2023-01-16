using low_age_data.Common;

namespace low_age_data.Domain.Masks
{
    public class MaskName : Name
    {
        private MaskName(string value) : base($"mask-{value}")
        {
        }

        public static MaskName Power => new($"{nameof(Power)}".ToKebabCase());
    }
}