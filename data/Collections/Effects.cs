using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;
using low_age_data.Domain.Shared.Modifications;
using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Features;

namespace low_age_data.Collections
{
    public static class Effects
    {
        public static List<Effect> Get()
        {
            return new List<Effect>
            {
                new ApplyBehaviour(
                    EffectName.Leader.AllForOneApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Leader.AllForOneBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new ModifyPlayer(
                    EffectName.Leader.AllForOnePlayerLoses,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    },
                    new List<Flag>
                    {
                        Flag.Effect.ModifyPlayer.GameLost
                    }),

                new Search(
                    EffectName.Leader.MenacingPresenceSearch,
                    6,
                    new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.AppliedOnAction,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Leader.MenacingPresenceApplyBehaviour
                    },
                    Location.Self),

                new ApplyBehaviour(
                    EffectName.Leader.MenacingPresenceApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Leader.MenacingPresenceBuff
                    }),

                new ApplyBehaviour(
                    EffectName.Leader.OneForAllApplyBehaviourObelisk,
                    new List<BehaviourName>
                    {
                        BehaviourName.Leader.OneForAllObeliskBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure,
                        Flag.Filter.SpecificStructure.Obelisk
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourDoesNotExist, BehaviourName.Leader.OneForAllObeliskBuff)
                        })
                    }),

                new Search(
                    EffectName.Leader.OneForAllSearch,
                    1,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Leader.OneForAllApplyBehaviourHeal
                    },
                    Location.Self,
                    Shape.Map),

                new ApplyBehaviour(
                    EffectName.Leader.OneForAllApplyBehaviourHeal,
                    new List<BehaviourName>
                    {
                        BehaviourName.Leader.OneForAllHealBuff
                    },
                    Location.Self),

                new ApplyBehaviour(
                    EffectName.Slave.RepairApplyBehaviourStructure,
                    new List<BehaviourName>
                    {
                        BehaviourName.Slave.RepairStructureBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.TargetDoesNotHaveFullHealth)
                        })
                    }),

                new ApplyBehaviour(
                    EffectName.Slave.RepairApplyBehaviourSelf,
                    new List<BehaviourName>
                    {
                        BehaviourName.Slave.RepairWait
                    },
                    Location.Origin),

                new ApplyBehaviour(
                    EffectName.Slave.ManualLabourApplyBehaviourHut,
                    new List<BehaviourName>
                    {
                        BehaviourName.Slave.ManualLabourBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure,
                        Flag.Filter.SpecificStructure.Hut
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourDoesNotExist, BehaviourName.Slave.ManualLabourBuff)
                        })
                    }),

                new ApplyBehaviour(
                    EffectName.Slave.ManualLabourApplyBehaviourSelf,
                    new List<BehaviourName>
                    {
                        BehaviourName.Slave.ManualLabourWait
                    },
                    Location.Origin),

                new ModifyPlayer(
                    EffectName.Slave.ManualLabourModifyPlayer,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    },
                    null,
                    new List<ResourceModification>
                    {
                        new ResourceModification(
                            Change.AddCurrent,
                            2.0f,
                            Resources.Scraps)
                    }),

                new ApplyBehaviour(
                    EffectName.Quickdraw.DoubleshotApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Quickdraw.DoubleshotExtraAttack
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new ApplyBehaviour(
                    EffectName.Quickdraw.CrippleApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Quickdraw.CrippleBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),

                new ApplyBehaviour(
                    EffectName.Gorger.FanaticSuicideApplyBehaviourBuff,
                    new List<BehaviourName>
                    {
                        BehaviourName.Gorger.FanaticSuicideBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new Destroy(
                    EffectName.Gorger.FanaticSuicideDestroy,
                    Location.Origin),

                new Search(
                    EffectName.Gorger.FanaticSuicideSearch,
                    1,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideDamage
                    },
                    Location.Self),

                new Damage(
                    EffectName.Gorger.FanaticSuicideDamage,
                    DamageType.OverrideMelee),

                new Damage(
                    EffectName.Camou.SilentAssassinOnHitDamage,
                    DamageType.CurrentMelee,
                    new Amount(
                        0,
                        0.5f,
                        Flag.Amount.FromMissingHealth,
                        new List<Flag>
                        {
                            Flag.Filter.Enemy,
                            Flag.Filter.Unit
                        }),
                    null,
                    null,
                    null,
                    null,
                    new List<Validator>
                    {
                        new ResultValidator(
                            EffectName.Camou.SilentAssassinSearchFriendly,
                            new List<Condition>
                            {
                                new Condition(Flag.Condition.NoActorsFoundFromEffect)
                            })
                    }),

                new ApplyBehaviour(
                    EffectName.Camou.SilentAssassinOnHitSilence,
                    new List<BehaviourName>
                    {
                        BehaviourName.Camou.SilentAssassinBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    null,
                    new List<Validator>
                    {
                        new ResultValidator(
                            EffectName.Camou.SilentAssassinSearchFriendly,
                            new List<Condition>
                            {
                                new Condition(Flag.Condition.NoActorsFoundFromEffect)
                            }),

                        new ResultValidator(
                            EffectName.Camou.SilentAssassinSearchEnemy,
                            new List<Condition>
                            {
                                new Condition(Flag.Condition.NoActorsFoundFromEffect)
                            })
                    }),

                new Search(
                    EffectName.Camou.SilentAssassinSearchFriendly,
                    4,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.Ally
                    },
                    null,
                    Location.Origin,
                    Shape.Circle,
                    null,
                    null,
                    true),

                new Search(
                    EffectName.Camou.SilentAssassinSearchEnemy,
                    4,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.Enemy
                    },
                    null,
                    Location.Actor,
                    Shape.Circle,
                    null,
                    null,
                    true),

                new Teleport(
                    EffectName.Camou.ClimbTeleport,
                    BehaviourName.Camou.ClimbWait,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.TargetIsHighGround)
                        }),
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.TargetIsUnoccupied)
                        })
                    }),

                new ApplyBehaviour(
                    EffectName.Camou.ClimbApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Camou.ClimbBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new CreateEntity(
                    EffectName.Shaman.WondrousGooCreateEntity,
                    FeatureName.ShamanWondrousGoo,
                    new List<BehaviourName>
                    {
                        BehaviourName.Shaman.WondrousGooFeatureWait
                    }),

                new Search(
                    EffectName.Shaman.WondrousGooSearch,
                    0,
                    new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnAction,
                        Flag.Effect.Search.AppliedOnEnter
                    },
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Shaman.WondrousGooApplyBehaviour
                    },
                    Location.Self),

                new ApplyBehaviour(
                    EffectName.Shaman.WondrousGooApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Shaman.WondrousGooBuff
                    }),

                new Destroy(
                    EffectName.Shaman.WondrousGooDestroy),

                new Damage(
                    EffectName.Shaman.WondrousGooDamage,
                    DamageType.Pure,
                    new Amount(1)),

                new CreateEntity(
                    EffectName.Pyre.CargoCreateEntity,
                    FeatureName.PyreCargo,
                    new List<BehaviourName>
                    {
                        BehaviourName.Pyre.CargoTether,
                        BehaviourName.Pyre.CargoWallOfFlamesBuff
                    }),

                new CreateEntity(
                    EffectName.Pyre.WallOfFlamesCreateEntity,
                    FeatureName.PyreFlames),

                new Destroy(
                    EffectName.Pyre.WallOfFlamesDestroy),

                new Damage(
                    EffectName.Pyre.WallOfFlamesDamage,
                    DamageType.Melee,
                    new Amount(5)),

                new ApplyBehaviour(
                    EffectName.Pyre.PhantomMenaceApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Pyre.PhantomMenaceBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new Search(
                    EffectName.BigBadBull.UnleashTheRageSearch,
                    1,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.BigBadBull.UnleashTheRageDamage,
                        EffectName.BigBadBull.UnleashTheRageForce
                    },
                    Location.Point,
                    Shape.Line),

                new Damage(
                    EffectName.BigBadBull.UnleashTheRageDamage,
                    DamageType.OverrideMelee),

                new Force(
                    EffectName.BigBadBull.UnleashTheRageForce,
                    Location.Origin, 
                    1,
                    new List<EffectName>
                    {
                        EffectName.BigBadBull.UnleashTheRageForceDamage
                    }),

                new Damage(
                    EffectName.BigBadBull.UnleashTheRageForceDamage,
                    DamageType.Melee,
                    new Amount(5)),

                new CreateEntity(
                    EffectName.Mummy.SpawnRoachCreateEntity,
                    UnitName.Roach),

                new ModifyAbility(
                    EffectName.Mummy.LeapOfHungerModifyAbility,
                    AbilityName.Mummy.SpawnRoach,
                    AbilityName.Mummy.SpawnRoachModified),

                new ApplyBehaviour(
                    EffectName.Roach.DegradingCarapaceApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Roach.DegradingCarapaceBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new ApplyBehaviour(
                    EffectName.Roach.DegradingCarapacePeriodicApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Roach.DegradingCarapacePeriodicDamageBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new Damage(
                    EffectName.Roach.DegradingCarapaceSelfDamage,
                    DamageType.Pure,
                    new Amount(1)),

                new Damage(
                    EffectName.Roach.CorrosiveSpitDamage,
                    DamageType.Ranged,
                    new Amount(6),
                    CombatAttributes.Mechanical,
                    new Amount(8),
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    }),

                new ApplyBehaviour(
                    EffectName.Parasite.ParalysingGraspApplyTetherBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Parasite.ParalysingGraspTether
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourDoesNotExist, BehaviourName.Parasite.ParalysingGraspTether)
                        })
                    }),

                new ApplyBehaviour(
                    EffectName.Parasite.ParalysingGraspApplyAttackBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Parasite.ParalysingGraspBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourDoesNotExist, BehaviourName.Parasite.ParalysingGraspBuff)
                        })
                    }),

                new ApplyBehaviour(
                    EffectName.Parasite.ParalysingGraspApplySelfBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Parasite.ParalysingGraspSelfBuff
                    },
                    Location.Source,
                    new List<Flag>
                    {
                        Flag.Filter.Unit
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourDoesNotExist, BehaviourName.Parasite.ParalysingGraspSelfBuff)
                        })
                    }),

                new Search(
                    EffectName.Horrior.ExpertFormationSearch,
                    1,
                    new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.AppliedOnAction,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Horrior
                    },
                    new List<EffectName>
                    {
                        EffectName.Horrior.ExpertFormationApplyBehaviour
                    },
                    Location.Self),

                new ApplyBehaviour(
                    EffectName.Horrior.ExpertFormationApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Horrior.ExpertFormationBuff
                    }),

                new ApplyBehaviour(
                    EffectName.Horrior.MountApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Horrior.MountWait
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new CreateEntity(
                    EffectName.Horrior.MountCreateEntity,
                    UnitName.Surfer),

                new Destroy(
                    EffectName.Horrior.MountDestroy,
                    Location.Self),

                new ApplyBehaviour(
                    EffectName.Marksman.CriticalMarkApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Marksman.CriticalMarkBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),
                
                new Damage(
                    EffectName.Marksman.CriticalMarkDamage,
                    DamageType.Melee,
                    new Amount(5),
                    null,
                    null,
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    EffectName.Surfer.DismountApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Surfer.DismountBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),
                
                new CreateEntity(
                    EffectName.Surfer.DismountCreateEntity,
                    UnitName.Horrior),
                
                new ApplyBehaviour(
                    EffectName.Mortar.DeadlyAmmunitionApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Mortar.DeadlyAmmunitionAmmunition
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Search(
                    EffectName.Mortar.DeadlyAmmunitionSearch,
                    1,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Mortar.DeadlyAmmunitionDamage
                    },
                    Location.Inherited,
                    Shape.Circle,
                    true),
                
                new Damage(
                    EffectName.Mortar.DeadlyAmmunitionDamage,
                    DamageType.OverrideRanged),
                
                new ApplyBehaviour(
                    EffectName.Mortar.ReloadApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Mortar.ReloadWait
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Reload(
                    EffectName.Mortar.ReloadReload,
                    BehaviourName.Mortar.DeadlyAmmunitionAmmunition,
                    Location.Self),
                
                new ApplyBehaviour(
                    EffectName.Mortar.PiercingBlastApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Mortar.PiercingBlastBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    EffectName.Hawk.TacticalGogglesApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Hawk.TacticalGogglesBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    EffectName.Hawk.LeadershipApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Hawk.LeadershipBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.Attribute.Ranged
                    }),
                
                new ApplyBehaviour(
                    EffectName.Hawk.HealthKitApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Hawk.HealthKitBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Search(
                    EffectName.Hawk.HealthKitSearch,
                    1,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Hawk.HealthKitHealApplyBehaviour
                    },
                    Location.Self,
                    Shape.Circle,
                    true),
                
                new ApplyBehaviour(
                    EffectName.Hawk.HealthKitHealApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Hawk.HealthKitHealBuff
                    }),

                new ApplyBehaviour(
                    EffectName.Engineer.OperateApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Engineer.OperateBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Cannon,
                        Flag.Filter.SpecificUnit.Ballista,
                        Flag.Filter.SpecificUnit.Radar,
                        Flag.Filter.SpecificUnit.Vessel
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourDoesNotExist, BehaviourName.Cannon.AssemblingBuildable)
                        }),
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourDoesNotExist, BehaviourName.Ballista.AssemblingBuildable)
                        }),
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourDoesNotExist, BehaviourName.Radar.AssemblingBuildable)
                        })
                    }),
                
                new ModifyCounter(
                    EffectName.Engineer.OperateModifyCounter,
                    new List<BehaviourName>
                    {
                        BehaviourName.Cannon.MachineCounter
                        // TODO add with each machine
                    },
                    Change.AddCurrent, 
                    1,
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),
                
                new Destroy(
                    EffectName.Engineer.OperateDestroy,
                    Location.Origin),
                
                new ApplyBehaviour(
                    EffectName.Engineer.RepairStructureApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Engineer.RepairStructureOrMachineBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.TargetDoesNotHaveFullHealth)
                        })
                    }),
                
                new ApplyBehaviour(
                    EffectName.Engineer.RepairMachineApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Engineer.RepairStructureOrMachineBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Cannon,
                        Flag.Filter.SpecificUnit.Ballista,
                        Flag.Filter.SpecificUnit.Radar,
                        Flag.Filter.SpecificUnit.Vessel
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.TargetDoesNotHaveFullHealth)
                        })
                    }),
                
                new ApplyBehaviour( 
                    EffectName.Engineer.RepairHorriorApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Engineer.RepairHorriorBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Horrior
                    },
                    null,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(Flag.Condition.BehaviourExists, BehaviourName.Horrior.MountWait)
                        })
                    }),
                
                new ApplyBehaviour(
                    EffectName.Engineer.RepairApplyBehaviourSelf,
                    new List<BehaviourName>
                    {
                        BehaviourName.Engineer.RepairWait
                    },
                    Location.Origin),
                
                new ApplyBehaviour(
                    EffectName.Cannon.MachineApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Cannon.MachineCounter,
                        BehaviourName.Cannon.MachineBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new RemoveBehaviour(
                    EffectName.Cannon.MachineRemoveBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Cannon.MachineBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new CreateEntity(
                    EffectName.Cannon.HeatUpCreateEntity,
                    FeatureName.CannonHeatUpDangerZone,
                    new List<BehaviourName>
                    {
                        BehaviourName.Cannon.HeatUpDangerZoneBuff
                    }),
                
                new ApplyBehaviour(
                    EffectName.Cannon.HeatUpApplyWaitBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Cannon.HeatUpWait
                    },
                    Location.Origin,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    }),
                
                new Search(
                    EffectName.Cannon.HeatUpSearch,
                    0,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Cannon.HeatUpDamage
                    },
                    Location.Self),
                
                new Damage(
                    EffectName.Cannon.HeatUpDamage,
                    DamageType.OverrideRanged),
                
                new Destroy(
                    EffectName.Cannon.HeatUpDestroy,
                    Location.Self),
                
                new RemoveBehaviour(
                    EffectName.Cannon.HeatUpRemoveBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Cannon.HeatUpWait
                    },
                    Location.Origin,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    EffectName.Ballista.MachineApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Ballista.MachineCounter,
                        BehaviourName.Ballista.MachineBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new RemoveBehaviour(
                    EffectName.Ballista.MachineRemoveBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Ballista.MachineBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Damage(
                    EffectName.Ballista.AimDamage,
                    DamageType.OverrideRanged,
                    null,
                    null,
                    null,
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    },
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            new Condition(
                                Flag.Condition.BehaviourExists, 
                                BehaviourName.Ballista.AimBuff, 
                                Location.Origin)
                        })
                    }),
                
                new ApplyBehaviour(
                    EffectName.Ballista.AimApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Ballista.AimBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    },
                    Location.Origin),
                
                new Search(
                    EffectName.Ballista.AimSearch,
                    9,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Origin,
                        Flag.Filter.SpecificUnit.Ballista
                    },
                    null,
                    Location.Self,
                    null,
                    null,
                    null,
                    true),
                
                new ApplyBehaviour(
                    EffectName.Radar.MachineApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Radar.MachineCounter,
                        BehaviourName.Radar.MachineBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new RemoveBehaviour(
                    EffectName.Radar.MachineRemoveBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Radar.MachineBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    EffectName.Radar.PowerDependencyApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Radar.PowerDependencyBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Self
                    }),
                
                new Damage(
                    EffectName.Radar.PowerDependencyDamage,
                    DamageType.Pure,
                    new Amount(1)),
                
                new ApplyBehaviour(
                    EffectName.Radar.PowerDependencyApplyBehaviourDisable,
                    new List<BehaviourName>
                    {
                        BehaviourName.Radar.PowerDependencyBuffDisable
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Self
                    }),
                
                new CreateEntity(
                    EffectName.Radar.ResonatingSweepCreateEntity,
                    FeatureName.RadarResonatingSweep,
                    new List<BehaviourName>
                    {
                        BehaviourName.Radar.ResonatingSweepBuff
                    }),
                
                new Destroy(
                    EffectName.Radar.ResonatingSweepDestroy,
                    Location.Self)
            };
        }
    }
}
