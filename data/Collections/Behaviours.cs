using low_age_data.Common;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;
using low_age_data.Domain.Shared.Flags;
using low_age_data.Domain.Shared.Modifications;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;

namespace low_age_data.Collections
{
    public static class Behaviours
    {
        public static List<Behaviour> Get()
        {
            return new List<Behaviour>
            {
                new Buff(
                    BehaviourName.Leader.AllForOneBuff,
                    nameof(BehaviourName.Leader.AllForOneBuff).CamelCaseToWords(),
                    "Revelators faction loses when this unit dies.",
                    null,
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Leader.AllForOnePlayerLoses
                    }),

                new Buff(
                    BehaviourName.Leader.MenacingPresenceBuff,
                    nameof(BehaviourName.Leader.MenacingPresenceBuff).CamelCaseToWords(),
                    "Melee and Range Damage for this unit is reduced by 2.",
                    null,
                    new List<Modification>
                    {
                        new AttackModification(
                            Change.SubtractMax,
                            2,
                            Attacks.Melee,
                            AttackAttribute.MaxAmount),
                        new AttackModification(
                            Change.SubtractMax,
                            2,
                            Attacks.Ranged,
                            AttackAttribute.MaxAmount)
                    },
                    null,
                    null,
                    null,
                    EndsAt.EndOf.This.Action,
                    false,
                    true,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),

                new Buff(
                    BehaviourName.Leader.OneForAllObeliskBuff,
                    nameof(BehaviourName.Leader.OneForAllObeliskBuff).CamelCaseToWords(),
                    "This unit has recently been sapped for health.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Leader.OneForAllSearch
                    },
                    null,
                    null,
                    EndsAt.StartOf.Tenth.Planning,
                    false,
                    false,
                    Alignment.Negative),

                new Buff(
                    BehaviourName.Leader.OneForAllHealBuff,
                    nameof(BehaviourName.Leader.OneForAllHealBuff).CamelCaseToWords(),
                    "Heals for 2 Health.",
                    null,
                    new List<Modification>
                    {
                        new StatModification(
                            Change.AddCurrent,
                            2,
                            Stats.Health)
                    },
                    null,
                    null,
                    null,
                    EndsAt.Instant,
                    false,
                    false,
                    Alignment.Positive),

