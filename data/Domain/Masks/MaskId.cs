using low_age_data.Common;

namespace low_age_data.Domain.Masks
{
    public class MaskId : Id
    {
        private MaskId(string value) : base($"mask-{value}")
        {
        }

        public static MaskId Power => new MaskId($"{nameof(Power)}".ToKebabCase());
    }
}