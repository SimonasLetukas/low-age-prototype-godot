using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Shared;

namespace low_age_data.Collections
{
    public static class Units
    {
        public static List<Unit> Get()
        {
            return new List<Unit>
            {
                new Unit(
                    id: UnitId.Slave,
                    displayName: nameof(UnitId.Slave),
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
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Slave.Build,
                        AbilityId.Slave.Repair,
                        AbilityId.Slave.ManualLabour
                    }),

                new Unit(
                    id: UnitId.Leader,
                    displayName: nameof(UnitId.Leader),
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
                            bonusTo: CombatAttribute.Biological, 
                            bonusAmount: 7)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Celestial
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Leader.AllForOne,
                        AbilityId.Leader.MenacingPresence,
                        AbilityId.Leader.OneForAll
                    }),

                new Unit(
                    id: UnitId.Quickdraw,
                    displayName: nameof(UnitId.Quickdraw),
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
                            bonusTo: CombatAttribute.Light,
                            bonusAmount: 2)
                    },
                    originalFaction: FactionId.Revelators, 
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Biological,
                        CombatAttribute.Ranged
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Quickdraw.Doubleshot,
                        AbilityId.Quickdraw.Cripple
                    }),

                new Unit(
                    id: UnitId.Gorger,
                    displayName: nameof(UnitId.Gorger),
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
                            bonusTo: CombatAttribute.Biological,
                            bonusAmount: 3)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Gorger.FanaticSuicide,
                        AbilityId.Gorger.FanaticSuicidePassive
                    }),

                new Unit(
                    id: UnitId.Camou,
                    displayName: nameof(UnitId.Camou),
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
                            bonusTo: CombatAttribute.Armoured,
                            bonusAmount: 4)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Camou.SilentAssassin,
                        AbilityId.Camou.Climb
                    }),

                new Unit(
                    id: UnitId.Shaman, 
                    displayName: nameof(UnitId.Shaman),
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
                            bonusTo: CombatAttribute.Ranged,
                            bonusAmount: 9)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Biological,
                        CombatAttribute.Ranged
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Shaman.WondrousGoo
                    }),

                new Unit(
                    id: UnitId.Pyre,
                    displayName: nameof(UnitId.Pyre),
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
                            bonusTo: CombatAttribute.Armoured,
                            bonusAmount: 8)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Mechanical,
                        CombatAttribute.Ranged
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Pyre.WallOfFlames,
                        AbilityId.Pyre.PhantomMenace
                    }),

                new Unit(
                    id: UnitId.BigBadBull,
                    displayName: nameof(UnitId.BigBadBull).CamelCaseToWords(),
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
                            bonusTo: CombatAttribute.Giant,
                            bonusAmount: 6)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Giant,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.BigBadBull.UnleashTheRage
                    },
                    size: 2),

                new Unit(
                    id: UnitId.Mummy,
                    displayName: nameof(UnitId.Mummy),
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
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Mummy.SpawnRoach,
                        AbilityId.Mummy.LeapOfHunger
                    }),

                new Unit(
                    id: UnitId.Roach,
                    displayName: nameof(UnitId.Roach),
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
                            bonusTo: CombatAttribute.Mechanical,
                            bonusAmount: 4)
                    },
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Roach.DegradingCarapace,
                        AbilityId.Roach.CorrosiveSpit
                    }),

                new Unit(
                    id: UnitId.Parasite,
                    displayName: nameof(UnitId.Parasite),
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
                    originalFaction: FactionId.Revelators,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Celestial
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Parasite.ParalysingGrasp
                    }),

                new Unit(
                    id: UnitId.Horrior, 
                    displayName: nameof(UnitId.Horrior),
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
                            bonusTo: CombatAttribute.Armoured,
                            bonusAmount: 4)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Horrior.ExpertFormation,
                        AbilityId.Horrior.Mount
                    }),

                new Unit(
                    id: UnitId.Marksman,
                    displayName: nameof(UnitId.Marksman),
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
                            bonusTo: CombatAttribute.Ranged,
                            bonusAmount: 3)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Biological,
                        CombatAttribute.Ranged
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Marksman.CriticalMark
                    }),

                new Unit(
                    id: UnitId.Surfer,
                    displayName: nameof(UnitId.Surfer),
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
                            bonusTo: CombatAttribute.Light,
                            bonusAmount: 4)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Surfer.Dismount
                    }),

                new Unit(
                    id: UnitId.Mortar, 
                    displayName: nameof(UnitId.Mortar),
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
                            bonusTo: CombatAttribute.Armoured,
                            bonusAmount: 9)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Biological,
                        CombatAttribute.Ranged
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Mortar.DeadlyAmmunition,
                        AbilityId.Mortar.Reload,
                        AbilityId.Mortar.PiercingBlast
                    }),

                new Unit(
                    id: UnitId.Hawk,
                    displayName: nameof(UnitId.Hawk),
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
                            bonusTo: CombatAttribute.Celestial,
                            bonusAmount: 3)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Hawk.TacticalGoggles,
                        AbilityId.Hawk.Leadership,
                        AbilityId.Hawk.HealthKit
                    }),

                new Unit(
                    id: UnitId.Engineer,
                    displayName: nameof(UnitId.Engineer),
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
                            bonusTo: CombatAttribute.Mechanical,
                            bonusAmount: 5)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Biological
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Engineer.AssembleMachine,
                        AbilityId.Engineer.Operate,
                        AbilityId.Engineer.Repair
                    }),                
                
                new Unit(
                    id: UnitId.Cannon,
                    displayName: nameof(UnitId.Cannon),
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
                            bonusTo: CombatAttribute.Structure,
                            bonusAmount: 20)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Armoured,
                        CombatAttribute.Mechanical,
                        CombatAttribute.Ranged
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Cannon.Assembling,
                        AbilityId.Cannon.Machine,
                        AbilityId.Cannon.HeatUp
                    }),                
                
                new Unit(
                    id: UnitId.Ballista,
                    displayName: nameof(UnitId.Ballista),
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
                            bonusTo: CombatAttribute.Giant,
                            bonusAmount: 6)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Mechanical,
                        CombatAttribute.Ranged
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Ballista.Assembling,
                        AbilityId.Ballista.Machine,
                        AbilityId.Ballista.AddOn,
                        AbilityId.Ballista.Aim
                    }),                
                
                new Unit(
                    id: UnitId.Radar,
                    displayName: nameof(UnitId.Radar),
                    description: "",
                    statistics: new List<Stat>
                    {
                        new CombatStat(maxAmount: 20, hasCurrent: true, combatType: Stats.Health),
                        new CombatStat(maxAmount: 12, hasCurrent: true, combatType: Stats.Shields),
                        new CombatStat(maxAmount: 2, hasCurrent: false, combatType: Stats.MeleeArmour),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.RangedArmour),
                        new CombatStat(maxAmount: 0, hasCurrent: true, combatType: Stats.Movement),
                        new CombatStat(maxAmount: 8, hasCurrent: false, combatType: Stats.Initiative),
                        new CombatStat(maxAmount: 5, hasCurrent: false, combatType: Stats.Vision)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Light,
                        CombatAttribute.Mechanical
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Radar.Assembling,
                        AbilityId.Radar.Machine,
                        AbilityId.Shared.Uee.PowerDependency,
                        AbilityId.Radar.ResonatingSweep,
                        AbilityId.Radar.RadioLocation
                    }),                
                
                new Unit(
                    id: UnitId.Vessel,
                    displayName: nameof(UnitId.Vessel),
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
                            bonusTo: CombatAttribute.Mechanical,
                            bonusAmount: 8)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Giant,
                        CombatAttribute.Mechanical
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Vessel.Machine,
                        AbilityId.Vessel.AbsorbentField,
                        AbilityId.Vessel.Fortify
                    },
                    size: 2),                
                
                new Unit(
                    id: UnitId.Omen,
                    displayName: nameof(UnitId.Omen),
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
                            bonusTo: CombatAttribute.Biological,
                            bonusAmount: 10),
                        new AttackStat(
                            maxAmount: 5,
                            attackType: Attacks.Ranged,
                            minimumDistance: 2,
                            maximumDistance: 4,
                            bonusTo: CombatAttribute.Biological,
                            bonusAmount: 5)
                    },
                    originalFaction: FactionId.Uee,
                    combatAttributes: new List<CombatAttribute>
                    {
                        CombatAttribute.Celestial,
                        CombatAttribute.Ranged
                    },
                    abilities: new List<AbilityId>
                    {
                        AbilityId.Omen.Rendition
                    })
            };
        }
    }
}
