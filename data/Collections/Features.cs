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
                    }),

                new Feature(
                    FeatureName.PyreCargo,
                    nameof(FeatureName.PyreCargo).CamelCaseToWords(),
                    "The cargo which is attached to Pyre leaves a path of flames when moved, which " +
                    "stay until the start of the next Pyre's action or until death."),

                new Feature(
                    FeatureName.PyreFlames,
                    nameof(FeatureName.PyreFlames).CamelCaseToWords(),
                    "Any unit which starts its turn or moves onto the flames receives 5 Melee Damage.",
                    EffectName.Pyre.WallOfFlamesDamage,
                    new List<Flag>
                    {
                        Flag.Filter.Unit
                    }),
                
                new Feature(
                    FeatureName.CannonHeatUpDangerZone,
                    nameof(FeatureName.CannonHeatUpDangerZone).CamelCaseToWords(),
                    "This tile will receive massive damage on the next Cannon's turn. Until then, Cannon's " +
                    "owner has vision of this tile.",
                    null,
                    null,
                    1,
                    true)
            };
        }
    }
}