                new Buff(
                    BehaviourName.Slave.RepairStructureBuff,
                    nameof(BehaviourName.Slave.RepairStructureBuff).CamelCaseToWords(),
                    "This structure will be repaired by +1 Health at the start of the planning phase.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Slave.RepairApplyBehaviourSelf
                    },
                    new List<Modification>
                    {
                        new StatModification(
                            Change.AddCurrent,
                            1,
                            Stats.Health)
                    },
                    null,
                    EndsAt.StartOf.Next.Planning,
                    true,
                    false,
                    Alignment.Positive,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsInterrupted
                        })
                    },
                    true),

                new Wait(
                    BehaviourName.Slave.RepairWait,
                    nameof(BehaviourName.Slave.RepairWait).CamelCaseToWords(),
                    "Currently repairing a structure.",
                    EndsAt.StartOf.Next.Planning),

                new Buff(
                    BehaviourName.Slave.ManualLabourBuff,
                    nameof(BehaviourName.Slave.ManualLabourBuff).CamelCaseToWords(),
                    "Slave is working on this Hut to provide +2 Scraps at the start of the planning phase.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Slave.ManualLabourApplyBehaviourSelf
                    },
                    null,
                    new List<EffectName>
                    {
                        EffectName.Slave.ManualLabourModifyPlayer
                    },
                    EndsAt.StartOf.Next.Planning,
                    false,
                    false,
                    Alignment.Positive,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    true),

                new Wait(
                    BehaviourName.Slave.ManualLabourWait,
                    nameof(BehaviourName.Slave.ManualLabourWait).CamelCaseToWords(),
                    "Currently working on a nearby Hut.",
                    EndsAt.StartOf.Next.Planning),

                new ExtraAttack(
                    BehaviourName.Quickdraw.DoubleshotExtraAttack,
                    nameof(BehaviourName.Quickdraw.DoubleshotExtraAttack).CamelCaseToWords(),
                    "Ranged attacks twice.",
                    new List<Attacks>
                    {
                        Attacks.Ranged
                    }),

                new Buff(
                    BehaviourName.Quickdraw.CrippleBuff,
                    nameof(BehaviourName.Quickdraw.CrippleBuff).CamelCaseToWords(),
                    "This unit has only 60% of their maximum Movement (rounded up) and cannot receive healing " +
                    "from any sources until the end of its action.",
                    new List<Flag>
                    {
                        Flag.Modification.CannotBeHealed
                    },
                    new List<Modification>
                    {
                        new StatModification(
                            Change.MultiplyMax,
                            0.6f,
                            Stats.Movement)
                    },
                    null,
                    null,
                    null,
                    EndsAt.EndOf.This.Action,
                    false,
                    true,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),

                new Buff(
                    BehaviourName.Gorger.FanaticSuicideBuff,
                    nameof(BehaviourName.Gorger.FanaticSuicideBuff).CamelCaseToWords(),
                    "Upon getting killed or executing a melee attack Gorger explodes dealing its Melee Damage " +
                    "to all friendly and enemy units in 1 Distance.",
                    null,
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideSearch
                    }),

                new Buff(
                    BehaviourName.Camou.SilentAssassinBuff,
                    nameof(BehaviourName.Camou.SilentAssassinBuff).CamelCaseToWords(),
                    "This unit is silenced (use of any abilities is disabled).",
                    new List<Flag>
                    {
                        Flag.Modification.AbilitiesDisabled
                    },
                    null,
                    null,
                    null,
                    null,
                    EndsAt.EndOf.Second.Action,
                    false,
                    true,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),

                new Wait(
                    BehaviourName.Camou.ClimbWait,
                    nameof(BehaviourName.Camou.ClimbWait).CamelCaseToWords(),
                    "Camou will complete climbing on an adjacent high ground space at the end of this action phase.",
                    EndsAt.EndOf.This.ActionPhase),

                new Buff(
                    BehaviourName.Camou.ClimbBuff,
                    nameof(BehaviourName.Camou.ClimbBuff).CamelCaseToWords(),
                    "Camou can move down from high ground at the additional cost of 1 Movement.",
                    new List<Flag>
                    {
                        Flag.Modification.ClimbsDown
                    },
                    null,
                    null,
                    null,
                    null,
                    EndsAt.Death,
                    false,
                    false,
                    Alignment.Positive),

                new Wait(
                    BehaviourName.Shaman.WondrousGooFeatureWait,
                    nameof(BehaviourName.Shaman.WondrousGooFeatureWait).CamelCaseToWords(),
                    "Effect area will expand at the end of this action phase.",
                    EndsAt.EndOf.This.ActionPhase,
                    BehaviourName.Shaman.WondrousGooFeatureBuff),

                new Buff(
                    BehaviourName.Shaman.WondrousGooFeatureBuff,
                    nameof(BehaviourName.Shaman.WondrousGooFeatureBuff).CamelCaseToWords(),
                    "Effect area will disappear at the end of this action phase.",
                    null,
                    new List<Modification>
                    {
                        new SizeModification(
                            Change.SetMax,
                            3)
                    },
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Shaman.WondrousGooDestroy
                    },
                    EndsAt.EndOf.This.ActionPhase),

                new Buff(
                    BehaviourName.Shaman.WondrousGooBuff,
                    nameof(BehaviourName.Shaman.WondrousGooBuff).CamelCaseToWords(),
                    "Unit has its vision and Attack Distance reduced by 3 (total minimum of 1) " +
                    "and receives 1 Pure Damage at the start of its turn, at which point the effect ends.",
                    null,
                    new List<Modification>
                    {
                        new AttackModification(
                            Change.SubtractMax,
                            3,
                            Attacks.Melee,
                            AttackAttribute.MaxDistance),
                        new AttackModification(
                            Change.SubtractMax,
                            3,
                            Attacks.Ranged,
                            AttackAttribute.MaxDistance)
                    },
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Shaman.WondrousGooDamage
                    },
                    EndsAt.StartOf.Next.Action,
                    false,
                    false,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),

                new Tether(
                    BehaviourName.Pyre.CargoTether,
                    nameof(BehaviourName.Pyre.CargoTether).CamelCaseToWords(),
                    "The cargo follows Pyre.",
                    Location.Origin,
                    true,
                    true,
                    1,
                    true),

                new Buff(
                    BehaviourName.Pyre.CargoWallOfFlamesBuff,
                    nameof(BehaviourName.Pyre.CargoWallOfFlamesBuff).CamelCaseToWords(),
                    "The cargo leaves a path of flames when moved, which stay until the start of the next " +
                    "Pyre's action or until death. Any unit which starts its turn or moves onto the flames receives " +
                    "5 Melee Damage.",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    false,
                    false,
                    null,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.EntityIsAboutToMove
                        })
                    },
                    false,
                    new List<EffectName>
                    {
                        EffectName.Pyre.WallOfFlamesCreateEntity
                    }),

                new Buff(
                    BehaviourName.Pyre.WallOfFlamesBuff,
                    nameof(BehaviourName.Pyre.WallOfFlamesBuff).CamelCaseToWords(),
                    "",
                    null,
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Pyre.WallOfFlamesDestroy
                    },
                    EndsAt.StartOf.Next.Action,
                    false,
                    false,
                    Alignment.Negative,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    true,
                    new List<EffectName>
                    {
                        EffectName.Pyre.WallOfFlamesDestroy
                    }),

                new Buff(
                    BehaviourName.Pyre.PhantomMenaceBuff,
                    nameof(BehaviourName.Pyre.PhantomMenaceBuff).CamelCaseToWords(),
                    "Pyre can move through enemy units (but not buildings).",
                    new List<Flag>
                    {
                        Flag.Modification.MovesThroughEnemyUnits
                    }),

                new Buff(
                    BehaviourName.Roach.DegradingCarapaceBuff,
                    nameof(BehaviourName.Roach.DegradingCarapaceBuff).CamelCaseToWords(),
                    "Increases periodic damage by 1 at the start of each action.",
                    null,
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Roach.DegradingCarapacePeriodicApplyBehaviour,
                        EffectName.Roach.DegradingCarapaceApplyBehaviour
                    },
                    EndsAt.StartOf.Next.Action,
                    true,
                    false,
                    Alignment.Negative),

                new Buff(
                    BehaviourName.Roach.DegradingCarapacePeriodicDamageBuff,
                    nameof(BehaviourName.Roach.DegradingCarapacePeriodicDamageBuff).CamelCaseToWords(),
                    "This unit will continue to receive 1 damage at every start of its action.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Roach.DegradingCarapaceSelfDamage
                    },
                    null,
                    new List<EffectName>
                    {
                        EffectName.Roach.DegradingCarapacePeriodicApplyBehaviour
                    },
                    EndsAt.StartOf.Next.Action,
                    true,
                    false,
                    Alignment.Negative),

                new Tether(
                    BehaviourName.Parasite.ParalysingGraspTether,
                    nameof(BehaviourName.Parasite.ParalysingGraspTether).CamelCaseToWords(),
                    "This unit is possessed by Parasite. On Parasite turn, it moves both units using the movement " +
                    "speed that the possessed unit has. Any damage received is shared between both.",
                    Location.Inherited,
                    false,
                    true,
                    0,
                    true),

                new Buff(
                    BehaviourName.Parasite.ParalysingGraspBuff,
                    nameof(BehaviourName.Parasite.ParalysingGraspBuff).CamelCaseToWords(),
                    "This unit is possessed by Parasite. On its turn, the possessed unit controls the attack which it " +
                    "must do unless there are no legal targets.",
                    new List<Flag>
                    {
                        Flag.Modification.CanAttackAnyTeam,
                        Flag.Modification.MovementDisabled,
                        Flag.Modification.MustExecuteAttack
                    },
                    null,
                    new List<EffectName>
                    {
                        EffectName.Parasite.ParalysingGraspApplySelfBehaviour
                    },
                    null,
                    null,
                    null,
                    null,
                    null,
                    Alignment.Negative,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    true,
                    null,
                    true),

                new Buff(
                    BehaviourName.Parasite.ParalysingGraspSelfBuff,
                    nameof(BehaviourName.Parasite.ParalysingGraspSelfBuff).CamelCaseToWords(),
                    "Parasite has possessed the unit on top, gaining its movement speed and the ability to move both " +
                    "units on Parasite's turn.",
                    new List<Flag>
                    {
                        Flag.Modification.AbilitiesDisabled,
                        Flag.Modification.CannotAttack
                    },
                    new List<Modification>
                    {
                        new StatCopyModification(
                            Change.SetMax,
                            Location.Source,
                            0,
                            Stats.Movement)
                    },
                    null,
                    null,
                    null,
                    null,
                    false,
                    false,
                    Alignment.Positive,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.SourceIsDestroyed
                        })
                    },
                    true,
                    null,
                    true),

                new Buff(
                    BehaviourName.Horrior.ExpertFormationBuff,
                    nameof(BehaviourName.Horrior.ExpertFormationBuff).CamelCaseToWords(),
                    "Range Armour for this unit is increased by 2 because it is in formation with an adjacent Horrior.",
                    null,
                    new List<Modification>
                    {
                        new StatModification(
                            Change.AddMax,
                            2,
                            Stats.RangedArmour)
                    },
                    null,
                    null,
                    null,
                    EndsAt.EndOf.This.Action,
                    false,
                    true,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),

                new Wait(
                    BehaviourName.Horrior.MountWait,
                    nameof(BehaviourName.Horrior.MountWait).CamelCaseToWords(),
                    "This unit is mounting up to transform into Surfer.",
                    EndsAt.StartOf.Fourth.Planning,
                    BehaviourName.Horrior.MountBuff),

                new Buff(
                    BehaviourName.Horrior.MountBuff,
                    nameof(BehaviourName.Horrior.MountBuff).CamelCaseToWords(),
                    "",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Horrior.MountCreateEntity
                    },
                    null,
                    new List<EffectName>
                    {
                        EffectName.Horrior.MountDestroy
                    },
                    EndsAt.Instant),

                new Buff(
                    BehaviourName.Marksman.CriticalMarkBuff,
                    nameof(BehaviourName.Marksman.CriticalMarkBuff).CamelCaseToWords(),
                    "Marksman has marked this target until the end of the next action phase. The mark " +
                    "is consumed dealing 5 Melee Damage, when an ally of Marksman attacks this unit.",
                    null,
                    null,
                    null,
                    null,
                    null,
                    EndsAt.EndOf.Next.ActionPhase,
                    false,
                    true,
                    Alignment.Negative,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.EntityIsAttacked
                        }, new List<Validator>
                        {
                            new Validator(
                                new List<Condition>
                                {
                                    new Condition(Flag.Condition.TargetIsDifferentTypeThanOrigin)
                                },
                                new List<Flag>
                                {
                                    Flag.Filter.Enemy,
                                    Flag.Filter.Unit
                                })
                        })
                    },
                    true,
                    new List<EffectName>
                    {
                        EffectName.Marksman.CriticalMarkDamage
                    },
                    false),
                
                new Buff(
                    BehaviourName.Surfer.DismountBuff,
                    nameof(BehaviourName.Surfer.DismountBuff).CamelCaseToWords(),
                    "Upon death, reemerges as Horrior.",
                    null,
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Surfer.DismountCreateEntity
                    },
                    EndsAt.Death,
                    false,
                    false,
                    Alignment.Positive),
                
                new Ammunition(
                    BehaviourName.Mortar.DeadlyAmmunitionAmmunition,
                    nameof(BehaviourName.Mortar.DeadlyAmmunitionAmmunition).CamelCaseToWords(),
                    "Each ranged attack consumes 1 ammo out of 2 total. Cannot range attack when out " +
                    "of ammo. Each ranged attack deals full Ranged Damage to all adjacent units around the target.",
                    2, 
                    new List<Attacks>
                    {
                        Attacks.Ranged
                    },
                    1,
                    new List<EffectName>
                    {
                        EffectName.Mortar.DeadlyAmmunitionSearch
                    },
                    2,
                    false),
                
                new Wait(
                    BehaviourName.Mortar.ReloadWait,
                    nameof(BehaviourName.Mortar.ReloadWait).CamelCaseToWords(),
                    "Mortar will reload its ammunition at the end of this action.",
                    EndsAt.EndOf.This.Action,
                    BehaviourName.Mortar.ReloadBuff),
                
                new Buff(
                    BehaviourName.Mortar.ReloadBuff,
                    nameof(BehaviourName.Mortar.ReloadBuff).CamelCaseToWords(),
                    "",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Mortar.ReloadReload
                    },
                    null,
                    null,
                    EndsAt.Instant),
                
                new Buff(
                    BehaviourName.Mortar.PiercingBlastBuff,
                    nameof(BehaviourName.Mortar.PiercingBlastBuff).CamelCaseToWords(),
                    "Ranged Armour from the main target is ignored when attacking with Deadly Ammunition.",
                    null,
                    new List<Modification>
                    {
                        new AttackModification(
                            Attacks.Ranged, 
                            new List<Flag>
                            {
                                Flag.Modification.IgnoreArmour
                            })
                    },
                    null,
                    null,
                    null,
                    EndsAt.Death,
                    null,
                    null,
                    Alignment.Positive),
                
                new Buff(
                    BehaviourName.Hawk.TacticalGogglesBuff,
                    nameof(BehaviourName.Hawk.TacticalGogglesBuff).CamelCaseToWords(),
                    "Gains +3 Vision range.",
                    null,
                    new List<Modification>
                    {
                        new StatModification(Change.AddMax, 3, Stats.Vision)
                    },
                    null,
                    null,
                    null,
                    EndsAt.Death,
                    false,
                    null,
                    Alignment.Positive),
                
                new Buff(
                    BehaviourName.Hawk.LeadershipBuff,
                    nameof(BehaviourName.Hawk.LeadershipBuff).CamelCaseToWords(),
                    "Gains +1 Attack Distance range from nearby Hawk. Bonus will be lost at the end of " +
                    "the next action or if Hawk is not adjacent anymore.",
                    null,
                    new List<Modification>
                    {
                        new AttackModification(
                            Change.AddMax, 
                            1, 
                            Attacks.Ranged, 
                            AttackAttribute.MaxDistance)
                    },
                    null,
                    null,
                    null,
                    EndsAt.EndOf.This.Action,
                    false,
                    null,
                    Alignment.Positive,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.SourceIsNotAdjacent
                        })
                    },
                    true,
                    null,
                    true),
                
                new Buff(
                    BehaviourName.Hawk.HealthKitBuff,
                    nameof(BehaviourName.Hawk.HealthKitBuff).CamelCaseToWords(),
                    "Restores 1 Health to all adjacent friendly units at the start of each planning phase.",
                    null,
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Hawk.HealthKitSearch,
                        EffectName.Hawk.HealthKitApplyBehaviour // Reapply the same buff to achieve periodic effect
                    },
                    EndsAt.StartOf.Next.Planning,
                    true, // Only needed because final effects happen right before this behaviour is destroyed
                    null,
                    Alignment.Positive),
                
                new Buff(
                    BehaviourName.Hawk.HealthKitHealBuff,
                    nameof(BehaviourName.Hawk.HealthKitHealBuff).CamelCaseToWords(),
                    "Heals for 1 Health.",
                    null,
                    new List<Modification>
                    {
                        new StatModification(
                            Change.AddCurrent,
                            1,
                            Stats.Health)
                    },
                    null,
                    null,
                    null,
                    EndsAt.Instant,
                    false,
                    false,
                    Alignment.Positive),
                
                new Buildable(
                    BehaviourName.Cannon.AssemblingBuildable,
                    nameof(BehaviourName.Cannon.AssemblingBuildable).CamelCaseToWords(),
                    "This machine can only be placed on the low ground and can be assembled by a maximum of " +
                    "3 Engineers at once.",
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.TargetIsLowGround)
                        })
                    },
                    3),
                
                new Buildable(
                    BehaviourName.Ballista.AssemblingBuildable,
                    nameof(BehaviourName.Ballista.AssemblingBuildable).CamelCaseToWords(),
                    "This machine can only be placed on a Watchtower and can be assembled by a maximum of " +
                    "1 Engineer at once.",
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourExists) // TODO add when the Watchtower structure has
                                                                          // the appropriate behaviour created
                        })
                    },
                    1),
                
                new Buildable(
                    BehaviourName.Radar.AssemblingBuildable,
                    nameof(BehaviourName.Radar.AssemblingBuildable).CamelCaseToWords(),
                    "This machine can only be placed on the low ground and can be assembled by a maximum of " +
                    "1 Engineer at once.",
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.TargetIsLowGround)
                        })
                    },
                    1),
                
                new Buff(
                    BehaviourName.Engineer.OperateBuff,
                    nameof(BehaviourName.Engineer.OperateBuff).CamelCaseToWords(),
                    "",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Engineer.OperateModifyCounter
                    },
                    null,
                    new List<EffectName>
                    {
                        EffectName.Engineer.OperateDestroy
                    },
                    EndsAt.Instant),
                
                new Buff(
                    BehaviourName.Engineer.RepairStructureOrMachineBuff,
                    nameof(BehaviourName.Engineer.RepairStructureOrMachineBuff).CamelCaseToWords(),
                    "This structure or machine will be repaired by +2 Health at the start of the planning phase.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Engineer.RepairApplyBehaviourSelf
                    },
                    new List<Modification>
                    {
                        new StatModification(
                            Change.AddCurrent,
                            1,
                            Stats.Health)
                    },
                    null,
                    EndsAt.StartOf.Next.Planning,
                    true,
                    false,
                    Alignment.Positive,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsInterrupted
                        })
                    },
                    true),
                
                new Buff(
                    BehaviourName.Engineer.RepairHorriorBuff,
                    nameof(BehaviourName.Engineer.RepairHorriorBuff).CamelCaseToWords(),
                    "This Horrior will have their Mount duration reduced by one turn at the start of the " +
                    "planning phase.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Engineer.RepairApplyBehaviourSelf
                    },
                    new List<Modification>
                    {
                        new DurationModification(
                            Change.SubtractCurrent, 
                            1f,
                            BehaviourName.Horrior.MountWait)
                    },
                    null,
                    EndsAt.StartOf.Next.Planning,
                    true,
                    false,
                    Alignment.Positive,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsInterrupted
                        })
                    },
                    true),

                new Wait(
                    BehaviourName.Engineer.RepairWait,
                    nameof(BehaviourName.Engineer.RepairWait).CamelCaseToWords(),
                    "Currently repairing.",
                    EndsAt.StartOf.Next.Planning),
                
                new Counter(
                    BehaviourName.Cannon.MachineCounter,
                    nameof(BehaviourName.Cannon.MachineCounter).CamelCaseToWords(),
                    "Needs 3 Engineers to operate this machine.",
                    3,
                    3,
                    new List<EffectName>
                    {
                        EffectName.Cannon.MachineRemoveBehaviour
                    }),
                
                new Buff(
                    BehaviourName.Cannon.MachineBuff,
                    nameof(BehaviourName.Cannon.MachineBuff).CamelCaseToWords(),
                    "This machine is disabled until it is fully operated by the required number of Engineers.",
                    new List<Flag>
                    {
                        Flag.Modification.FullyDisabled
                    },
                    null,
                    null,
                    null,
                    null,
                    EndsAt.Death,
                    false,
                    null,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),
                
                new Buff(
                    BehaviourName.Cannon.HeatUpDangerZoneBuff,
                    nameof(BehaviourName.Cannon.HeatUpDangerZoneBuff).CamelCaseToWords(),
                    "This tile will receive massive damage on the next Cannon's turn. Until then, Cannon's " +
                    "owner has vision of this tile.",
                    new List<Flag>
                    {
                        Flag.Modification.ProvidesVision
                    },
                    new List<Modification>
                    {
                        new StatModification(Change.SetCurrent, 1, Stats.Vision)
                    },
                    new List<EffectName>
                    {
                        EffectName.Cannon.HeatUpApplyWaitBehaviour
                    },
                    null,
                    new List<EffectName>
                    {
                        EffectName.Cannon.HeatUpSearch,
                        EffectName.Cannon.HeatUpDestroy
                    },
                    EndsAt.EndOf.Next.Action,
                    false,
                    false,
                    Alignment.Positive,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsInterrupted
                        }),
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    true,
                    new List<EffectName>
                    {
                        EffectName.Cannon.MachineRemoveBehaviour,
                        EffectName.Cannon.HeatUpDestroy
                    },
                    true),
                
                new Wait(
                    BehaviourName.Cannon.HeatUpWait,
                    nameof(BehaviourName.Cannon.HeatUpWait).CamelCaseToWords(),
                    "This Cannon is heating up for a blast at the danger zone.",
                    EndsAt.EndOf.Next.Action),
                
                new Counter(
                    BehaviourName.Ballista.MachineCounter,
                    nameof(BehaviourName.Ballista.MachineCounter).CamelCaseToWords(),
                    "Needs 1 Engineer to operate this machine.",
                    3,
                    3,
                    new List<EffectName>
                    {
                        EffectName.Ballista.MachineRemoveBehaviour
                    }),
                
                new Buff(
                    BehaviourName.Ballista.MachineBuff,
                    nameof(BehaviourName.Ballista.MachineBuff).CamelCaseToWords(),
                    "This machine is disabled until it is fully operated by the required number of Engineers.",
                    new List<Flag>
                    {
                        Flag.Modification.FullyDisabled
                    },
                    null,
                    null,
                    null,
                    null,
                    EndsAt.Death,
                    false,
                    null,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),
                
                new Buff(
                    BehaviourName.Ballista.AimBuff,
                    nameof(BehaviourName.Ballista.AimBuff).CamelCaseToWords(),
                    "This unit is aimed by a Ballista, which allows it to shoot every turn as long as this " +
                    "unit remains in range.",
                    null,
                    null,
                    null,
                    null,
                    null,
                    EndsAt.EndOf.Next.ActionPhase,
                    false,
                    true,
                    Alignment.Negative,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.EntityFinishedMoving
                        },new List<Validator> 
                        {
                            new ResultValidator(
                                EffectName.Ballista.AimSearch, 
                                new List<Condition>
                                {
                                    new Condition(Flag.Condition.NoActorsFoundFromEffect)
                                })
                        }),
                        new Trigger(new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    true,
                    null,
                    null,
                    true,
                    false),
                
                new Counter(
                    BehaviourName.Radar.MachineCounter,
                    nameof(BehaviourName.Radar.MachineCounter).CamelCaseToWords(),
                    "Needs 1 Engineer to operate this machine.",
                    3,
                    3,
                    new List<EffectName>
                    {
                        EffectName.Radar.MachineRemoveBehaviour
                    }),
                
                new Buff(
                    BehaviourName.Radar.MachineBuff,
                    nameof(BehaviourName.Radar.MachineBuff).CamelCaseToWords(),
                    "This machine is disabled until it is fully operated by the required number of Engineers.",
                    new List<Flag>
                    {
                        Flag.Modification.FullyDisabled
                    },
                    null,
                    null,
                    null,
                    null,
                    EndsAt.Death,
                    false,
                    null,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),
                
                new Buff(
                    BehaviourName.Radar.PowerDependencyBuff,
                    nameof(BehaviourName.Radar.PowerDependencyBuff).CamelCaseToWords(),
                    "If this unit is not connected to Power, all abilities get disabled and it loses 1 Health " +
                    "at the start of its action.",
                    null,
                    null,
                    null,
                    null,
                    null,
                    EndsAt.Death,
                    false,
                    false,
                    Alignment.Neutral,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.EntityStartsActionNotOnPower
                        })
                    },
                    false,
                    new List<EffectName>
                    {
                        EffectName.Radar.PowerDependencyDamage,
                        EffectName.Radar.PowerDependencyApplyBehaviourDisable,
                    }),
                
                new Buff(
                    BehaviourName.Radar.PowerDependencyBuffDisable,
                    nameof(BehaviourName.Radar.PowerDependencyBuffDisable).CamelCaseToWords(),
                    "All abilities are disabled because Power is not supplied.",
                    new List<Flag>
                    {
                        Flag.Modification.AbilitiesDisabled
                    },
                    null,
                    null,
                    null,
                    null,
                    EndsAt.StartOf.Next.ActionPhase,
                    false,
                    true,
                    Alignment.Negative,
                    new List<Trigger>
                    {
                        new Trigger(new List<Event>
                        {
                            Event.EntityReceivedPower
                        })
                    },
                    true,
                    null,
                    true),
                
                new Buff(
                    BehaviourName.Radar.ResonatingSweepBuff,
                    nameof(BehaviourName.Radar.ResonatingSweepBuff).CamelCaseToWords(),
                    "Provides vision to the Radar's owner.",
                    new List<Flag>
                    {
                        Flag.Modification.ProvidesVision
                    },
                    new List<Modification>
                    {
                        new StatModification(Change.SetCurrent, 1, Stats.Vision)
                    },
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Radar.ResonatingSweepDestroy
                    },
                    EndsAt.StartOf.Next.Planning,
                    false,
                    false,
                    Alignment.Positive),
                
                new Buff(
                    BehaviourName.Radar.RadioLocationBuff,
                    nameof(BehaviourName.Radar.RadioLocationBuff).CamelCaseToWords(),
                    "",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Radar.RadioLocationSearchDestroy
                    },
                    null,
                    new List<EffectName>
                    {
                        EffectName.Radar.RadioLocationSearchCreate
                    },
                    EndsAt.Instant,
                    false,
                    false),
                
                new Buff(
                    BehaviourName.Radar.RadioLocationFeatureBuff,
                    nameof(BehaviourName.Radar.RadioLocationFeatureBuff).CamelCaseToWords(),
                    "This red dot is able to detect enemy units in the fog of war.",
                    new List<Flag>
                    {
                        Flag.Modification.OnlyVisibleToAllies,
                        Flag.Modification.OnlyVisibleInFogOfWar
                    },
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Radar.RadioLocationDestroy
                    },
                    EndsAt.EndOf.This.Planning),
                
                new Counter(
                    BehaviourName.Vessel.MachineCounter,
                    nameof(BehaviourName.Vessel.MachineCounter).CamelCaseToWords(),
                    "Needs 3 Engineers to operate this machine.",
                    3,
                    3,
                    new List<EffectName>
                    {
                        EffectName.Vessel.MachineRemoveBehaviour
                    }),
                
                new Buff(
                    BehaviourName.Vessel.MachineBuff,
                    nameof(BehaviourName.Vessel.MachineBuff).CamelCaseToWords(),
                    "This machine is disabled until it is fully operated by the required number of Engineers.",
                    new List<Flag>
                    {
                        Flag.Modification.FullyDisabled
                    },
                    null,
                    null,
                    null,
                    null,
                    EndsAt.Death,
                    false,
                    null,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),
            };
        }
    }
}