using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Shared;

namespace low_age_data.Collections
{
    public static class Units
    {
        public static List<Unit> Get()
        {
            return new List<Unit>
            {
                new(
                    name: UnitName.Slave,
                    displayName: nameof(UnitName.Slave),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 6, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 20, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 1, 
                            attackType: Attacks.Melee, 
                            minimumDistance: 1, 
                            maximumDistance: 1)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Slave.Build,
                        AbilityName.Slave.Repair,
                        AbilityName.Slave.ManualLabour
                    }),

                new(
                    name: UnitName.Leader,
                    displayName: nameof(UnitName.Leader),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 3, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 28, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 5, 
                            attackType: Attacks.Melee, 
                            minimumDistance: 1, 
                            maximumDistance: 2, 
                            bonusTo: CombatAttributes.Biological, 
                            bonusAmount: 7)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Celestial
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Leader.AllForOne,
                        AbilityName.Leader.MenacingPresence,
                        AbilityName.Leader.OneForAll
                    }),

                new(
                    name: UnitName.Quickdraw,
                    displayName: nameof(UnitName.Quickdraw),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 13, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 18, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 1,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1),
                        new AttackStat(
                            maxAmount: 3,
                            attackType: Attacks.Ranged, 
                            minimumDistance: 2,
                            maximumDistance: 6,
                            bonusTo: CombatAttributes.Light,
                            bonusAmount: 2)
                    },
                    originalFaction: Factions.Revelators, 
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological,
                        CombatAttributes.Ranged
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Quickdraw.Doubleshot,
                        AbilityName.Quickdraw.Cripple
                    }),

                new(
                    name: UnitName.Gorger,
                    displayName: nameof(UnitName.Gorger),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 7, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 26, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 3,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Biological,
                            bonusAmount: 3)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Gorger.FanaticSuicide,
                        AbilityName.Gorger.FanaticSuicidePassive
                    }),

                new(
                    name: UnitName.Camou,
                    displayName: nameof(UnitName.Camou),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 19, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 23, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 6,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Armoured,
                            bonusAmount: 4)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Camou.SilentAssassin,
                        AbilityName.Camou.Climb
                    }),

                new(
                    name: UnitName.Shaman, 
                    displayName: nameof(UnitName.Shaman),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 18, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 19, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 1,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1),
                        new AttackStat(
                            maxAmount: 2,
                            attackType: Attacks.Ranged,
                            minimumDistance: 2,
                            maximumDistance: 4,
                            bonusTo: CombatAttributes.Ranged,
                            bonusAmount: 9)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Biological,
                        CombatAttributes.Ranged
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Shaman.WondrousGoo
                    }),

                new(
                    name: UnitName.Pyre,
                    displayName: nameof(UnitName.Pyre),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 22, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 12, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 1,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1),
                        new AttackStat(
                            maxAmount: 6,
                            attackType: Attacks.Ranged,
                            minimumDistance: 2,
                            maximumDistance: 5,
                            bonusTo: CombatAttributes.Armoured,
                            bonusAmount: 8)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Mechanical,
                        CombatAttributes.Ranged
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Pyre.WallOfFlames,
                        AbilityName.Pyre.PhantomMenace
                    }),

                new(
                    name: UnitName.BigBadBull,
                    displayName: nameof(UnitName.BigBadBull).CamelCaseToWords(),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 40, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 25, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 6, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 8,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Giant,
                            bonusAmount: 6)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Giant,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.BigBadBull.UnleashTheRage
                    },
                    size: 2),

                new(
                    name: UnitName.Mummy,
                    displayName: nameof(UnitName.Mummy),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 39, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 2, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 16, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 3,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Mummy.SpawnRoach,
                        AbilityName.Mummy.LeapOfHunger
                    }),

                new(
                    name: UnitName.Roach,
                    displayName: nameof(UnitName.Roach),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 15, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 6,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Mechanical,
                            bonusAmount: 4)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Roach.DegradingCarapace,
                        AbilityName.Roach.CorrosiveSpit
                    }),

                new(
                    name: UnitName.Parasite,
                    displayName: nameof(UnitName.Parasite),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 36, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 6, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 14, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 0,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1)
                    },
                    originalFaction: Factions.Revelators,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Celestial
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Parasite.ParalysingGrasp
                    }),

                new(
                    name: UnitName.Horrior, 
                    displayName: nameof(UnitName.Horrior),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 25, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 12, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 3, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 2, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 32, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 9,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Armoured,
                            bonusAmount: 4)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Horrior.ExpertFormation,
                        AbilityName.Horrior.Mount
                    }),

                new(
                    name: UnitName.Marksman,
                    displayName: nameof(UnitName.Marksman),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 7, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 17, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 1,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1),
                        new AttackStat(
                            maxAmount: 4,
                            attackType: Attacks.Ranged, 
                            minimumDistance: 2,
                            maximumDistance: 7,
                            bonusTo: CombatAttributes.Ranged,
                            bonusAmount: 3)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological,
                        CombatAttributes.Ranged
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Marksman.CriticalMark
                    }),

                new(
                    name: UnitName.Surfer,
                    displayName: nameof(UnitName.Surfer),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 2, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 2, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 30, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 5,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Light,
                            bonusAmount: 4)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Surfer.Dismount
                    }),

                new(
                    name: UnitName.Mortar, 
                    displayName: nameof(UnitName.Mortar),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 21, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 2,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1),
                        new AttackStat(
                            maxAmount: 8,
                            attackType: Attacks.Ranged,
                            minimumDistance: 2,
                            maximumDistance: 5,
                            bonusTo: CombatAttributes.Armoured,
                            bonusAmount: 9)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Biological,
                        CombatAttributes.Ranged
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Mortar.DeadlyAmmunition,
                        AbilityName.Mortar.Reload,
                        AbilityName.Mortar.PiercingBlast
                    }),

                new(
                    name: UnitName.Hawk,
                    displayName: nameof(UnitName.Hawk),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 9, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 45, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 3,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Celestial,
                            bonusAmount: 3)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Hawk.TacticalGoggles,
                        AbilityName.Hawk.Leadership,
                        AbilityName.Hawk.HealthKit
                    }),

                new(
                    name: UnitName.Engineer,
                    displayName: nameof(UnitName.Engineer),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 8, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 4, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 11, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 1,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Mechanical,
                            bonusAmount: 5)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Engineer.AssembleMachine,
                        AbilityName.Engineer.Operate,
                        AbilityName.Engineer.Repair
                    }),                
                
                new(
                    name: UnitName.Cannon,
                    displayName: nameof(UnitName.Cannon),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 30, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 8, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 3, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 8, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 20,
                            attackType: Attacks.Ranged,
                            minimumDistance: 1,
                            maximumDistance: 10,
                            bonusTo: CombatAttributes.Structure,
                            bonusAmount: 20)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Mechanical,
                        CombatAttributes.Ranged
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Cannon.Assembling,
                        AbilityName.Cannon.Machine,
                        AbilityName.Cannon.HeatUp
                    }),                
                
                new(
                    name: UnitName.Ballista,
                    displayName: nameof(UnitName.Ballista),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 14, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 0, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 27, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 6, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 9,
                            attackType: Attacks.Ranged,
                            minimumDistance: 1,
                            maximumDistance: 9,
                            bonusTo: CombatAttributes.Giant,
                            bonusAmount: 6)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Mechanical,
                        CombatAttributes.Ranged
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Ballista.Assembling,
                        AbilityName.Ballista.Machine,
                        AbilityName.Ballista.AddOn,
                        AbilityName.Ballista.Aim
                    }),                
                
                new(
                    name: UnitName.Radar,
                    displayName: nameof(UnitName.Radar),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 12, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 8, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Mechanical
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Radar.Assembling,
                        AbilityName.Radar.Machine,
                        AbilityName.Radar.PowerDependency,
                        AbilityName.Radar.ResonatingSweep,
                        AbilityName.Radar.RadioLocation
                    }),                
                
                new(
                    name: UnitName.Vessel,
                    displayName: nameof(UnitName.Vessel),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 50, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 10, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 3, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 29, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 6, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 2,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Mechanical,
                            bonusAmount: 8)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Giant,
                        CombatAttributes.Mechanical
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Vessel.Machine,
                        AbilityName.Vessel.AbsorbentField,
                        AbilityName.Vessel.Fortify
                    },
                    size: 2),                
                
                new(
                    name: UnitName.Omen,
                    displayName: nameof(UnitName.Omen),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 20, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 1, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 13, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision),
                        new AttackStat(
                            maxAmount: 10,
                            attackType: Attacks.Melee,
                            minimumDistance: 1,
                            maximumDistance: 1,
                            bonusTo: CombatAttributes.Biological,
                            bonusAmount: 10),
                        new AttackStat(
                            maxAmount: 5,
                            attackType: Attacks.Ranged,
                            minimumDistance: 2,
                            maximumDistance: 4,
                            bonusTo: CombatAttributes.Biological,
                            bonusAmount: 5)
                    },
                    originalFaction: Factions.UEE,
                    combatAttributes: new List<CombatAttributes>
                    {
                        CombatAttributes.Celestial,
                        CombatAttributes.Ranged
                    },
                    abilities: new List<AbilityName>
                    {
                        AbilityName.Omen.Rendition
                    })
            };
        }
    }
}
