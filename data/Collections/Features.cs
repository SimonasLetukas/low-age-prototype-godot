using low_age_data.Common;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Features;
using low_age_data.Domain.Shared.Flags;
using System.Collections.Generic;

namespace low_age_data.Collections
{
    public static class Features
    {
        public static List<Feature> Get()
        {
            return new List<Feature>
            {
                new Feature(
                    FeatureName.WondrousGoo,
                    nameof(FeatureName.WondrousGoo).CamelCaseToWords(),
                    "Any unit in this area is contaminated: has its vision and Attack Distance " +
                    "reduced by 3 (total minimum of 1) and receives 1 Pure Damage at the start of its turn.",
                    EffectName.Shaman.WondrousGooSearch,
                    new List<Flag>
                    {
                        Flag.Filter.Unit
                    })
            };
        }
    }
}
