using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;
using low_age_data.Domain.Shared.Modifications;
using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Features;
using low_age_data.Domain.Resources;
using low_age_data.Domain.Shared.Shape;

namespace low_age_data.Collections
{
    public static class Effects
    {
        public static List<Effect> Get()
        {
            return new List<Effect>
            {
                #region Shared

                new Search(
                    name: EffectName.Shared.HighGroundSearch,
                    shape: new Circle(radius: 0),
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Shared.HighGroundApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Shared.HighGroundApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.HighGroundBuff
                    }),

                new ApplyBehaviour(
                    name: EffectName.Shared.PassiveIncomeApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.PassiveIncomeIncome
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Shared.ScrapsIncomeApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.ScrapsIncomeIncome
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Shared.CelestiumIncomeApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.CelestiumIncomeIncome
                    },
                    location: Location.Self),

                new Search(
                    name: EffectName.Shared.Revelators.NoPopulationSpaceSearch,
                    shape: new Map(),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Shared.Revelators.NoPopulationSpaceApplyBehaviour
                    },
                    location: Location.Inherited),

                new ApplyBehaviour(
                    name: EffectName.Shared.Revelators.NoPopulationSpaceApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.Revelators.NoPopulationSpaceInterceptDamage
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Shared.Uee.PowerGeneratorApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.Uee.PowerGeneratorBuff
                    },
                    location: Location.Self),

                new ModifyPlayer(
                    name: EffectName.Shared.Uee.PowerGeneratorModifyPlayer,
                    playerFilterFlags: new List<Flag>
                    {
                        Flag.Filter.Self
                    },
                    modifyFlags: new List<Flag>
                    {
                        Flag.Effect.ModifyPlayer.GameLost
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Shared.Uee.PowerDependencyApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.Uee.PowerDependencyBuff
                    },
                    location: Location.Self),

                new Damage(
                    name: EffectName.Shared.Uee.PowerDependencyDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 5),
                    ignoresShield: true),

                new ApplyBehaviour(
                    name: EffectName.Shared.Uee.PowerDependencyApplyBehaviourDisable,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.Uee.PowerDependencyBuffDisable
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.Shared.Uee.PowerDependencyApplyBehaviourInactive,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shared.Uee.PowerDependencyBuffInactive
                    },
                    location: Location.Self),

                #endregion

                #region Structures

                new ApplyBehaviour(
                    name: EffectName.Citadel.ExecutiveStashApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Citadel.ExecutiveStashIncome
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Citadel.AscendableApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Citadel.AscendableAscendable
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Citadel.HighGroundApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Citadel.HighGroundHighGround
                    },
                    location: Location.Self),

                new Search(
                    name: EffectName.Obelisk.CelestiumDischargeSearchLong,
                    shape: new Circle(radius: 5, ignoreRadius: 1),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Obelisk.CelestiumDischargeApplyBehaviourLong
                    },
                    location: Location.Origin),

                new ApplyBehaviour(
                    name: EffectName.Obelisk.CelestiumDischargeApplyBehaviourLong,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Obelisk.CelestiumDischargeBuffLong
                    }),

                new ApplyBehaviour(
                    name: EffectName.Obelisk.CelestiumDischargeApplyBehaviourNegative,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Obelisk.CelestiumDischargeBuffNegative
                    },
                    location: Location.Self),

                new Search(
                    name: EffectName.Obelisk.CelestiumDischargeSearchShort,
                    shape: new Circle(radius: 1, ignoreRadius: 0),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Obelisk.CelestiumDischargeApplyBehaviourShort
                    },
                    location: Location.Origin),

                new ApplyBehaviour(
                    name: EffectName.Obelisk.CelestiumDischargeApplyBehaviourShort,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Obelisk.CelestiumDischargeBuffShort
                    }),

                new ApplyBehaviour(
                    name: EffectName.Shack.AccommodationApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Shack.AccommodationIncome
                    }),

                new ApplyBehaviour(
                    name: EffectName.Smith.MeleeWeaponProductionApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Smith.MeleeWeaponProductionIncome
                    }),

                new ApplyBehaviour(
                    name: EffectName.Fletcher.RangedWeaponProductionApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Fletcher.RangedWeaponProductionIncome
                    }),

                new ApplyBehaviour(
                    name: EffectName.Alchemy.SpecialWeaponProductionApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Alchemy.SpecialWeaponProductionIncome
                    }),

                new ApplyBehaviour(
                    name: EffectName.Depot.WeaponStorageApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Depot.WeaponStorageIncome
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Outpost.AscendableApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Outpost.AscendableAscendable
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Outpost.HighGroundApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Outpost.HighGroundHighGround
                    },
                    location: Location.Self),
                
                new Search(
                    name: EffectName.Barricade.ProtectiveShieldSearch,
                    shape: new Custom(areas: new List<Area>
                    {
                        new(start: new Vector2<int>(x: -1,y: -1), 
                            size: new Vector2<int> (x: 3, y: 2))
                    }),
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.AppliedOnActionPhaseStart,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Barricade.ProtectiveShieldApplyBehaviour
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.Barricade.ProtectiveShieldApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Barricade.ProtectiveShieldBuff
                    }),
                
                new Search(
                    name: EffectName.Barricade.CaltropsSearch,
                    shape: new Custom(areas: new List<Area>
                    {
                        new(start: new Vector2<int>(x: -1,y: 1), 
                            size: new Vector2<int> (x: 3, y: 2))
                    }),
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnActionPhaseStart,
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Barricade.CaltropsDamage
                    },
                    location: Location.Self),
                
                new Damage(
                    name: EffectName.Barricade.CaltropsDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 5)),
                
                new ApplyBehaviour(
                    name: EffectName.Barricade.DecomposeApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Barricade.DecomposeBuff
                    },
                    location: Location.Self),
                
                new RemoveBehaviour(
                    name: EffectName.Barricade.DecomposeRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourName>
                    {
                        BehaviourName.Barricade.DecomposeBuff
                    },
                    location: Location.Self),
                
                new Damage(
                    name: EffectName.Barricade.DecomposeDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 15),
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.BatteryCore.PowerGridApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.BatteryCore.PowerGridMaskProvider
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.BatteryCore.FusionCoreUpgradeApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.BatteryCore.FusionCoreUpgradeBuff
                    },
                    location: Location.Self),

                new CreateEntity(
                    name: EffectName.BatteryCore.FusionCoreUpgradeCreateEntity,
                    entityToCreate: StructureName.FusionCore),

                new Destroy(
                    name: EffectName.BatteryCore.FusionCoreUpgradeDestroy,
                    target: Location.Self,
                    blocksBehaviours: true),
                
                new ModifyResearch(
                    name: EffectName.BatteryCore.FusionCoreUpgradeModifyResearch,
                    researchToAdd: new List<ResearchName>
                    {
                        ResearchName.Uee.FusionCoreUpgrade
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.FusionCore.PowerGridApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.FusionCore.PowerGridMaskProvider
                    },
                    location: Location.Self),
                
                new Damage(
                    name: EffectName.FusionCore.DefenceProtocolDamage,
                    damageType: DamageType.Ranged,
                    amount: new Amount(flat: 3),
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),

                new ApplyBehaviour(
                    name: EffectName.FusionCore.CelestiumCoreUpgradeApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.FusionCore.CelestiumCoreUpgradeBuff
                    },
                    location: Location.Self),

                new CreateEntity(
                    name: EffectName.FusionCore.CelestiumCoreUpgradeCreateEntity,
                    entityToCreate: StructureName.CelestiumCore),

                new Destroy(
                    name: EffectName.FusionCore.CelestiumCoreUpgradeDestroy,
                    target: Location.Self,
                    blocksBehaviours: true),
                
                new ModifyResearch(
                    name: EffectName.FusionCore.CelestiumCoreUpgradeModifyResearch,
                    researchToAdd: new List<ResearchName>
                    {
                        ResearchName.Uee.CelestiumCoreUpgrade
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.CelestiumCore.PowerGridApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.CelestiumCore.PowerGridMaskProvider
                    },
                    location: Location.Self),
                
                new Damage(
                    name: EffectName.CelestiumCore.DefenceProtocolDamage,
                    damageType: DamageType.Ranged,
                    amount: new Amount(flat: 4),
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),
                
                new ModifyResearch(
                    name: EffectName.CelestiumCore.HeightenedConductivityModifyResearch,
                    researchToAdd: new List<ResearchName>
                    {
                        ResearchName.Uee.HeightenedConductivity
                    }),
                
                new ApplyBehaviour(
                    name: EffectName.Collector.DirectTransitSystemApplyBehaviourInactive,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Collector.DirectTransitSystemInactiveBuff
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.Collector.DirectTransitSystemApplyBehaviourActive,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Collector.DirectTransitSystemActiveIncome
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.Extractor.ReinforcedInfrastructureApplyBehaviourInactive,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Extractor.ReinforcedInfrastructureInactiveBuff
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    name: EffectName.Extractor.ReinforcedInfrastructureApplyBehaviourActive,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Extractor.ReinforcedInfrastructureActiveBuff
                    },
                    location: Location.Self),

                #endregion

                #region Units

                new ApplyBehaviour(
                    name: EffectName.Leader.AllForOneApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Leader.AllForOneBuff
                    },
                    location: Location.Self),

                new ModifyPlayer(
                    name: EffectName.Leader.AllForOneModifyPlayer,
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
                    shape: new Circle(radius: 6),
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.AppliedOnSourceAction,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
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
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Structure,
                        Flag.Filter.SpecificStructure.Obelisk
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.DoesNotExist,
                                conditionedBehaviour: BehaviourName.Leader.OneForAllObeliskBuff)
                        })
                    }),

                new Search(
                    name: EffectName.Leader.OneForAllSearch,
                    shape: new Map(),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Leader.OneForAllApplyBehaviourHeal
                    },
                    location: Location.Self),

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
                        Flag.Filter.Player,
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
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Structure,
                        Flag.Filter.SpecificStructure.Hut
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.DoesNotExist,
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
                            resource: ResourceName.Scraps)
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
                    shape: new Circle(radius: 1),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit,
                        Flag.Filter.Structure
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
                    shape: new Circle(radius: 4),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Unit,
                        Flag.Filter.Ally
                    },
                    location: Location.Origin,
                    usedForValidator: true),

                new Search(
                    name: EffectName.Camou.SilentAssassinSearchEnemy,
                    shape: new Circle(radius: 4),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.Enemy
                    },
                    location: Location.Actor,
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
                    shape: new Circle(radius: 0),
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEveryAction,
                        Flag.Effect.Search.AppliedOnEnter
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
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
                    shape: new Line(length: 1),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.BigBadBull.UnleashTheRageDamage,
                        EffectName.BigBadBull.UnleashTheRageForce
                    },
                    location: Location.Point),

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
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.DoesNotExist,
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
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.DoesNotExist,
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
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.DoesNotExist,
                                conditionedBehaviour: BehaviourName.Parasite.ParalysingGraspSelfBuff)
                        })
                    }),

                new Search(
                    name: EffectName.Horrior.ExpertFormationSearch,
                    shape: new Circle(radius: 1),
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.AppliedOnEveryAction,
                        Flag.Effect.Search.RemovedOnExit,
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
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
                    shape: new Circle(radius: 1, ignoreRadius: 0),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Mortar.DeadlyAmmunitionDamage
                    },
                    location: Location.Inherited),

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
                        Flag.Filter.Player,
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
                    shape: new Circle(radius: 1, ignoreRadius: 0),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Hawk.HealthKitHealApplyBehaviour
                    },
                    location: Location.Self),

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
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.DoesNotExist,
                                conditionedBehaviour: BehaviourName.Cannon.AssemblingBuildable)
                        }),
                        new(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.DoesNotExist,
                                conditionedBehaviour: BehaviourName.Ballista.AssemblingBuildable)
                        }),
                        new(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.DoesNotExist,
                                conditionedBehaviour: BehaviourName.Radar.AssemblingBuildable)
                        })
                    }),

                new ModifyCounter(
                    name: EffectName.Engineer.OperateModifyCounter,
                    countersToModify: new List<BehaviourName>
                    {
                        BehaviourName.Cannon.MachineCounter,
                        BehaviourName.Ballista.MachineCounter,
                        BehaviourName.Radar.MachineCounter,
                        BehaviourName.Vessel.MachineCounter
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
                        Flag.Filter.Player,
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
                        Flag.Filter.Player,
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
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Unit,
                        Flag.Filter.SpecificUnit.Horrior
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.Exists,
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
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    }),

                new Search(
                    name: EffectName.Cannon.HeatUpSearch,
                    shape: new Circle(radius: 0),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
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
                        Flag.Filter.Player,
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
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.Exists,
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
                    shape: new Circle(radius: 9),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Origin,
                        Flag.Filter.SpecificUnit.Ballista
                    },
                    location: Location.Self,
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
                    shape: new Map(),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.SpecificFeature.RadarRedDot
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Radar.RadioLocationDestroy
                    },
                    location: Location.Self),

                new Destroy(
                    name: EffectName.Radar.RadioLocationDestroy,
                    target: Location.Inherited,
                    validators: new List<Validator>
                    {
                        new(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: Flag.Condition.Behaviour.Exists,
                                conditionedBehaviour: BehaviourName.Radar.RadioLocationFeatureBuff,
                                behaviourOwner: Location.Origin)
                        })
                    }),

                new Search(
                    name: EffectName.Radar.RadioLocationSearchCreate,
                    shape: new Circle(radius: 15, ignoreRadius: 0),
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
                    location: Location.Self),

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
                    shape: new Circle(radius: 3),
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
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
                        Flag.Filter.Player,
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
                    shape: new Circle(radius: 0),
                    searchFlags: new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    effects: new List<EffectName>
                    {
                        EffectName.Vessel.FortifyApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    name: EffectName.Vessel.FortifyApplyBehaviour,
                    behavioursToApply: new List<BehaviourName>
                    {
                        BehaviourName.Vessel.FortifyBuff
                    },
                    location: Location.Actor,
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
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
                    shape: new Circle(radius: 1),
                    searchFlags: new List<Flag>(),
                    filterFlags: new List<Flag>
                    {
                        Flag.Filter.Player,
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

                #endregion
            };
        }
    }
}