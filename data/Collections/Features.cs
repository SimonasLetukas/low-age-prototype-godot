using low_age_data.Common;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Features;
using low_age_data.Domain.Shared.Flags;
using System.Collections.Generic;
using low_age_data.Domain.Shared.Filters;

namespace low_age_data.Collections
{
    public static class Features
    {
        public static List<Feature> Get()
        {
            return new List<Feature>
            {
                new(
                    name: FeatureName.ShamanWondrousGoo,
                    displayName: nameof(FeatureName.ShamanWondrousGoo).CamelCaseToWords(),
                    description: "Any unit in this area is contaminated: has its vision and Attack Distance " +
                                 "reduced by 3 (total minimum of 1) and receives 1 Pure Damage at the start of its turn.",
                    onCollisionEffect: EffectName.Shaman.WondrousGooSearch,
                    collisionFilters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    onlyOneCanExist: true),

                new(
                    name: FeatureName.PyreCargo,
                    displayName: nameof(FeatureName.PyreCargo).CamelCaseToWords(),
                    description: "The cargo which is attached to Pyre leaves a path of flames when moved, which " +
                                 "stay until the start of the next Pyre's action or until death.",
                    canBeAttacked: true,
                    occupiesSpace: true,
                    onlyOneCanExist: true),

                new(
                    name: FeatureName.PyreFlames,
                    displayName: nameof(FeatureName.PyreFlames).CamelCaseToWords(),
                    description: "Any unit which starts its turn or moves onto the flames receives 5 Melee Damage.",
                    onCollisionEffect: EffectName.Pyre.WallOfFlamesDamage,
                    collisionFilters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    onlyOneCanExist: true),

                new(
                    name: FeatureName.CannonHeatUpDangerZone,
                    displayName: nameof(FeatureName.CannonHeatUpDangerZone).CamelCaseToWords(),
                    description:
                    "This tile will receive massive damage on the next Cannon's turn. Until then, Cannon's " +
                    "owner has vision of this tile.",
                    size: 1,
                    statsCopiedFromSource: true,
                    alliesCanStack: true),

                new(
                    name: FeatureName.RadarResonatingSweep,
                    displayName: nameof(FeatureName.RadarResonatingSweep).CamelCaseToWords(),
                    description:
                    "These tiles are revealed for Radar's owner until the start of the next planning phase.",
                    size: 3,
                    alliesCanStack: true),

                new(
                    name: FeatureName.RadarRedDot,
                    displayName: nameof(FeatureName.RadarRedDot).CamelCaseToWords(),
                    description: "This red dot shows where enemy unit is currently located inside the fog of war.",
                    size: 1),

                new(
                    name: FeatureName.VesselFortification,
                    displayName: nameof(FeatureName.VesselFortification).CamelCaseToWords(),
                    description:
                    "Provides +3 Melee Armour and +3 Range Armour to all friendly units until the start of " +
                    "Vessel's action.",
                    periodicEffect: EffectName.Vessel.FortifySearch,
                    size: 7,
                    alliesCanStack: true,
                    onlyOneCanExist: false),

                new(
                    name: FeatureName.OmenRendition,
                    displayName: nameof(FeatureName.OmenRendition).CamelCaseToWords(),
                    description:
                    "50% of all damage done to this rendition will be done as Pure Damage to the original " +
                    "target.",
                    canBeAttacked: true,
                    occupiesSpace: true,
                    statsCopiedFromSource: true)
            };
        }
    }
}