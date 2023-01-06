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
                new(
                    name: StructureName.Citadel, 
                    displayName: nameof(StructureName.Citadel).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionName.Revelators, 
                    combatAttributes: new List<CombatAttributes>(),
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
                
                new(
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
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Hut.Building, // TODO add to all buildings
                        AbilityName.Shared.ScrapsIncome
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new(
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
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Obelisk.Building,
                        AbilityName.Shared.CelestiumIncome,
                        AbilityName.Obelisk.CelestiumDischarge
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                    
                new(
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
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shack.Building,
                        AbilityName.Shack.Accommodation
                    },
                    size: new Vector2<int>(x: 1, y: 1)),
                
                new(
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
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Smith.Building,
                        AbilityName.Smith.MeleeWeaponProduction,
                    },
                    size: new Vector2<int>(x: 1, y: 2)),
                
                new(
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
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Fletcher.Building,
                        AbilityName.Fletcher.RangedWeaponProduction,
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
                
                new(
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
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Alchemy.Building,
                        AbilityName.Alchemy.SpecialWeaponProduction,
                    },
                    size: new Vector2<int>(x: 2, y: 3)),
                    
                new(
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
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Structure
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Depot.Building,
                        AbilityName.Depot.WeaponStorage,
                    },
                    size: new Vector2<int>(x: 2, y: 2)),
            };
        }
    }
}
