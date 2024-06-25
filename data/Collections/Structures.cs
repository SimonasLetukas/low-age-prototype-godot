using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Factions;
using low_age_data.Shared;

namespace low_age_data.Collections
{
    public static class StructuresCollection
    {
        public static List<Structure> Get()
        {
            return new List<Structure>
            {
                new Structure(
                    id: StructureId.Citadel,
                    displayName: nameof(StructureId.Citadel).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/revs/boss post front indexed 2x3.png",
                    centerOffset: new Vector2<int>(30, 22),
                    backSideSprite: "res://assets/sprites/structures/revs/boss post back indexed 2x3.png",
                    backSideCenterOffset: new Vector2<int>(14, 14),
                    flattenedSprite: "res://assets/sprites/structures/revs/boss post bottom indexed 2x3.png",
                    flattenedCenterOffset: new Vector2<int>(28, 15),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>(),
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.PassiveIncome,
                        AbilityId.Citadel.ExecutiveStash,
                        AbilityId.Citadel.Ascendable,
                        AbilityId.Citadel.HighGround,
                        AbilityId.Citadel.PromoteGoons
                    },
                    size: new Vector2<int>(x: 4, y: 3),
                    centerPoint: new Vector2<int>(x: 1, y: 1),
                    destructible: false,
                    walkableAreas: new List<Area>
                    {
                        new Area(
                            start: new Vector2<int>(x: 2, y: 0),
                            size: new Vector2<int>(x: 2, y: 3))
                    }),
                
                new Structure(
                    id: StructureId.Hut,
                    displayName: nameof(StructureId.Hut).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Hut).CamelCaseToWords()} brings Scraps income and should be " +
                                 $"built on top of Scraps resources.",
                    sprite: "res://assets/sprites/structures/revs/collectors hut indexed 2x2.png",
                    centerOffset: new Vector2<int>(15, 14),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Light,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Hut.Building,
                        AbilityId.Shared.ScrapsIncome
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    id: StructureId.Obelisk,
                    displayName: nameof(StructureId.Obelisk).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Obelisk).CamelCaseToWords()} provides Celestium income and " +
                                 $"should be built on top of Celestium resources",
                    sprite: "res://assets/sprites/structures/revs/obelisk indexed 2x2 or 1x1.png",
                    centerOffset: new Vector2<int>(8, 29),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 3, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Obelisk.Building,
                        AbilityId.Shared.CelestiumIncome,
                        AbilityId.Obelisk.CelestiumDischarge
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    id: StructureId.Shack,
                    displayName: nameof(StructureId.Shack).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Shack).CamelCaseToWords()} houses Population needed for new " +
                                 $"unit production.",
                    sprite: "res://assets/sprites/structures/revs/shack indexed 1x1.png",
                    centerOffset: new Vector2<int>(7, 11),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 35, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Light,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.BuildingStructure,
                        AbilityId.Shack.Accommodation
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    id: StructureId.Smith,
                    displayName: nameof(StructureId.Smith).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Smith).CamelCaseToWords()} works on Melee Weapon production.",
                    sprite: "res://assets/sprites/structures/revs/smith indexed 1x2.png",
                    centerOffset: new Vector2<int>(12, 10),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 75, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.BuildingStructure,
                        AbilityId.Smith.MeleeWeaponProduction,
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    id: StructureId.Fletcher,
                    displayName: nameof(StructureId.Fletcher).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Fletcher).CamelCaseToWords()} works on Ranged Weapon production.",
                    sprite: "res://assets/sprites/structures/revs/fletchery indexed 2x2.png",
                    centerOffset: new Vector2<int>(15, 14),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 80, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.BuildingStructure,
                        AbilityId.Fletcher.RangedWeaponProduction,
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    id: StructureId.Alchemy,
                    displayName: nameof(StructureId.Alchemy).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Alchemy).CamelCaseToWords()} works on Special Weapon production.",
                    sprite: "res://assets/sprites/structures/revs/alchemy indexed 2x3.png",
                    centerOffset: new Vector2<int>(16, 17),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 90, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.BuildingStructure,
                        AbilityId.Alchemy.SpecialWeaponProduction,
                    },
                    size: new Vector2<int>(x: 2, y: 3)),
                
                new Structure(
                    id: StructureId.Depot,
                    displayName: nameof(StructureId.Depot).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Depot).CamelCaseToWords()} provides storage to all Weapons.",
                    sprite: "res://assets/sprites/structures/revs/depot indexed 2x2.png",
                    centerOffset: new Vector2<int>(14, 20),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.BuildingStructure,
                        AbilityId.Depot.WeaponStorage,
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    id: StructureId.Workshop,
                    displayName: nameof(StructureId.Workshop).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Workshop).CamelCaseToWords()} can be used to research various " +
                                 $"upgrades.",
                    sprite: "res://assets/sprites/structures/revs/workshop indexed 2x2.png",
                    centerOffset: new Vector2<int>(11, 18),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 55, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.BuildingStructure,
                        AbilityId.Workshop.Research
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    id: StructureId.Outpost,
                    displayName: nameof(StructureId.Outpost).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Outpost).CamelCaseToWords()} can be climbed and used as a tower.",
                    sprite: "res://assets/sprites/structures/revs/outpost indexed 1x1.png",
                    centerOffset: new Vector2<int>(8, 17),
                    flattenedSprite: "res://assets/sprites/structures/revs/outpost bottom indexed 1x1.png",
                    flattenedCenterOffset: new Vector2<int>(6, 6),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 90, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.BuildingStructure,
                        AbilityId.Outpost.Ascendable,
                        AbilityId.Outpost.HighGround
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    id: StructureId.Barricade,
                    displayName: nameof(StructureId.Barricade).CamelCaseToWords(),
                    description: $"{nameof(StructureId.Barricade).CamelCaseToWords()} helps to defend against ranged " +
                                 $"attacks and acts as an obstacle for melee units.",
                    sprite: "res://assets/sprites/structures/revs/barricade front indexed 2x1.png",
                    centerOffset: new Vector2<int>(12, 16),
                    backSideSprite: "res://assets/sprites/structures/revs/barricade back indexed 2x1.png",
                    backSideCenterOffset: new Vector2<int>(15, 11),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Light,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.BuildingStructure,
                        AbilityId.Barricade.ProtectiveShield,
                        AbilityId.Barricade.Caltrops,
                        AbilityId.Barricade.Decompose
                    },
                    size: new Vector2<int>(x: 2, y: 1)),
                
                new Structure(
                    id: StructureId.BatteryCore,
                    displayName: nameof(StructureId.BatteryCore).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/core low 3 indexed 3x3.png",
                    centerOffset: new Vector2<int>(19, 10),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.PowerGenerator,
                        AbilityId.Shared.PassiveIncome,
                        AbilityId.BatteryCore.PowerGrid,
                        AbilityId.Shared.Uee.Build,
                        AbilityId.BatteryCore.FusionCoreUpgrade
                    },
                    size: new Vector2<int>(x: 3, y: 3)),
                
                new Structure(
                    id: StructureId.FusionCore,
                    displayName: nameof(StructureId.FusionCore).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/core mid 3 indexed 3x3.png",
                    centerOffset: new Vector2<int>(17, 13),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 25, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.PowerGenerator,
                        AbilityId.Shared.PassiveIncome,
                        AbilityId.FusionCore.PowerGrid,
                        AbilityId.Shared.Uee.Build,
                        AbilityId.FusionCore.DefenceProtocol,
                        AbilityId.FusionCore.CelestiumCoreUpgrade
                    },
                    size: new Vector2<int>(x: 3, y: 3)),
                
                new Structure(
                    id: StructureId.CelestiumCore,
                    displayName: nameof(StructureId.CelestiumCore).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/core top 3 indexed 3x3.png",
                    centerOffset: new Vector2<int>(15, 19),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.PowerGenerator,
                        AbilityId.Shared.PassiveIncome,
                        AbilityId.CelestiumCore.PowerGrid,
                        AbilityId.Shared.Uee.Build,
                        AbilityId.CelestiumCore.DefenceProtocol,
                        AbilityId.CelestiumCore.HeightenedConductivity
                    },
                    size: new Vector2<int>(x: 3, y: 3)),
                
                new Structure(
                    id: StructureId.Collector,
                    displayName: nameof(StructureId.Collector).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/collector1 indexed 2x2.png",
                    centerOffset: new Vector2<int>(9, 15),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 40, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 30, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Light,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Collector.Building,
                        AbilityId.Shared.ScrapsIncome,
                        AbilityId.Collector.DirectTransitSystem
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    id: StructureId.Extractor,
                    displayName: nameof(StructureId.Extractor).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/extractor3 indexed 2x2.png",
                    centerOffset: new Vector2<int>(11, 17),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 70, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 15, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Extractor.Building,
                        AbilityId.Shared.CelestiumIncome,
                        AbilityId.Extractor.ReinforcedInfrastructure
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    id: StructureId.PowerPole,
                    displayName: nameof(StructureId.PowerPole).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/pole3 indexed 1x1.png",
                    centerOffset: new Vector2<int>(4, 17),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 6, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Light,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.PowerPole.PowerGrid,
                        AbilityId.PowerPole.ExcessDistribution,
                        AbilityId.PowerPole.ImprovedPowerGrid
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    id: StructureId.Temple,
                    displayName: nameof(StructureId.Temple).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/faith indexed 2x2 or 1x2.png",
                    centerOffset: new Vector2<int>(9, 19),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 35, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Temple.KeepingTheFaith
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    id: StructureId.MilitaryBase,
                    displayName: nameof(StructureId.MilitaryBase).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/military base indexed 1x2.png",
                    centerOffset: new Vector2<int>(12, 11),
                    backSideCenterOffset: new Vector2<int>(5, 7),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 80, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 15, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.MilitaryBase.Train
                    },
                    size: new Vector2<int>(x: 3, y: 1),
                    walkableAreas: new List<Area>
                    {
                        new Area(
                            start: new Vector2<int>(x: 2, y: 0),
                            size: new Vector2<int>(x: 1, y: 1))
                    }),
                
                new Structure(
                    id: StructureId.Factory,
                    displayName: nameof(StructureId.Factory).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/factory front indexed 3x2.png",
                    centerOffset: new Vector2<int>(25, 26),
                    backSideSprite: "res://assets/sprites/structures/uee/factory back indexed 3x2.png",
                    backSideCenterOffset: new Vector2<int>(8, 14),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 95, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 20, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Factory.Train
                    },
                    size: new Vector2<int>(x: 5, y: 2),
                    walkableAreas: new List<Area>
                    {
                        new Area(
                            start: new Vector2<int>(x: 3, y: 0),
                            size: new Vector2<int>(x: 2, y: 2))
                    }),
                
                new Structure(
                    id: StructureId.Laboratory,
                    displayName: nameof(StructureId.Laboratory).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/lab front indexed 3x3.png",
                    centerOffset: new Vector2<int>(22, 25),
                    backSideSprite: "res://assets/sprites/structures/uee/lab back indexed 3x3.png",
                    backSideCenterOffset: new Vector2<int>(22, 25),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 110, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 30, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Laboratory.Train
                    },
                    size: new Vector2<int>(x: 3, y: 3),
                    walkableAreas: new List<Area>
                    {
                        new Area(
                            start: new Vector2<int>(x: 2, y: 1),
                            size: new Vector2<int>(x: 1, y: 1))
                    }),
                
                new Structure(
                    id: StructureId.Armoury,
                    displayName: nameof(StructureId.Armoury).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/armoury1 indexed 1x2.png",
                    centerOffset: new Vector2<int>(11, 8),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 25, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Armoury.Research
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    id: StructureId.Wall,
                    displayName: nameof(StructureId.Wall).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/wall indexed 1x1.png",
                    centerOffset: new Vector2<int>(8, 20),
                    flattenedSprite: "res://assets/sprites/structures/uee/wall bottom indexed 1x1.png",
                    flattenedCenterOffset: new Vector2<int>(8, 6),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Wall.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Wall.HighGround
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    id: StructureId.Stairs,
                    displayName: nameof(StructureId.Stairs).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/stairs front indexed 2x1.png",
                    centerOffset: new Vector2<int>(10, 21),
                    backSideSprite: "res://assets/sprites/structures/uee/stairs back indexed 2x1.png",
                    backSideCenterOffset: new Vector2<int>(9, 15),
                    flattenedSprite: "res://assets/sprites/structures/uee/stairs bottom indexed 2x1.png",
                    flattenedCenterOffset: new Vector2<int>(10, 5),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Stairs.Ascendable
                    },
                    size: new Vector2<int>(x: 2, y: 1)),
                
                new Structure(
                    id: StructureId.Gate,
                    displayName: nameof(StructureId.Gate).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/gate 0 indexed 1x4.png",
                    centerOffset: new Vector2<int>(20, 26),
                    flattenedSprite: "res://assets/sprites/structures/uee/gate bottom indexed 1x4.png",
                    flattenedCenterOffset: new Vector2<int>(20, 12),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 300, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Gate.HighGround,
                        AbilityId.Gate.Ascendable,
                        AbilityId.Gate.Entrance
                    },
                    size: new Vector2<int>(x: 1, y: 4)),
                
                new Structure(
                    id: StructureId.Watchtower,
                    displayName: nameof(StructureId.Watchtower).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/watchtower indexed 1x1.png",
                    centerOffset: new Vector2<int>(8, 27),
                    flattenedSprite: "res://assets/sprites/structures/uee/watchtower bottom indexed 1x1.png",
                    flattenedCenterOffset: new Vector2<int>(8, 8),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 95, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Watchtower.VantagePoint,
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    id: StructureId.Bastion,
                    displayName: nameof(StructureId.Bastion).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/sprites/structures/uee/tower indexed 2x2.png",
                    centerOffset: new Vector2<int>(16, 23),
                    flattenedSprite: "res://assets/sprites/structures/uee/tower bottom indexed 2x2.png",
                    flattenedCenterOffset: new Vector2<int>(12, 10),
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 550, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 3, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    actorAttributes: new List<ActorAttribute>
                    {
                        ActorAttribute.Armoured,
                        ActorAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.BuildingStructure,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Bastion.Battlement,
                    },
                    size: new Vector2<int>(x: 2, y: 2)), 
            };
        }
    }
}