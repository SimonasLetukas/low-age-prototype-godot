using low_age_data.Common;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Doodads;
using low_age_data.Domain.Shared.Flags;
using System.Collections.Generic;
using low_age_data.Domain.Shared.Filters;

namespace low_age_data.Collections
{
    public static class Doodads
    {
        public static List<Doodad> Get()
        {
            return new List<Doodad>
            {
                new Doodad(
                    id: DoodadId.ShamanWondrousGoo,
                    displayName: nameof(DoodadId.ShamanWondrousGoo).CamelCaseToWords(),
                    description: "Any unit in this area is contaminated: has its vision and Attack Distance " +
                                 "reduced by 3 (total minimum of 1) and receives 1 Pure Damage at the start of its turn.",
                    sprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png", // TODO
                    onCollisionEffect: EffectId.Shaman.WondrousGooSearch,
                    collisionFilters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    onlyOneCanExist: true),

                new Doodad(
                    id: DoodadId.PyreCargo,
                    displayName: nameof(DoodadId.PyreCargo).CamelCaseToWords(),
                    description: "The cargo which is attached to Pyre leaves a path of flames when moved, which " +
                                 "stay until the start of the next Pyre's action or until death.",
                    sprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png", // TODO
                    canBeAttacked: true,
                    occupiesSpace: true,
                    onlyOneCanExist: true),

                new Doodad(
                    id: DoodadId.PyreFlames,
                    displayName: nameof(DoodadId.PyreFlames).CamelCaseToWords(),
                    description: "Any unit which starts its turn or moves onto the flames receives 5 Melee Damage.",
                    sprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png", // TODO
                    onCollisionEffect: EffectId.Pyre.WallOfFlamesDamage,
                    collisionFilters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    onlyOneCanExist: true),

                new Doodad(
                    id: DoodadId.CannonHeatUpDangerZone,
                    displayName: nameof(DoodadId.CannonHeatUpDangerZone).CamelCaseToWords(),
                    description:
                    "This tile will receive massive damage on the next Cannon's turn. Until then, Cannon's " +
                    "owner has vision of this tile.",
                    sprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png", // TODO
                    size: 1,
                    statsCopiedFromSource: true,
                    alliesCanStack: true),

                new Doodad(
                    id: DoodadId.RadarResonatingSweep,
                    displayName: nameof(DoodadId.RadarResonatingSweep).CamelCaseToWords(),
                    description:
                    "These tiles are revealed for Radar's owner until the start of the next planning phase.",
                    sprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png", // TODO
                    size: 3,
                    alliesCanStack: true),

                new Doodad(
                    id: DoodadId.RadarRedDot,
                    displayName: nameof(DoodadId.RadarRedDot).CamelCaseToWords(),
                    description: "This red dot shows where enemy unit is currently located inside the fog of war.",
                    sprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png", // TODO
                    size: 1),

                new Doodad(
                    id: DoodadId.VesselFortification,
                    displayName: nameof(DoodadId.VesselFortification).CamelCaseToWords(),
                    description:
                    "Provides +3 Melee Armour and +3 Range Armour to all friendly units until the start of " +
                    "Vessel's action.",
                    sprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png", // TODO
                    periodicEffect: EffectId.Vessel.FortifySearch,
                    size: 7,
                    alliesCanStack: true,
                    onlyOneCanExist: false),

                new Doodad(
                    id: DoodadId.OmenRendition,
                    displayName: nameof(DoodadId.OmenRendition).CamelCaseToWords(),
                    description:
                    "50% of all damage done to this rendition will be done as Pure Damage to the original " +
                    "target.",
                    sprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png", // TODO
                    canBeAttacked: true,
                    occupiesSpace: true,
                    statsCopiedFromSource: true)
            };
        }
    }
}