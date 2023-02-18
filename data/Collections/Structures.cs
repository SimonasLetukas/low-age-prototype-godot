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
                    name: StructureName.Citadel,
                    displayName: nameof(StructureName.Citadel).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>(),
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.PassiveIncome,
                        AbilityName.Citadel.ExecutiveStash,
                        AbilityName.Citadel.Ascendable,
                        AbilityName.Citadel.HighGround,
                        AbilityName.Citadel.PromoteGoons
                    },
                    size: new Vector2<int>(x: 3, y: 4),
                    centerPoint: new Vector2<int>(x: 1, y: 1),
                    destructible: false,
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 2),
                        size: new Vector2<int>(x: 3, y: 2))),

                new Structure(
                    name: StructureName.Hut,
                    displayName: nameof(StructureName.Hut).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Hut.Building,
                        AbilityName.Shared.ScrapsIncome
                    },
                    size: new Vector2<int>(x: 2, y: 2)),

                new Structure(
                    name: StructureName.Obelisk,
                    displayName: nameof(StructureName.Obelisk).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 3, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Obelisk.Building,
                        AbilityName.Shared.CelestiumIncome,
                        AbilityName.Obelisk.CelestiumDischarge
                    },
                    size: new Vector2<int>(x: 2, y: 2)),

                new Structure(
                    name: StructureName.Shack,
                    displayName: nameof(StructureName.Shack).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 35, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Revelators.Building,
                        AbilityName.Shack.Accommodation
                    },
                    size: new Vector2<int>(x: 1, y: 1)),

                new Structure(
                    name: StructureName.Smith,
                    displayName: nameof(StructureName.Smith).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 75, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Revelators.Building,
                        AbilityName.Smith.MeleeWeaponProduction,
                    },
                    size: new Vector2<int>(x: 1, y: 2)),

                new Structure(
                    name: StructureName.Fletcher,
                    displayName: nameof(StructureName.Fletcher).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 80, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Revelators.Building,
                        AbilityName.Fletcher.RangedWeaponProduction,
                    },
                    size: new Vector2<int>(x: 2, y: 2)),

                new Structure(
                    name: StructureName.Alchemy,
                    displayName: nameof(StructureName.Alchemy).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 90, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Revelators.Building,
                        AbilityName.Alchemy.SpecialWeaponProduction,
                    },
                    size: new Vector2<int>(x: 2, y: 3)),

                new Structure(
                    name: StructureName.Depot,
                    displayName: nameof(StructureName.Depot).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Revelators.Building,
                        AbilityName.Depot.WeaponStorage,
                    },
                    size: new Vector2<int>(x: 2, y: 2)),

                new Structure(
                    name: StructureName.Workshop,
                    displayName: nameof(StructureName.Workshop).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 55, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Revelators.Building,
                        AbilityName.Workshop.Research
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    name: StructureName.Outpost,
                    displayName: nameof(StructureName.Outpost).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 90, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Revelators.Building,
                        AbilityName.Outpost.Ascendable,
                        AbilityName.Outpost.HighGround
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    name: StructureName.Barricade,
                    displayName: nameof(StructureName.Barricade).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Revelators.Building,
                        AbilityName.Barricade.ProtectiveShield,
                        AbilityName.Barricade.Caltrops,
                        AbilityName.Barricade.Decompose
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    name: StructureName.BatteryCore,
                    displayName: nameof(StructureName.BatteryCore).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.PowerGenerator,
                        AbilityName.Shared.PassiveIncome,
                        AbilityName.BatteryCore.PowerGrid,
                        AbilityName.Shared.Uee.Build,
                        AbilityName.BatteryCore.FusionCoreUpgrade
                    },
                    size: new Vector2<int>(x: 3, y: 3)),
                    
                new Structure(
                    name: StructureName.FusionCore,
                    displayName: nameof(StructureName.FusionCore).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 25, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.PowerGenerator,
                        AbilityName.Shared.PassiveIncome,
                        AbilityName.FusionCore.PowerGrid,
                        AbilityName.Shared.Uee.Build,
                        AbilityName.FusionCore.DefenceProtocol,
                        AbilityName.FusionCore.CelestiumCoreUpgrade
                    },
                    size: new Vector2<int>(x: 3, y: 3)),
                
                new Structure(
                    name: StructureName.CelestiumCore,
                    displayName: nameof(StructureName.CelestiumCore).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.PowerGenerator,
                        AbilityName.Shared.PassiveIncome,
                        AbilityName.CelestiumCore.PowerGrid,
                        AbilityName.Shared.Uee.Build,
                        AbilityName.CelestiumCore.DefenceProtocol,
                        AbilityName.CelestiumCore.HeightenedConductivity
                    },
                    size: new Vector2<int>(x: 3, y: 3)),
                
                new Structure(
                    name: StructureName.Collector,
                    displayName: nameof(StructureName.Collector).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 40, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 30, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Collector.Building,
                        AbilityName.Shared.ScrapsIncome,
                        AbilityName.Collector.DirectTransitSystem
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    name: StructureName.Extractor,
                    displayName: nameof(StructureName.Extractor).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 70, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 15, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Extractor.Building,
                        AbilityName.Shared.CelestiumIncome,
                        AbilityName.Extractor.ReinforcedInfrastructure
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new Structure(
                    name: StructureName.PowerPole,
                    displayName: nameof(StructureName.PowerPole).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 6, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.PowerPole.PowerGrid,
                        AbilityName.PowerPole.ExcessDistribution,
                        AbilityName.PowerPole.ImprovedPowerGrid
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                    
                new Structure(
                    name: StructureName.Temple,
                    displayName: nameof(StructureName.Temple).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 35, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Temple.KeepingTheFaith
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    name: StructureName.MilitaryBase,
                    displayName: nameof(StructureName.MilitaryBase).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 80, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 15, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.MilitaryBase.Train
                    },
                    size: new Vector2<int>(x: 1, y: 3),
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 2), 
                        size: new Vector2<int>(x: 1, y: 1))),
                
                new Structure(
                    name: StructureName.Factory,
                    displayName: nameof(StructureName.Factory).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 95, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 20, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Factory.Train
                    },
                    size: new Vector2<int>(x: 2, y: 5),
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 2), 
                        size: new Vector2<int>(x: 2, y: 2))),
                
                new Structure(
                    name: StructureName.Laboratory,
                    displayName: nameof(StructureName.Laboratory).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 110, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 30, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Laboratory.Train
                    },
                    size: new Vector2<int>(x: 3, y: 3),
                    walkableArea: new Area(
                        start: new Vector2<int>(x: 0, y: 1), 
                        size: new Vector2<int>(x: 1, y: 1))),
                
                new Structure(
                    name: StructureName.Armoury,
                    displayName: nameof(StructureName.Armoury).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 65, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 25, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Armoury.Research
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new Structure(
                    name: StructureName.Wall,
                    displayName: nameof(StructureName.Wall).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 100, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Wall.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Wall.HighGround
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    name: StructureName.Stairs,
                    displayName: nameof(StructureName.Stairs).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 60, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Stairs.Ascendable
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    name: StructureName.Gate,
                    displayName: nameof(StructureName.Gate).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 300, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Gate.HighGround,
                        AbilityName.Gate.Ascendable,
                        AbilityName.Gate.Entrance
                    },
                    size: new Vector2<int>(x: 1, y: 4)),
                
                new Structure(
                    name: StructureName.Watchtower,
                    displayName: nameof(StructureName.Watchtower).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 95, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Watchtower.VantagePoint
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new Structure(
                    name: StructureName.Bastion,
                    displayName: nameof(StructureName.Bastion).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 550, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 3, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shared.Uee.Building,
                        AbilityName.Shared.Uee.PowerDependency,
                        AbilityName.Bastion.Battlement
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
            };
        }
    }
}