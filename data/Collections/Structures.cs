using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Shared;

namespace low_age_data.Collections
{
    public static class Structures
    {
        public static List<Structure> Get()
        {
            return new List<Structure>
            {
                new Structure(
                    id: StructureId.Citadel,
                    displayName: nameof(StructureId.Citadel).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>(),
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.PassiveIncome,
                        AbilityId.Citadel.ExecutiveStash,
                        AbilityId.Citadel.Ascendable,
                        AbilityId.Citadel.HighGround,
                        AbilityId.Citadel.PromoteGoons
                    },
                    size: new Vector2<int>(x: 3, y: 4),
                    centerPoint: new Vector2<int>(x: 1, y: 1),
                    destructible: false,
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 2),
                        size: new Vector2<int>(x: 3, y: 2))),

                new Structure(
                    id: StructureId.Hut,
                    displayName: nameof(StructureId.Hut).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
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
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 3, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
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
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 35, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.Building,
                        AbilityId.Shack.Accommodation
                    },
                    size: new Vector2<int>(x: 1, y: 1)),

                new Structure(
                    id: StructureId.Smith,
                    displayName: nameof(StructureId.Smith).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 75, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.Building,
                        AbilityId.Smith.MeleeWeaponProduction,
                    },
                    size: new Vector2<int>(x: 1, y: 2)),

                new Structure(
                    id: StructureId.Fletcher,
                    displayName: nameof(StructureId.Fletcher).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 80, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.Building,
                        AbilityId.Fletcher.RangedWeaponProduction,
                    },
                    size: new Vector2<int>(x: 2, y: 2)),

                new Structure(
                    id: StructureId.Alchemy,
                    displayName: nameof(StructureId.Alchemy).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 90, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.Building,
                        AbilityId.Alchemy.SpecialWeaponProduction,
                    },
                    size: new Vector2<int>(x: 2, y: 3)),

                new Structure(
                    id: StructureId.Depot,
                    displayName: nameof(StructureId.Depot).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.Building,
                        AbilityId.Depot.WeaponStorage,
                    },
                    size: new Vector2<int>(x: 2, y: 2)),

                new Structure(
                    id: StructureId.Workshop,
                    displayName: nameof(StructureId.Workshop).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 55, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.Building,
                        AbilityId.Workshop.Research
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    id: StructureId.Outpost,
                    displayName: nameof(StructureId.Outpost).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 90, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.Building,
                        AbilityId.Outpost.Ascendable,
                        AbilityId.Outpost.HighGround
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    id: StructureId.Barricade,
                    displayName: nameof(StructureId.Barricade).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Revelators.Building,
                        AbilityId.Barricade.ProtectiveShield,
                        AbilityId.Barricade.Caltrops,
                        AbilityId.Barricade.Decompose
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    id: StructureId.BatteryCore,
                    displayName: nameof(StructureId.BatteryCore).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
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
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 25, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
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
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
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
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 40, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 30, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
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
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 70, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 15, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
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
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 6, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
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
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 35, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Temple.KeepingTheFaith
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    id: StructureId.MilitaryBase,
                    displayName: nameof(StructureId.MilitaryBase).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 80, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 15, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.MilitaryBase.Train
                    },
                    size: new Vector2<int>(x: 1, y: 3),
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 2), 
                        size: new Vector2<int>(x: 1, y: 1))),
                
                new Structure(
                    id: StructureId.Factory,
                    displayName: nameof(StructureId.Factory).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 95, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 20, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Factory.Train
                    },
                    size: new Vector2<int>(x: 2, y: 5),
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 2), 
                        size: new Vector2<int>(x: 2, y: 2))),
                
                new Structure(
                    id: StructureId.Laboratory,
                    displayName: nameof(StructureId.Laboratory).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 110, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 30, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Laboratory.Train
                    },
                    size: new Vector2<int>(x: 3, y: 3),
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 1), 
                        size: new Vector2<int>(x: 1, y: 1))),
                
                new Structure(
                    id: StructureId.Armoury,
                    displayName: nameof(StructureId.Armoury).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 25, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Armoury.Research
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    id: StructureId.Wall,
                    displayName: nameof(StructureId.Wall).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
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
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Stairs.Ascendable
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    id: StructureId.Gate,
                    displayName: nameof(StructureId.Gate).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 300, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
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
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 95, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Watchtower.VantagePoint
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    id: StructureId.Bastion,
                    displayName: nameof(StructureId.Bastion).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 550, hasCurrent: true, combatType: StatType.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: StatType.Shields),
                        new CombatStat(maxAmount: 3, hasCurrent: false, combatType: StatType.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: StatType.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: StatType.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shared.Uee.Building,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Bastion.Battlement
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
            };
        }
    }
}