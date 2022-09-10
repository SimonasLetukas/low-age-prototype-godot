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
                    name: EffectName.Leader.AllForOneApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Leader.AllForOneBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new ModifyPlayer(
                    name: EffectName.Leader.AllForOnePlayerLoses,
                    playerFilterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    },
                    modifyFlags: new List<Flag>
                    {
                        Flag.Effect.ModifyPlayer.GameLost
                    }),

                new Search(
                    name: EffectName.Leader.MenacingPresenceSearch,
                    radius: 6,
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.AppliedOnActorAction,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Leader.MenacingPresenceApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Leader.MenacingPresenceApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Leader.MenacingPresenceBuff
                    }),

                new ApplyBehaviour(
                    name: EffectName.Leader.OneForAllApplyBehaviourObelisk,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Leader.OneForAllObeliskBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure,
                        Flag.Filter.SpecificStructure.Obelisk
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourDoesNotExist, 
                                conditionedBehaviour: BehaviourName.Leader.OneForAllObeliskBuff)
                        })
                    }),

                new Search(
                    name: EffectName.Leader.OneForAllSearch,
                    radius: 1,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Leader.OneForAllApplyBehaviourHeal
                    },
                    location: Location.Self,
                    shape: Shape.Map),

                new ApplyBehaviour(
                    name: EffectName.Leader.OneForAllApplyBehaviourHeal,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Leader.OneForAllHealBuff
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Slave.RepairApplyBehaviourStructure,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Slave.RepairStructureBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.TargetDoesNotHaveFullHealth)
                        })
                    }),

                new ApplyBehaviour(
                    name: EffectName.Slave.RepairApplyBehaviourSelf,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Slave.RepairWait
                    },
                    location: Location.Origin),

                new ApplyBehaviour(
                    name: EffectName.Slave.ManualLabourApplyBehaviourHut,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Slave.ManualLabourBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure,
                        Flag.Filter.SpecificStructure.Hut
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourDoesNotExist, 
                                conditionedBehaviour: BehaviourName.Slave.ManualLabourBuff)
                        })
                    }),

                new ApplyBehaviour(
                    name: EffectName.Slave.ManualLabourApplyBehaviourSelf,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Slave.ManualLabourWait
                    },
                    location: Location.Origin),

                new ModifyPlayer(
                    name: EffectName.Slave.ManualLabourModifyPlayer,
                    playerFilterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    },
                    modifyFlags: null,
                    resourceModifications: new List<ResourceModification>
                    {
                        new(
                            change: Change.AddCurrent,
                            amount: 2.0f,
                            resource: Resources.Scraps)
                    }),

                new ApplyBehaviour(
                    name: EffectName.Quickdraw.DoubleshotApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Quickdraw.DoubleshotExtraAttack
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new ApplyBehaviour(
                    name: EffectName.Quickdraw.CrippleApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Quickdraw.CrippleBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),

                new ApplyBehaviour(
                    name: EffectName.Gorger.FanaticSuicideApplyBehaviourBuff,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Gorger.FanaticSuicideBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new Destroy(
                    name: EffectName.Gorger.FanaticSuicideDestroy,
                    target: Location.Origin),

                new Search(
                    name: EffectName.Gorger.FanaticSuicideSearch,
                    radius: 1,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideDamage
                    },
                    location: Location.Self),

                new Damage(
                    name: EffectName.Gorger.FanaticSuicideDamage,
                    damageType: DamageType.OverrideMelee),

                new Damage(
                    name: EffectName.Camou.SilentAssassinOnHitDamage,
                    damageType: DamageType.CurrentMelee,
                    amount: new Amount(
                        flat: 0,
                        multiplier: 0.5f,
                        multiplierOf: Flag.Amount.FromMissingHealth,
                        multiplierFlags: new List<Flag>
                        {
                            Flag.Filter.Enemy,
                            Flag.Filter.Unit
                        }),
                    bonusTo: null,
                    bonusAmount: null,
                    location: null,
                    filterFlags: null,
                    validators: new List<Validator>
                    {
                        new ResultValidator(
                            searchEffect: EffectName.Camou.SilentAssassinSearchFriendly,
                            conditions: new List<Condition>
                            {
                                new(conditionFlag: Flag.Condition.NoActorsFoundFromEffect)
                            })
                    }),

                new ApplyBehaviour(
                    name: EffectName.Camou.SilentAssassinOnHitSilence,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Camou.SilentAssassinBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new ResultValidator(
                            searchEffect: EffectName.Camou.SilentAssassinSearchFriendly,
                            conditions: new List<Condition>
                            {
                                new(conditionFlag: Flag.Condition.NoActorsFoundFromEffect)
                            }),

                        new ResultValidator(
                            searchEffect: EffectName.Camou.SilentAssassinSearchEnemy,
                            conditions: new List<Condition>
                            {
                                new(conditionFlag: Flag.Condition.NoActorsFoundFromEffect)
                            })
                    }),

                new Search(
                    name: EffectName.Camou.SilentAssassinSearchFriendly,
                    radius: 4,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.Ally
                    },
                    effects: null,
                    location: Location.Origin,
                    shape: Shape.Circle,
                    ignoreCenter: null,
                    validators: null,
                    usedForValidator: true),

                new Search(
                    name: EffectName.Camou.SilentAssassinSearchEnemy,
                    radius: 4,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.Enemy
                    },
                    effects: null,
                    location: Location.Actor,
                    shape: Shape.Circle,
                    ignoreCenter: null,
                    validators: null,
                    usedForValidator: true),

                new Teleport(
                    name: EffectName.Camou.ClimbTeleport,
                    waitBefore: BehaviourName.Camou.ClimbWait,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.TargetIsHighGround)
                        }),
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.TargetIsUnoccupied)
                        })
                    }),

                new ApplyBehaviour(
                    name: EffectName.Camou.ClimbApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Camou.ClimbBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new CreateEntity(
                    name: EffectName.Shaman.WondrousGooCreateEntity,
                    entityToCreate: FeatureName.ShamanWondrousGoo,
                    initialEntityBehaviours: new List<BehaviourName>
                    {
                        BehaviourName.Shaman.WondrousGooFeatureWait
                    }),

                new Search(
                    name: EffectName.Shaman.WondrousGooSearch,
                    radius: 0,
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEveryAction,
                        Flag.Effect.Search.AppliedOnEnter
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Shaman.WondrousGooApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Shaman.WondrousGooApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shaman.WondrousGooBuff
                    }),

                new Destroy(
                    name: EffectName.Shaman.WondrousGooDestroy),

                new Damage(
                    name: EffectName.Shaman.WondrousGooDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 1)),

                new CreateEntity(
                    name: EffectName.Pyre.CargoCreateEntity,
                    entityToCreate: FeatureName.PyreCargo,
                    initialEntityBehaviours: new List<BehaviourName>
                    {
                        BehaviourName.Pyre.CargoTether,
                        BehaviourName.Pyre.CargoWallOfFlamesBuff
                    }),

                new CreateEntity(
                    name: EffectName.Pyre.WallOfFlamesCreateEntity,
                    entityToCreate: FeatureName.PyreFlames),

                new Destroy(
                    name: EffectName.Pyre.WallOfFlamesDestroy),

                new Damage(
                    name: EffectName.Pyre.WallOfFlamesDamage,
                    damageType: DamageType.Melee,
                    amount: new Amount(flat: 5)),

                new ApplyBehaviour(
                    name: EffectName.Pyre.PhantomMenaceApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Pyre.PhantomMenaceBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new Search(
                    name: EffectName.BigBadBull.UnleashTheRageSearch,
                    radius: 1,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.BigBadBull.UnleashTheRageDamage,
                        EffectName.BigBadBull.UnleashTheRageForce
                    },
                    location: Location.Point,
                    shape: Shape.Line),

                new Damage(
                    name: EffectName.BigBadBull.UnleashTheRageDamage,
                    damageType: DamageType.OverrideMelee),

                new Force(
                    name: EffectName.BigBadBull.UnleashTheRageForce,
                    @from: Location.Origin, 
                    amount: 1,
                    onCollisionEffects: new List<EffectName>
                    {
                        EffectName.BigBadBull.UnleashTheRageForceDamage
                    }),

                new Damage(
                    name: EffectName.BigBadBull.UnleashTheRageForceDamage,
                    damageType: DamageType.Melee,
                    amount: new Amount(flat: 5)),

                new CreateEntity(
                    name: EffectName.Mummy.SpawnRoachCreateEntity,
                    entityToCreate: UnitName.Roach),

                new ModifyAbility(
                    name: EffectName.Mummy.LeapOfHungerModifyAbility,
                    abilityToModify: AbilityName.Mummy.SpawnRoach,
                    modifiedAbility: AbilityName.Mummy.SpawnRoachModified),

                new ApplyBehaviour(
                    name: EffectName.Roach.DegradingCarapaceApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Roach.DegradingCarapaceBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new ApplyBehaviour(
                    name: EffectName.Roach.DegradingCarapacePeriodicApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Roach.DegradingCarapacePeriodicDamageBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new Damage(
                    name: EffectName.Roach.DegradingCarapaceSelfDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 1)),

                new Damage(
                    name: EffectName.Roach.CorrosiveSpitDamage,
                    damageType: DamageType.Ranged,
                    amount: new Amount(flat: 6),
                    bonusTo: CombatAttributes.Mechanical,
                    bonusAmount: new Amount(flat: 8),
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    }),

                new ApplyBehaviour(
                    name: EffectName.Parasite.ParalysingGraspApplyTetherBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Parasite.ParalysingGraspTether
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourDoesNotExist, 
                                conditionedBehaviour: BehaviourName.Parasite.ParalysingGraspTether)
                        })
                    }),

                new ApplyBehaviour(
                    name: EffectName.Parasite.ParalysingGraspApplyAttackBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Parasite.ParalysingGraspBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourDoesNotExist, 
                                conditionedBehaviour: BehaviourName.Parasite.ParalysingGraspBuff)
                        })
                    }),

                new ApplyBehaviour(
                    name: EffectName.Parasite.ParalysingGraspApplySelfBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Parasite.ParalysingGraspSelfBuff
                    },
                    location: Location.Source,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Unit
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourDoesNotExist, 
                                conditionedBehaviour: BehaviourName.Parasite.ParalysingGraspSelfBuff)
                        })
                    }),

                new Search(
                    name: EffectName.Horrior.ExpertFormationSearch,
                    radius: 1,
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.AppliedOnEveryAction,
                        Flag.Effect.Search.RemovedOnExit,
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Horrior
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Horrior.ExpertFormationApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Horrior.ExpertFormationApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Horrior.ExpertFormationBuff
                    }),

                new ApplyBehaviour(
                    name: EffectName.Horrior.MountApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Horrior.MountWait
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new CreateEntity(
                    name: EffectName.Horrior.MountCreateEntity,
                    entityToCreate: UnitName.Surfer),

                new Destroy(
                    name: EffectName.Horrior.MountDestroy,
                    target: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Marksman.CriticalMarkApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Marksman.CriticalMarkBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),
                
                new Damage(
                    name: EffectName.Marksman.CriticalMarkDamage,
                    damageType: DamageType.Melee,
                    amount: new Amount(flat: 5),
                    bonusTo: null,
                    bonusAmount: null,
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Surfer.DismountApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Surfer.DismountBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),
                
                new CreateEntity(
                    name: EffectName.Surfer.DismountCreateEntity,
                    entityToCreate: UnitName.Horrior),
                
                new ApplyBehaviour(
                    name: EffectName.Mortar.DeadlyAmmunitionApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Mortar.DeadlyAmmunitionAmmunition
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Search(
                    name: EffectName.Mortar.DeadlyAmmunitionSearch,
                    radius: 1,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Mortar.DeadlyAmmunitionDamage
                    },
                    location: Location.Inherited,
                    shape: Shape.Circle,
                    ignoreCenter: true),
                
                new Damage(
                    name: EffectName.Mortar.DeadlyAmmunitionDamage,
                    damageType: DamageType.OverrideRanged),
                
                new ApplyBehaviour(
                    name: EffectName.Mortar.ReloadApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Mortar.ReloadWait
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Reload(
                    name: EffectName.Mortar.ReloadReload,
                    ammunitionToTarget: BehaviourName.Mortar.DeadlyAmmunitionAmmunition,
                    location: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.Mortar.PiercingBlastApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Mortar.PiercingBlastBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Hawk.TacticalGogglesApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Hawk.TacticalGogglesBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Hawk.LeadershipApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Hawk.LeadershipBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.Attribute.Ranged
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Hawk.HealthKitApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Hawk.HealthKitBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Search(
                    name: EffectName.Hawk.HealthKitSearch,
                    radius: 1,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Hawk.HealthKitHealApplyBehaviour
                    },
                    location: Location.Self,
                    shape: Shape.Circle,
                    ignoreCenter: true),
                
                new ApplyBehaviour(
                    name: EffectName.Hawk.HealthKitHealApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Hawk.HealthKitHealBuff
                    }),

                new ApplyBehaviour(
                    name: EffectName.Engineer.OperateApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Engineer.OperateBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Cannon,
                        Flag.Filter.SpecificUnit.Ballista,
                        Flag.Filter.SpecificUnit.Radar,
                        Flag.Filter.SpecificUnit.Vessel
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourDoesNotExist, 
                                conditionedBehaviour: BehaviourName.Cannon.AssemblingBuildable)
                        }),
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourDoesNotExist, 
                                conditionedBehaviour: BehaviourName.Ballista.AssemblingBuildable)
                        }),
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourDoesNotExist, 
                                conditionedBehaviour: BehaviourName.Radar.AssemblingBuildable)
                        })
                    }),
                
                new ModifyCounter(
                    name: EffectName.Engineer.OperateModifyCounter,
                    countersToModify: new List<BehaviourName>
                    {
                        BehaviourName.Cannon.MachineCounter
                        // TODO add with each machine
                    },
                    change: Change.AddCurrent, 
                    amount: 1,
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    }),
                
                new Destroy(
                    name: EffectName.Engineer.OperateDestroy,
                    target: Location.Origin),
                
                new ApplyBehaviour(
                    name: EffectName.Engineer.RepairStructureApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Engineer.RepairStructureOrMachineBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.TargetDoesNotHaveFullHealth)
                        })
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Engineer.RepairMachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Engineer.RepairStructureOrMachineBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Cannon,
                        Flag.Filter.SpecificUnit.Ballista,
                        Flag.Filter.SpecificUnit.Radar,
                        Flag.Filter.SpecificUnit.Vessel
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.TargetDoesNotHaveFullHealth)
                        })
                    }),
                
                new ApplyBehaviour( 
                    name: EffectName.Engineer.RepairHorriorApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Engineer.RepairHorriorBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Horrior
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourExists, 
                                conditionedBehaviour: BehaviourName.Horrior.MountWait)
                        })
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Engineer.RepairApplyBehaviourSelf,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Engineer.RepairWait
                    },
                    location: Location.Origin),
                
                new ApplyBehaviour(
                    name: EffectName.Cannon.MachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Cannon.MachineCounter,
                        BehaviourName.Cannon.MachineBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new RemoveBehaviour(
                    name: EffectName.Cannon.MachineRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourName>
                    {
                        BehaviourName.Cannon.MachineBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new CreateEntity(
                    name: EffectName.Cannon.HeatUpCreateEntity,
                    entityToCreate: FeatureName.CannonHeatUpDangerZone,
                    initialEntityBehaviours: new List<BehaviourName>
                    {
                        BehaviourName.Cannon.HeatUpDangerZoneBuff
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Cannon.HeatUpApplyWaitBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Cannon.HeatUpWait
                    },
                    location: Location.Origin,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    }),
                
                new Search(
                    name: EffectName.Cannon.HeatUpSearch,
                    radius: 0,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Cannon.HeatUpDamage
                    },
                    location: Location.Self),
                
                new Damage(
                    name: EffectName.Cannon.HeatUpDamage,
                    damageType: DamageType.OverrideRanged),
                
                new Destroy(
                    name: EffectName.Cannon.HeatUpDestroy,
                    target: Location.Self),
                
                new RemoveBehaviour(
                    name: EffectName.Cannon.HeatUpRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourName>
                    {
                        BehaviourName.Cannon.HeatUpWait
                    },
                    location: Location.Origin,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Ballista.MachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Ballista.MachineCounter,
                        BehaviourName.Ballista.MachineBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new RemoveBehaviour(
                    name: EffectName.Ballista.MachineRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourName>
                    {
                        BehaviourName.Ballista.MachineBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Damage(
                    name: EffectName.Ballista.AimDamage,
                    damageType: DamageType.OverrideRanged,
                    amount: null,
                    bonusTo: null,
                    bonusAmount: null,
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    },
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourExists, 
                                conditionedBehaviour: BehaviourName.Ballista.AimBuff, 
                                behaviourOwner: Location.Origin)
                        })
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Ballista.AimApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Ballista.AimBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    },
                    behaviourOwner: Location.Origin),
                
                new Search(
                    name: EffectName.Ballista.AimSearch,
                    radius: 9,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Origin,
                        Flag.Filter.SpecificUnit.Ballista
                    },
                    effects: null,
                    location: Location.Self,
                    shape: null,
                    ignoreCenter: null,
                    validators: null,
                    usedForValidator: true),
                
                new ApplyBehaviour(
                    name: EffectName.Radar.MachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Radar.MachineCounter,
                        BehaviourName.Radar.MachineBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new RemoveBehaviour(
                    name: EffectName.Radar.MachineRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourName>
                    {
                        BehaviourName.Radar.MachineBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Radar.PowerDependencyApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Radar.PowerDependencyBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Self
                    }),
                
                new Damage(
                    name: EffectName.Radar.PowerDependencyDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 1)),
                
                new ApplyBehaviour(
                    name: EffectName.Radar.PowerDependencyApplyBehaviourDisable,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Radar.PowerDependencyBuffDisable
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Self
                    }),
                
                new CreateEntity(
                    name: EffectName.Radar.ResonatingSweepCreateEntity,
                    entityToCreate: FeatureName.RadarResonatingSweep,
                    initialEntityBehaviours: new List<BehaviourName>
                    {
                        BehaviourName.Radar.ResonatingSweepBuff
                    }),
                
                new Destroy(
                    name: EffectName.Radar.ResonatingSweepDestroy,
                    target: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.Radar.RadioLocationApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Radar.RadioLocationBuff
                    },
                    location: Location.Self),
                
                new Search(
                    name: EffectName.Radar.RadioLocationSearchDestroy,
                    radius: 1,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.SpecificFeature.RadarRedDot
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Radar.RadioLocationDestroy
                    },
                    location: Location.Self,
                    shape: Shape.Map),
                
                new Destroy(
                    name: EffectName.Radar.RadioLocationDestroy,
                    target: Location.Inherited,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new(conditionFlag: Flag.Condition.BehaviourExists, 
                                conditionedBehaviour: BehaviourName.Radar.RadioLocationFeatureBuff,
                                behaviourOwner: Location.Origin)
                        })
                    }),
                
                new Search(
                    name: EffectName.Radar.RadioLocationSearchCreate,
                    radius: 15,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Radar.RadioLocationCreateEntity
                    },
                    location: Location.Self,
                    shape: Shape.Circle,
                    ignoreCenter: true
                ),
                
                new CreateEntity(
                    name: EffectName.Radar.RadioLocationCreateEntity,
                    entityToCreate: FeatureName.RadarRedDot,
                    initialEntityBehaviours: new List<BehaviourName>
                    {
                        BehaviourName.Radar.RadioLocationFeatureBuff
                    },
                    behaviourOwner: Location.Origin),
                
                new ApplyBehaviour(
                    name: EffectName.Vessel.MachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Vessel.MachineCounter,
                        BehaviourName.Vessel.MachineBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new RemoveBehaviour(
                    name: EffectName.Vessel.MachineRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourName>
                    {
                        BehaviourName.Vessel.MachineBuff
                    },
                    location: Location.Self,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Self,
                        Flag.Filter.Unit
                    }),
                
                new Search(
                    name: EffectName.Vessel.AbsorbentFieldSearch,
                    radius: 3,
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Vessel.AbsorbentFieldApplyBehaviour
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.Vessel.AbsorbentFieldApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Vessel.AbsorbentFieldInterceptDamage
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    behaviourOwner: Location.Origin),

                new CreateEntity(
                    name: EffectName.Vessel.FortifyCreateEntity,
                    entityToCreate: FeatureName.VesselFortification,
                    initialEntityBehaviours: new List<BehaviourName>
                    {
                        BehaviourName.Vessel.FortifyDestroyBuff
                    }),
                
                new Destroy(
                    name: EffectName.Vessel.FortifyDestroy,
                    target: Location.Self),

                new Search(
                    name: EffectName.Vessel.FortifySearch,
                    radius: 0,
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Vessel.FortifyApplyBehaviour
                    },
                    location: Location.Self,
                    shape: Shape.Circle,
                    ignoreCenter: false),
                
                new ApplyBehaviour(
                    name: EffectName.Vessel.FortifyApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Vessel.FortifyBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Omen.RenditionPlacementApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Omen.RenditionPlacementBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    waitForInitialEffects: true),
                
                new ExecuteAbility(
                    name: EffectName.Omen.RenditionPlacementExecuteAbility,
                    abilityToExecute: AbilityName.Omen.RenditionPlacement,
                    executingPlayer: Location.Origin,
                    cancelSynchronised: true),
                
                new CreateEntity(
                    name: EffectName.Omen.RenditionPlacementCreateEntity,
                    entityToCreate: FeatureName.OmenRendition,
                    initialEntityBehaviours: new List<BehaviourName>
                    {
                        BehaviourName.Omen.RenditionInterceptDamage, 
                        BehaviourName.Omen.RenditionBuffDeath,
                        // Order is important, death check should happen first through the FinalEffect, because 
                        // the timer check disables any further Behaviours when it goes through the destroy
                        BehaviourName.Omen.RenditionBuffTimer
                    }),
                
                new Destroy(
                    name: EffectName.Omen.RenditionDestroy,
                    target: Location.Self,
                    validators: null,
                    blocksBehaviours: true),
                
                new Search(
                    name: EffectName.Omen.RenditionSearch,
                    radius: 1,
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Omen.RenditionDamage,
                        EffectName.Omen.RenditionApplyBehaviourSlow
                    },
                    location: Location.Source),
                
                new Damage(
                    name: EffectName.Omen.RenditionDamage,
                    damageType: DamageType.Melee,
                    amount: new Amount(flat: 10))
            };
        }
    }
}
