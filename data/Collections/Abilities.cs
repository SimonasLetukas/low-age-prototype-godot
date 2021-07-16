using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Collections
{
    public static class Abilities
    {
        public static List<Ability> Get()
        {
            return new List<Ability>
            {
                new Passive(
                    AbilityName.Leader.AllForOne,
                    nameof(AbilityName.Leader.AllForOne).CamelCaseToWords(),
                    "Revelators faction loses if Leader dies.",
                    true,
                    null,
                    null,
                    null,
                    null,
                    EffectName.Leader.AllForOneApplyBehaviour),

                new Passive(
                    AbilityName.Leader.MenacingPresence,
                    nameof(AbilityName.Leader.MenacingPresence).CamelCaseToWords(),
                    "All friendly and enemy units that enter 6 Attack Distance around Leader " +
                    "have their Melee Damage and Ranged Damage reduced by 2 (total minimum of 1).",
                    true,
                    EffectName.Leader.MenacingPresenceSearch),

                new Target(
                    AbilityName.Leader.OneForAll,
                    TurnPhase.Planning,
                    nameof(AbilityName.Leader.OneForAll).CamelCaseToWords(),
                    "Select an adjacent Obelisk and sap its energy to give all friendly units " +
                    "+2 Health. This Obelisk cannot be sapped again for 10 turns.",
                    1,
                    EffectName.Leader.OneForAllApplyBehaviourObelisk),

                new Build(
                    AbilityName.Slave.Build,
                    nameof(AbilityName.Slave.Build).CamelCaseToWords(),
                    "Start building a Revelators' structure on an adjacent tile. Multiple Slaves " +
                    "can build the structure, each additional one after the first provides half of the " +
                    "Celestium production to the construction than the previous Slave.",
                    1,
                    new List<StructureName>
                    {
                        StructureName.Hut,
                        StructureName.Obelisk,
                        StructureName.Shack,
                        StructureName.Smith,
                        StructureName.Fletcher,
                        StructureName.Alchemy,
                        StructureName.Depot,
                        StructureName.Workshop,
                        StructureName.Outpost,
                        StructureName.Barricade
                    },
                    true,
                    true),

                new Target(
                    AbilityName.Slave.Repair,
                    TurnPhase.Planning,
                    nameof(AbilityName.Slave.Repair).CamelCaseToWords(),
                    "Select an adjacent structure. At the start of the next planning phase the " +
                    "selected structure receives +1 Health. Multiple Slaves can stack their repairs.",
                    1,
                    EffectName.Slave.RepairApplyBehaviourStructure),

                new Target(
                    AbilityName.Slave.ManualLabour,
                    TurnPhase.Planning,
                    nameof(AbilityName.Slave.ManualLabour).CamelCaseToWords(),
                    "Select an adjacent Hut. At the start of the next planning phase receive +2 " +
                    "Scraps. Maximum of one Slave per Hut.",
                    1,
                    EffectName.Slave.ManualLabourApplyBehaviourHut),

                new Passive(
                    AbilityName.Quickdraw.Doubleshot,
                    nameof(AbilityName.Quickdraw.Doubleshot).CamelCaseToWords(),
                    "Ranged attacks twice.",
                    true,
                    null,
                    null,
                    null,
                    null,
                    EffectName.Quickdraw.DoubleshotApplyBehaviour),

                new Passive(
                    AbilityName.Quickdraw.Cripple,
                    nameof(AbilityName.Quickdraw.Cripple).CamelCaseToWords(),
                    "Each ranged attack cripples the target until the end of their action. During " +
                    "this time target has 60% of their maximum Movement (rounded up) and cannot receive healing " +
                    "from any sources. Multiple attacks on a crippled target have no additional effects.",
                    true,
                    null,
                    new List<Research>
                    {
                        Research.Revelators.PoisonedSlits
                    },
                    new List<EffectName>
                    {
                        EffectName.Quickdraw.CrippleApplyBehaviour
                    },
                    new List<Attacks>
                    {
                        Attacks.Ranged
                    }),

                new Passive(
                    AbilityName.Gorger.FanaticSuicidePassive,
                    nameof(AbilityName.Gorger.FanaticSuicidePassive).CamelCaseToWords(),
                    "",
                    false,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideDestroy
                    },
                    new List<Attacks>
                    {
                        Attacks.Melee
                    },
                    EffectName.Gorger.FanaticSuicideApplyBehaviourBuff),

                new Instant(
                    AbilityName.Gorger.FanaticSuicide,
                    TurnPhase.Action, 
                    nameof(AbilityName.Gorger.FanaticSuicide).CamelCaseToWords(),
                    "Either as an action, or instead of attacking, or upon getting killed Gorger " +
                    "detonates, dealing its Melee Damage to all friendly and enemy units in 1 Attack Distance, " +
                    "killing itself in the process.",
                    new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideSearch,
                        EffectName.Gorger.FanaticSuicideDestroy
                    }),

                new Passive(
                    AbilityName.Camou.SilentAssassin,
                    nameof(AbilityName.Camou.SilentAssassin).CamelCaseToWords(),
                    "Deals 50% of target's lost Health as bonus Melee Damage if there are no friendly " +
                    "units around Camou in 4 Attack Distance. Additionally, if the target has none of its allies " +
                    "in the same radius, Camou silences the target for 2 of its actions, disabling the use of any " +
                    "abilities or passives.",
                    true,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Camou.SilentAssassinOnHitDamage,
                        EffectName.Camou.SilentAssassinOnHitSilence
                    },
                    new List<Attacks>
                    {
                        Attacks.Melee
                    }),

                new Target(
                    AbilityName.Camou.Climb,
                    TurnPhase.Action, 
                    nameof(AbilityName.Camou.Climb).CamelCaseToWords(),
                    "Select an adjacent unoccupied space on a high ground. This space is considered " +
                    "occupied until the end of the action phase at which point Camou moves to it. Passively, " +
                    "Camou can move down from high ground at the additional cost of 1 Movement.",
                    1,
                    EffectName.Camou.ClimbTeleport,
                    new List<Research>
                    {
                        Research.Revelators.SpikedRope
                    }),

                new Passive(
                    AbilityName.Camou.ClimbPassive,
                    nameof(AbilityName.Camou.ClimbPassive).CamelCaseToWords(),
                    "",
                    false,
                    null,
                    new List<Research>
                    {
                        Research.Revelators.SpikedRope
                    },
                    null,
                    null,
                    EffectName.Camou.ClimbApplyBehaviour),

                new Target(
                    AbilityName.Shaman.WondrousGoo,
                    TurnPhase.Action,
                    nameof(AbilityName.Shaman.WondrousGoo).CamelCaseToWords(),
                    "Select a tile in 4 Attack Distance, which gets contaminated. Any unit in the " +
                    "contamination has its vision and Attack Distance reduced by 3 (total minimum of 1) and " +
                    "receives 1 Pure Damage at the start of its turn. At the end of this action phase, the " +
                    "contamination area expands to adjacent tiles and stays until the end of the next action phase.",
                    4,
                    EffectName.Shaman.WondrousGooCreateEntity,
                    null,
                    EndsAt.EndOf.Second.ActionPhase),

                new Passive(
                    AbilityName.Pyre.WallOfFlames,
                    nameof(AbilityName.Pyre.WallOfFlames).CamelCaseToWords(),
                    "The cargo leaves a path of flames when moved, which stay until the start of the " +
                    "next Pyre's action or until death. Any unit which starts its turn or moves onto the flames " +
                    "receives 5 Melee Damage.",
                    true,
                    null,
                    null,
                    null,
                    null,
                    EffectName.Pyre.CargoCreateEntity),

                new Passive(
                    AbilityName.Pyre.PhantomMenace,
                    nameof(AbilityName.Pyre.PhantomMenace).CamelCaseToWords(),
                    "Can move through enemy units (but not buildings).",
                    true,
                    null,
                    new List<Research>
                    {
                        Research.Revelators.QuestionableCargo
                    },
                    null,
                    null,
                    EffectName.Pyre.PhantomMenaceApplyBehaviour),

                new Target(
                    AbilityName.BigBadBull.UnleashTheRage,
                    TurnPhase.Action,
                    nameof(AbilityName.BigBadBull.UnleashTheRage).CamelCaseToWords(),
                    "Select a direction (1 out of 4) originating from Big Bad Bull. Any two adjacent " +
                    "units towards the selected direction suffer Bull's Melee Damage and are pushed one tile farther. " +
                    "If the destination tile is occupied or impassable, the target receives additional 5 Melee Damage.",
                    1,
                    EffectName.BigBadBull.UnleashTheRageSearch,
                    null,
                    EndsAt.EndOf.Next.ActionPhase),

                new Target(
                    AbilityName.Mummy.SpawnRoach,
                    TurnPhase.Action,
                    nameof(AbilityName.Mummy.SpawnRoach).CamelCaseToWords(),
                    "Select an adjacent tile in which Roach is created.",
                    1,
                    EffectName.Mummy.SpawnRoachCreateEntity,
                    null,
                    EndsAt.EndOf.Next.ActionPhase),

                new Passive(
                    AbilityName.Mummy.LeapOfHunger,
                    nameof(AbilityName.Mummy.LeapOfHunger).CamelCaseToWords(),
                    "Roach creation range is increased to 4 Distance.",
                    true,
                    null,
                    new List<Research>
                    {
                        Research.Revelators.HumanfleshRations
                    },
                    null,
                    null,
                    EffectName.Mummy.LeapOfHungerModifyAbility),

                new Target(
                    AbilityName.Mummy.SpawnRoachModified,
                    TurnPhase.Action,
                    nameof(AbilityName.Mummy.SpawnRoach).CamelCaseToWords(),
                    "Select a tile in 4 Distance in which Roach is created.",
                    4,
                    EffectName.Mummy.SpawnRoachCreateEntity,
                    null,
                    EndsAt.EndOf.Next.ActionPhase),

                new Passive(
                    AbilityName.Roach.DegradingCarapace,
                    nameof(AbilityName.Roach.DegradingCarapace).CamelCaseToWords(),
                    "At the start of each action loses 1 Health more than the previous action.",
                    true,
                    null,
                    null,
                    null,
                    null,
                    EffectName.Roach.DegradingCarapaceApplyBehaviour),

                new Target(
                    AbilityName.Roach.CorrosiveSpit,
                    TurnPhase.Action, 
                    nameof(AbilityName.Roach.CorrosiveSpit).CamelCaseToWords(),
                    "Perform a ranged attack in 4 Distance dealing 6 (+8 to mechanical) Range Damage.",
                    4,
                    EffectName.Roach.CorrosiveSpitDamage,
                    new List<Research>
                    {
                        Research.Revelators.AdaptiveDigestion
                    },
                    EndsAt.EndOf.Second.ActionPhase),

                new Passive(
                    AbilityName.Parasite.ParalysingGrasp,
                    nameof(AbilityName.Parasite.ParalysingGrasp).CamelCaseToWords(),
                    "Instead of attacking, Parasite attaches to the target. Both units occupy the same space and " +
                    "are considered enemy to all players. Parasite can only detach when the target is killed. All units who " +
                    "attack this combined unit do damage to both. On its turn, Parasite can move the target, using target's " +
                    "Movement. On target's turn, it must execute attack action to any friendly or enemy unit in range, " +
                    "otherwise skip turn.",
                    true,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Parasite.ParalysingGraspApplyTetherBehaviour,
                        EffectName.Parasite.ParalysingGraspApplyAttackBehaviour,
                        //EffectName.Parasite.ParalysingGraspApplySelfBehaviour,
                    },
                    new List<Attacks>
                    {
                        Attacks.Melee
                    }),

                new Passive(
                    AbilityName.Horrior.ExpertFormation,
                    nameof(AbilityName.Horrior.ExpertFormation).CamelCaseToWords(),
                    "Gains +2 Range Armour if at least one other Horrior is adjacent.",
                    true,
                    EffectName.Horrior.ExpertFormationSearch),

                new Instant(
                    AbilityName.Horrior.Mount,
                    TurnPhase.Planning,
                    nameof(AbilityName.Horrior.Mount).CamelCaseToWords(),
                    "Spend 3 turns mounting (unable to act) and at the start of the fourth planning phase " +
                    "transform into Surfer.",
                    new List<EffectName>
                    {
                        EffectName.Horrior.MountApplyBehaviour
                    },
                    new List<Research>
                    {
                        Research.Uee.HoverboardReignition
                    }),

                new Passive(
                    AbilityName.Marksman.CriticalMark,
                    nameof(AbilityName.Marksman.CriticalMark).CamelCaseToWords(),
                    "Each ranged attack marks the target unit. If a friendly non-Marksman unit attacks the marked " +
                    "target, the mark is consumed and the target receives 5 Melee Damage. The mark lasts until the end " +
                    "of the next action phase.",
                    true,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Marksman.CriticalMarkApplyBehaviour
                    },
                    new List<Attacks>
                    {
                        Attacks.Ranged
                    }),
                
                new Passive(
                    AbilityName.Surfer.Dismount,
                    nameof(AbilityName.Surfer.Dismount).CamelCaseToWords(),
                    "Upon death, reemerges as Horrior.",
                    true,
                    null,
                    null,
                    null,
                    null,
                    EffectName.Surfer.DismountApplyBehaviour)
            };
        }
    }
}
