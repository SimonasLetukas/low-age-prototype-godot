using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Logic;
using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Filters;
using low_age_data.Domain.Common.Flags;
using low_age_data.Domain.Common.Modifications;
using low_age_data.Domain.Common.Shape;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Doodads;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Resources;
using low_age_data.Shared;
using low_age_prototype_common;

namespace low_age_data.Collections
{
    public static class EffectsCollection
    {
        public static List<Effect> Get()
        {
            return new List<Effect>
            {
                #region Shared

                new Search(
                    id: EffectId.Shared.HighGroundSearch,
                    shape: new Circle(radius: 0),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEnter,
                        SearchFlag.RemovedOnExit
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Shared.HighGroundApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Shared.HighGroundApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.HighGroundBuff
                    }),

                new ApplyBehaviour(
                    id: EffectId.Shared.PassiveIncomeApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.PassiveIncomeIncome
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Shared.ScrapsIncomeApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.ScrapsIncomeIncome
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Shared.CelestiumIncomeApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.CelestiumIncomeIncome
                    },
                    target: Location.Self),

                new Search(
                    id: EffectId.Shared.Revelators.NoPopulationSpaceSearch,
                    shape: new Map(),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Shared.Revelators.NoPopulationSpaceApplyBehaviour
                    },
                    location: Location.Inherited),

                new ApplyBehaviour(
                    id: EffectId.Shared.Revelators.NoPopulationSpaceApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.Revelators.NoPopulationSpaceInterceptDamage
                    }),
                
                new ApplyBehaviour(
                    id: EffectId.Shared.Uee.PowerGeneratorApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.Uee.PowerGeneratorBuff
                    },
                    target: Location.Self),

                new ModifyPlayer(
                    id: EffectId.Shared.Uee.PowerGeneratorModifyPlayer,
                    playerFilters: new List<IFilterItem>
                    {
                        new SpecificFlag(FilterFlag.Self)
                    },
                    modifyFlags: new List<ModifyPlayerFlag>
                    {
                        ModifyPlayerFlag.GameLost
                    }),
                
                new ApplyBehaviour(
                    id: EffectId.Shared.Uee.PowerDependencyApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.Uee.PowerDependencyBuff
                    },
                    target: Location.Self),

                new Damage(
                    id: EffectId.Shared.Uee.PowerDependencyDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 5),
                    ignoresShield: true),

                new ApplyBehaviour(
                    id: EffectId.Shared.Uee.PowerDependencyApplyBehaviourDisable,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.Uee.PowerDependencyBuffDisable
                    },
                    target: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Shared.Uee.PowerDependencyApplyBehaviourInactive,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.Uee.PowerDependencyBuffInactive
                    },
                    target: Location.Self),
                
                new Search(
                    id: EffectId.Shared.Uee.PositiveFaithSearch,
                    shape: new Map(),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Player),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Shared.Uee.PositiveFaithApplyBehaviour
                    },
                    location: Location.Inherited),
                
                new ApplyBehaviour(
                    id: EffectId.Shared.Uee.PositiveFaithApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shared.Uee.PositiveFaithBuff
                    },
                    target: Location.Actor),

                #endregion

                #region Structures

                new ApplyBehaviour(
                    id: EffectId.Citadel.ExecutiveStashApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Citadel.ExecutiveStashIncome
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Citadel.AscendableApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Citadel.AscendableAscendable
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Citadel.HighGroundApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Citadel.HighGroundHighGround
                    },
                    target: Location.Self),

                new Search(
                    id: EffectId.Obelisk.CelestiumDischargeSearchLong,
                    shape: new Circle(radius: 5, ignoreRadius: 1),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Obelisk.CelestiumDischargeApplyBehaviourLong
                    },
                    location: Location.Origin),

                new ApplyBehaviour(
                    id: EffectId.Obelisk.CelestiumDischargeApplyBehaviourLong,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Obelisk.CelestiumDischargeBuffLong
                    }),

                new ApplyBehaviour(
                    id: EffectId.Obelisk.CelestiumDischargeApplyBehaviourNegative,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Obelisk.CelestiumDischargeBuffNegative
                    },
                    target: Location.Self),

                new Search(
                    id: EffectId.Obelisk.CelestiumDischargeSearchShort,
                    shape: new Circle(radius: 1, ignoreRadius: 0),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Obelisk.CelestiumDischargeApplyBehaviourShort
                    },
                    location: Location.Origin),

                new ApplyBehaviour(
                    id: EffectId.Obelisk.CelestiumDischargeApplyBehaviourShort,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Obelisk.CelestiumDischargeBuffShort
                    }),

                new ApplyBehaviour(
                    id: EffectId.Shack.AccommodationApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shack.AccommodationIncome
                    }),

                new ApplyBehaviour(
                    id: EffectId.Smith.MeleeWeaponProductionApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Smith.MeleeWeaponProductionIncome
                    }),

                new ApplyBehaviour(
                    id: EffectId.Fletcher.RangedWeaponProductionApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Fletcher.RangedWeaponProductionIncome
                    }),

                new ApplyBehaviour(
                    id: EffectId.Alchemy.SpecialWeaponProductionApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Alchemy.SpecialWeaponProductionIncome
                    }),

                new ApplyBehaviour(
                    id: EffectId.Depot.WeaponStorageApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Depot.WeaponStorageIncome
                    }),
                
                new ApplyBehaviour(
                    id: EffectId.Outpost.AscendableApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Outpost.AscendableAscendable
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Outpost.HighGroundApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Outpost.HighGroundHighGround
                    },
                    target: Location.Self),
                
                new Search(
                    id: EffectId.Barricade.ProtectiveShieldSearch,
                    shape: new Custom(areas: new List<Area>
                    {
                        new Area(start: new Vector2<int>(x: -1,y: -1), 
                            size: new Vector2<int> (x: 3, y: 2))
                    }),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEnter,
                        SearchFlag.AppliedOnActionPhaseStart,
                        SearchFlag.RemovedOnExit
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Barricade.ProtectiveShieldApplyBehaviour
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Barricade.ProtectiveShieldApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Barricade.ProtectiveShieldBuff
                    }),
                
                new Search(
                    id: EffectId.Barricade.CaltropsSearch,
                    shape: new Custom(areas: new List<Area>
                    {
                        new Area(start: new Vector2<int>(x: 1,y: -1), 
                            size: new Vector2<int> (x: 3, y: 2))
                    }),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnActionPhaseStart,
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Barricade.CaltropsDamage
                    },
                    location: Location.Self),
                
                new Damage(
                    id: EffectId.Barricade.CaltropsDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 5)),
                
                new ApplyBehaviour(
                    id: EffectId.Barricade.DecomposeApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Barricade.DecomposeBuff
                    },
                    target: Location.Self),
                
                new RemoveBehaviour(
                    id: EffectId.Barricade.DecomposeRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourId>
                    {
                        BehaviourId.Barricade.DecomposeBuff
                    },
                    location: Location.Self),
                
                new Damage(
                    id: EffectId.Barricade.DecomposeDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 15),
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.BatteryCore.PowerGridApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.BatteryCore.PowerGridMaskProvider
                    },
                    target: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.BatteryCore.FusionCoreUpgradeApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.BatteryCore.FusionCoreUpgradeBuff
                    },
                    target: Location.Self),

                new CreateEntity(
                    id: EffectId.BatteryCore.FusionCoreUpgradeCreateEntity,
                    entityToCreate: StructureId.FusionCore),

                new Destroy(
                    id: EffectId.BatteryCore.FusionCoreUpgradeDestroy,
                    target: Location.Self,
                    blocksBehaviours: true),
                
                new ModifyResearch(
                    id: EffectId.BatteryCore.FusionCoreUpgradeModifyResearch,
                    researchToAdd: new List<ResearchId>
                    {
                        ResearchId.Uee.FusionCoreUpgrade
                    }),
                
                new ApplyBehaviour(
                    id: EffectId.FusionCore.PowerGridApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.FusionCore.PowerGridMaskProvider
                    },
                    target: Location.Self),
                
                new Damage(
                    id: EffectId.FusionCore.DefenceProtocolDamage,
                    damageType: DamageType.Ranged,
                    amount: new Amount(flat: 3),
                    location: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),

                new ApplyBehaviour(
                    id: EffectId.FusionCore.CelestiumCoreUpgradeApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.FusionCore.CelestiumCoreUpgradeBuff
                    },
                    target: Location.Self),

                new CreateEntity(
                    id: EffectId.FusionCore.CelestiumCoreUpgradeCreateEntity,
                    entityToCreate: StructureId.CelestiumCore),

                new Destroy(
                    id: EffectId.FusionCore.CelestiumCoreUpgradeDestroy,
                    target: Location.Self,
                    blocksBehaviours: true),
                
                new ModifyResearch(
                    id: EffectId.FusionCore.CelestiumCoreUpgradeModifyResearch,
                    researchToAdd: new List<ResearchId>
                    {
                        ResearchId.Uee.CelestiumCoreUpgrade
                    }),
                
                new ApplyBehaviour(
                    id: EffectId.CelestiumCore.PowerGridApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.CelestiumCore.PowerGridMaskProvider
                    },
                    target: Location.Self),
                
                new Damage(
                    id: EffectId.CelestiumCore.DefenceProtocolDamage,
                    damageType: DamageType.Ranged,
                    amount: new Amount(flat: 4),
                    location: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),
                
                new ModifyResearch(
                    id: EffectId.CelestiumCore.HeightenedConductivityModifyResearch,
                    researchToAdd: new List<ResearchId>
                    {
                        ResearchId.Uee.HeightenedConductivity
                    }),
                
                new ApplyBehaviour(
                    id: EffectId.Collector.DirectTransitSystemApplyBehaviourInactive,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Collector.DirectTransitSystemInactiveBuff
                    },
                    target: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Collector.DirectTransitSystemApplyBehaviourActive,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Collector.DirectTransitSystemActiveIncome
                    },
                    target: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Extractor.ReinforcedInfrastructureApplyBehaviourInactive,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Extractor.ReinforcedInfrastructureInactiveBuff
                    },
                    target: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Extractor.ReinforcedInfrastructureApplyBehaviourActive,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Extractor.ReinforcedInfrastructureActiveBuff
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.PowerPole.PowerGridApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.PowerPole.PowerGridMaskProvider
                    },
                    target: Location.Self),
                
                new Search(
                    id: EffectId.PowerPole.ExcessDistributionSearch,
                    shape: new Circle(radius: 4),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnPlanningPhaseStart
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Unit),
                                new SpecificFlag(value: FilterFlag.Structure)
                            }),
                        new SpecificFaction(value: FactionId.Uee)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.PowerPole.ExcessDistributionApplyBehaviour
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.PowerPole.ExcessDistributionApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.PowerPole.ExcessDistributionBuff
                    },
                    target: Location.Actor),
                
                new ModifyAbility(
                    id: EffectId.PowerPole.ImprovedPowerGridModifyAbilityPowerGrid,
                    abilityToModify: AbilityId.PowerPole.PowerGrid,
                    modifiedAbility: AbilityId.PowerPole.PowerGridImproved),
                
                new ModifyAbility(
                    id: EffectId.PowerPole.ImprovedPowerGridModifyAbilityExcessDistribution,
                    abilityToModify: AbilityId.PowerPole.ExcessDistribution,
                    modifiedAbility: AbilityId.PowerPole.ExcessDistributionImproved),
                
                new ApplyBehaviour(
                    id: EffectId.PowerPole.PowerGridImprovedApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.PowerPole.PowerGridImprovedMaskProvider
                    },
                    target: Location.Self),
                
                new Search(
                    id: EffectId.PowerPole.ExcessDistributionImprovedSearch,
                    shape: new Circle(radius: 6),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnPlanningPhaseStart
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Unit),
                                new SpecificFlag(value: FilterFlag.Structure)
                            }),
                        new SpecificFaction(value: FactionId.Uee)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.PowerPole.ExcessDistributionApplyBehaviour
                    },
                    location: Location.Self),
                
                new Search(
                    id: EffectId.Temple.KeepingTheFaithSearch,
                    shape: new Circle(radius: 6),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnActionPhaseStart
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Temple.KeepingTheFaithApplyBehaviourBuff
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Temple.KeepingTheFaithApplyBehaviourBuff,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Temple.KeepingTheFaithBuff
                    },
                    target: Location.Actor),
                
                new ApplyBehaviour(
                    id: EffectId.Temple.KeepingTheFaithApplyBehaviourIncome,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Temple.KeepingTheFaithIncome
                    },
                    target: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Wall.HighGroundApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Wall.HighGroundHighGround
                    },
                    target: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Stairs.AscendableApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Stairs.AscendableAscendable
                    },
                    target: Location.Self), 
                
                new ApplyBehaviour(
                    id: EffectId.Gate.HighGroundApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Gate.HighGroundHighGround
                    },
                    target: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Gate.AscendableApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Gate.AscendableAscendable
                    },
                    target: Location.Self), 
                
                new ApplyBehaviour(
                    id: EffectId.Gate.EntranceApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Gate.EntranceMovementBlock
                    },
                    target: Location.Self), 
                
                new ApplyBehaviour(
                    id: EffectId.Watchtower.VantagePointApplyBehaviourHighGround,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Watchtower.VantagePointHighGround
                    },
                    target: Location.Self),
                
                new Search(
                    id: EffectId.Watchtower.VantagePointSearch,
                    shape: new Circle(radius: 0),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEnter,
                        SearchFlag.RemovedOnExit
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Watchtower.VantagePointApplyBehaviourBuff
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Watchtower.VantagePointApplyBehaviourBuff,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Watchtower.VantagePointBuff
                    }), 
                
                new ApplyBehaviour(
                    id: EffectId.Bastion.BattlementApplyBehaviourHighGround,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Bastion.BattlementHighGround
                    },
                    target: Location.Self),
                
                new Search(
                    id: EffectId.Bastion.BattlementSearch,
                    shape: new Circle(radius: 0),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEnter,
                        SearchFlag.RemovedOnExit
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Bastion.BattlementApplyBehaviourBuff
                    },
                    location: Location.Self),
                
                new ApplyBehaviour(
                    id: EffectId.Bastion.BattlementApplyBehaviourBuff,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Bastion.BattlementBuff
                    }), 
                
                #endregion

                #region Units

                new ApplyBehaviour(
                    id: EffectId.Leader.AllForOneApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Leader.AllForOneBuff
                    },
                    target: Location.Self),

                new ModifyPlayer(
                    id: EffectId.Leader.AllForOneModifyPlayer,
                    playerFilters: new List<IFilterItem>
                    {
                        new SpecificFlag(FilterFlag.Self)
                    },
                    modifyFlags: new List<ModifyPlayerFlag>
                    {
                        ModifyPlayerFlag.GameLost
                    }),

                new Search(
                    id: EffectId.Leader.MenacingPresenceSearch,
                    shape: new Circle(radius: 6),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEnter,
                        SearchFlag.AppliedOnSourceAction,
                        SearchFlag.RemovedOnExit
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Leader.MenacingPresenceApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Leader.MenacingPresenceApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Leader.MenacingPresenceBuff
                    }),

                new ApplyBehaviour(
                    id: EffectId.Leader.OneForAllApplyBehaviourObelisk,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Leader.OneForAllObeliskBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificEntity(value: StructureId.Obelisk)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.DoesNotExist,
                                conditionedBehaviour: BehaviourId.Leader.OneForAllObeliskBuff)
                        })
                    }),

                new Search(
                    id: EffectId.Leader.OneForAllSearch,
                    shape: new Map(),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Leader.OneForAllApplyBehaviourHeal
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Leader.OneForAllApplyBehaviourHeal,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Leader.OneForAllHealBuff
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Slave.RepairApplyBehaviourStructure,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Slave.RepairStructureBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Structure)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetDoesNotHaveFullHealth)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Slave.RepairApplyBehaviourSelf,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Slave.RepairWait
                    },
                    target: Location.Origin),

                new ApplyBehaviour(
                    id: EffectId.Slave.ManualLabourApplyBehaviourHut,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Slave.ManualLabourBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificEntity(value: StructureId.Hut)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.DoesNotExist,
                                conditionedBehaviour: BehaviourId.Slave.ManualLabourBuff)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Slave.ManualLabourApplyBehaviourSelf,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Slave.ManualLabourWait
                    },
                    target: Location.Origin),

                new ModifyPlayer(
                    id: EffectId.Slave.ManualLabourModifyPlayer,
                    playerFilters: new List<IFilterItem>
                    {
                        new SpecificFlag(FilterFlag.Self)
                    },
                    modifyFlags: null,
                    resourceModifications: new List<ResourceModification>
                    {
                        new ResourceModification(
                            change: Change.AddCurrent,
                            amount: 2.0f,
                            resource: ResourceId.Scraps)
                    }),

                new ApplyBehaviour(
                    id: EffectId.Quickdraw.DoubleshotApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Quickdraw.DoubleshotExtraAttack
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Quickdraw.CrippleApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Quickdraw.CrippleBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),

                new ApplyBehaviour(
                    id: EffectId.Gorger.FanaticSuicideApplyBehaviourBuff,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Gorger.FanaticSuicideBuff
                    },
                    target: Location.Self),

                new Destroy(
                    id: EffectId.Gorger.FanaticSuicideDestroy,
                    target: Location.Origin),

                new Search(
                    id: EffectId.Gorger.FanaticSuicideSearch,
                    shape: new Circle(radius: 1),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy),
                            }),
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Unit),
                                new SpecificFlag(value: FilterFlag.Structure),
                            }),
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Gorger.FanaticSuicideDamage
                    },
                    location: Location.Self),

                new Damage(
                    id: EffectId.Gorger.FanaticSuicideDamage,
                    damageType: DamageType.OverrideMelee),

                new Damage(
                    id: EffectId.Camou.SilentAssassinOnHitDamage,
                    damageType: DamageType.CurrentMelee,
                    amount: new Amount(
                        flat: 0,
                        multiplier: 0.5f,
                        multiplierOf: AmountFlag.FromMissingHealth,
                        multiplierFilters: new List<IFilterItem>
                        {
                            new SpecificFlag(value: FilterFlag.Enemy),
                            new SpecificFlag(value: FilterFlag.Unit)
                        }),
                    bonusTo: null,
                    bonusAmount: null,
                    location: null,
                    filters: null,
                    validators: new List<Validator>
                    {
                        new ResultValidator(
                            searchEffect: EffectId.Camou.SilentAssassinSearchFriendly,
                            conditions: new List<Condition>
                            {
                                new Condition(conditionFlag: ConditionFlag.NoActorsFoundFromEffect)
                            })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Camou.SilentAssassinOnHitSilence,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Camou.SilentAssassinBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new ResultValidator(
                            searchEffect: EffectId.Camou.SilentAssassinSearchFriendly,
                            conditions: new List<Condition>
                            {
                                new Condition(conditionFlag: ConditionFlag.NoActorsFoundFromEffect)
                            }),

                        new ResultValidator(
                            searchEffect: EffectId.Camou.SilentAssassinSearchEnemy,
                            conditions: new List<Condition>
                            {
                                new Condition(conditionFlag: ConditionFlag.NoActorsFoundFromEffect)
                            })
                    }),

                new Search(
                    id: EffectId.Camou.SilentAssassinSearchFriendly,
                    shape: new Circle(radius: 4, ignoreRadius: 0),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    location: Location.Origin,
                    usedForValidator: true),

                new Search(
                    id: EffectId.Camou.SilentAssassinSearchEnemy,
                    shape: new Circle(radius: 4, ignoreRadius: 0),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    location: Location.Actor,
                    usedForValidator: true),

                new Teleport(
                    id: EffectId.Camou.ClimbTeleport,
                    waitBefore: BehaviourId.Camou.ClimbWait,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsHighGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Camou.ClimbApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Camou.ClimbBuff
                    },
                    target: Location.Self),

                new CreateEntity(
                    id: EffectId.Shaman.WondrousGooCreateEntity,
                    entityToCreate: DoodadId.ShamanWondrousGoo,
                    initialEntityBehaviours: new List<BehaviourId>
                    {
                        BehaviourId.Shaman.WondrousGooFeatureWait
                    }),

                new Search(
                    id: EffectId.Shaman.WondrousGooSearch,
                    shape: new Circle(radius: 0),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEveryAction,
                        SearchFlag.AppliedOnEnter
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Shaman.WondrousGooApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Shaman.WondrousGooApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Shaman.WondrousGooBuff
                    }),

                new Destroy(
                    id: EffectId.Shaman.WondrousGooDestroy),

                new Damage(
                    id: EffectId.Shaman.WondrousGooDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 1)),

                new CreateEntity(
                    id: EffectId.Pyre.CargoCreateEntity,
                    entityToCreate: DoodadId.PyreCargo,
                    initialEntityBehaviours: new List<BehaviourId>
                    {
                        BehaviourId.Pyre.CargoTether,
                        BehaviourId.Pyre.CargoWallOfFlamesBuff
                    }),

                new CreateEntity(
                    id: EffectId.Pyre.WallOfFlamesCreateEntity,
                    entityToCreate: DoodadId.PyreFlames),

                new Destroy(
                    id: EffectId.Pyre.WallOfFlamesDestroy),

                new Damage(
                    id: EffectId.Pyre.WallOfFlamesDamage,
                    damageType: DamageType.Melee,
                    amount: new Amount(flat: 5)),

                new ApplyBehaviour(
                    id: EffectId.Pyre.PhantomMenaceApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Pyre.PhantomMenaceBuff
                    },
                    target: Location.Self),

                new Search(
                    id: EffectId.BigBadBull.UnleashTheRageSearch,
                    shape: new Line(length: 1),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.BigBadBull.UnleashTheRageDamage,
                        EffectId.BigBadBull.UnleashTheRageForce
                    },
                    location: Location.Point),

                new Damage(
                    id: EffectId.BigBadBull.UnleashTheRageDamage,
                    damageType: DamageType.OverrideMelee),

                new Force(
                    id: EffectId.BigBadBull.UnleashTheRageForce,
                    @from: Location.Origin,
                    amount: 1,
                    onCollisionEffects: new List<EffectId>
                    {
                        EffectId.BigBadBull.UnleashTheRageForceDamage
                    }),

                new Damage(
                    id: EffectId.BigBadBull.UnleashTheRageForceDamage,
                    damageType: DamageType.Melee,
                    amount: new Amount(flat: 5)),

                new CreateEntity(
                    id: EffectId.Mummy.SpawnRoachCreateEntity,
                    entityToCreate: UnitId.Roach),

                new ModifyAbility(
                    id: EffectId.Mummy.LeapOfHungerModifyAbility,
                    abilityToModify: AbilityId.Mummy.SpawnRoach,
                    modifiedAbility: AbilityId.Mummy.SpawnRoachModified),

                new ApplyBehaviour(
                    id: EffectId.Roach.DegradingCarapaceApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Roach.DegradingCarapaceBuff
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Roach.DegradingCarapacePeriodicApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Roach.DegradingCarapacePeriodicDamageBuff
                    },
                    target: Location.Self),

                new Damage(
                    id: EffectId.Roach.DegradingCarapaceSelfDamage,
                    damageType: DamageType.Pure,
                    amount: new Amount(flat: 1)),

                new Damage(
                    id: EffectId.Roach.CorrosiveSpitDamage,
                    damageType: DamageType.Ranged,
                    amount: new Amount(flat: 6),
                    bonusTo: ActorAttribute.Mechanical,
                    bonusAmount: new Amount(flat: 8),
                    location: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Structure),
                                new SpecificFlag(value: FilterFlag.Unit),
                            }),
                        new SpecificFlag(value: FilterFlag.Enemy)
                    }),

                new ApplyBehaviour(
                    id: EffectId.Parasite.ParalysingGraspApplyTetherBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Parasite.ParalysingGraspTether
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.DoesNotExist,
                                conditionedBehaviour: BehaviourId.Parasite.ParalysingGraspTether)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Parasite.ParalysingGraspApplyAttackBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Parasite.ParalysingGraspBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.DoesNotExist,
                                conditionedBehaviour: BehaviourId.Parasite.ParalysingGraspBuff)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Parasite.ParalysingGraspApplySelfBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Parasite.ParalysingGraspSelfBuff
                    },
                    target: Location.Source,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.DoesNotExist,
                                conditionedBehaviour: BehaviourId.Parasite.ParalysingGraspSelfBuff)
                        })
                    }),

                new Search(
                    id: EffectId.Horrior.ExpertFormationSearch,
                    shape: new Circle(radius: 1),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEnter,
                        SearchFlag.AppliedOnEveryAction,
                        SearchFlag.RemovedOnExit,
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificEntity(value: UnitId.Horrior)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Horrior.ExpertFormationApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Horrior.ExpertFormationApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Horrior.ExpertFormationBuff
                    }),

                new ApplyBehaviour(
                    id: EffectId.Horrior.MountApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Horrior.MountWait
                    },
                    target: Location.Self),

                new CreateEntity(
                    id: EffectId.Horrior.MountCreateEntity,
                    entityToCreate: UnitId.Surfer),

                new Destroy(
                    id: EffectId.Horrior.MountDestroy,
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Marksman.CriticalMarkApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Marksman.CriticalMarkBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),

                new Damage(
                    id: EffectId.Marksman.CriticalMarkDamage,
                    damageType: DamageType.Melee,
                    amount: new Amount(flat: 5),
                    bonusTo: null,
                    bonusAmount: null,
                    location: Location.Self,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),

                new ApplyBehaviour(
                    id: EffectId.Surfer.DismountApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Surfer.DismountBuff
                    },
                    target: Location.Self),

                new CreateEntity(
                    id: EffectId.Surfer.DismountCreateEntity,
                    entityToCreate: UnitId.Horrior),

                new ApplyBehaviour(
                    id: EffectId.Mortar.DeadlyAmmunitionApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Mortar.DeadlyAmmunitionAmmunition
                    },
                    target: Location.Self),

                new Search(
                    id: EffectId.Mortar.DeadlyAmmunitionSearch,
                    shape: new Circle(radius: 1, ignoreRadius: 0),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Mortar.DeadlyAmmunitionDamage
                    },
                    location: Location.Inherited),

                new Damage(
                    id: EffectId.Mortar.DeadlyAmmunitionDamage,
                    damageType: DamageType.OverrideRanged),

                new ApplyBehaviour(
                    id: EffectId.Mortar.ReloadApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Mortar.ReloadWait
                    },
                    target: Location.Self),

                new Reload(
                    id: EffectId.Mortar.ReloadReload,
                    ammunitionToTarget: BehaviourId.Mortar.DeadlyAmmunitionAmmunition,
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Mortar.PiercingBlastApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Mortar.PiercingBlastBuff
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Hawk.TacticalGogglesApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Hawk.TacticalGogglesBuff
                    },
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Hawk.LeadershipApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Hawk.LeadershipBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit),
                        new SpecificCombatAttribute(ActorAttribute.Ranged)
                    }),

                new ApplyBehaviour(
                    id: EffectId.Hawk.HealthKitApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Hawk.HealthKitBuff
                    },
                    target: Location.Self),

                new Search(
                    id: EffectId.Hawk.HealthKitSearch,
                    shape: new Circle(radius: 1, ignoreRadius: 0),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Hawk.HealthKitHealApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Hawk.HealthKitHealApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Hawk.HealthKitHealBuff
                    }),

                new ApplyBehaviour(
                    id: EffectId.Engineer.OperateApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Engineer.OperateBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificEntity(value: UnitId.Cannon),
                                new SpecificEntity(value: UnitId.Ballista),
                                new SpecificEntity(value: UnitId.Radar),
                                new SpecificEntity(value: UnitId.Vessel),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.DoesNotExist,
                                conditionedBehaviour: BehaviourId.Cannon.AssemblingBuildable)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.DoesNotExist,
                                conditionedBehaviour: BehaviourId.Ballista.AssemblingBuildable)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.DoesNotExist,
                                conditionedBehaviour: BehaviourId.Radar.AssemblingBuildable)
                        })
                    }),

                new ModifyCounter(
                    id: EffectId.Engineer.OperateModifyCounter,
                    countersToModify: new List<BehaviourId>
                    {
                        BehaviourId.Cannon.MachineCounter,
                        BehaviourId.Ballista.MachineCounter,
                        BehaviourId.Radar.MachineCounter,
                        BehaviourId.Vessel.MachineCounter
                    },
                    change: Change.AddCurrent,
                    amount: 1,
                    location: Location.Self),

                new Destroy(
                    id: EffectId.Engineer.OperateDestroy,
                    target: Location.Origin),

                new ApplyBehaviour(
                    id: EffectId.Engineer.RepairStructureApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Engineer.RepairStructureOrMachineBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Structure)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetDoesNotHaveFullHealth)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Engineer.RepairMachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Engineer.RepairStructureOrMachineBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificEntity(value: UnitId.Cannon),
                                new SpecificEntity(value: UnitId.Ballista),
                                new SpecificEntity(value: UnitId.Radar),
                                new SpecificEntity(value: UnitId.Vessel),
                            }),
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetDoesNotHaveFullHealth)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Engineer.RepairHorriorApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Engineer.RepairHorriorBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificEntity(value: UnitId.Horrior)
                    },
                    behaviourOwner: null,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedBehaviour: BehaviourId.Horrior.MountWait)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Engineer.RepairApplyBehaviourSelf,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Engineer.RepairWait
                    },
                    target: Location.Origin),

                new ApplyBehaviour(
                    id: EffectId.Cannon.MachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Cannon.MachineCounter,
                        BehaviourId.Cannon.MachineBuff
                    },
                    target: Location.Self),

                new RemoveBehaviour(
                    id: EffectId.Cannon.MachineRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourId>
                    {
                        BehaviourId.Cannon.MachineBuff
                    },
                    location: Location.Self),

                new CreateEntity(
                    id: EffectId.Cannon.HeatUpCreateEntity,
                    entityToCreate: DoodadId.CannonHeatUpDangerZone,
                    initialEntityBehaviours: new List<BehaviourId>
                    {
                        BehaviourId.Cannon.HeatUpDangerZoneBuff
                    }),

                new ApplyBehaviour(
                    id: EffectId.Cannon.HeatUpApplyWaitBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Cannon.HeatUpWait
                    },
                    target: Location.Origin,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),

                new Search(
                    id: EffectId.Cannon.HeatUpSearch,
                    shape: new Circle(radius: 0),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                                new SpecificFlag(value: FilterFlag.Enemy)
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Cannon.HeatUpDamage
                    },
                    location: Location.Self),

                new Damage(
                    id: EffectId.Cannon.HeatUpDamage,
                    damageType: DamageType.OverrideRanged),

                new Destroy(
                    id: EffectId.Cannon.HeatUpDestroy,
                    target: Location.Self),

                new RemoveBehaviour(
                    id: EffectId.Cannon.HeatUpRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourId>
                    {
                        BehaviourId.Cannon.HeatUpWait
                    },
                    location: Location.Origin,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),

                new ApplyBehaviour(
                    id: EffectId.Ballista.MachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Ballista.MachineCounter,
                        BehaviourId.Ballista.MachineBuff
                    },
                    target: Location.Self),

                new RemoveBehaviour(
                    id: EffectId.Ballista.MachineRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourId>
                    {
                        BehaviourId.Ballista.MachineBuff
                    },
                    location: Location.Self),

                new Damage(
                    id: EffectId.Ballista.AimDamage,
                    damageType: DamageType.OverrideRanged,
                    amount: null,
                    bonusTo: null,
                    bonusAmount: null,
                    location: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Structure),
                                new SpecificFlag(value: FilterFlag.Unit)
                            }),
                        new SpecificFlag(value: FilterFlag.Enemy)
                    },
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedBehaviour: BehaviourId.Ballista.AimBuff,
                                behaviourOwner: Location.Origin)
                        })
                    }),

                new ApplyBehaviour(
                    id: EffectId.Ballista.AimApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Ballista.AimBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Structure),
                                new SpecificFlag(value: FilterFlag.Unit)
                            }),
                        new SpecificFlag(value: FilterFlag.Enemy)
                    },
                    behaviourOwner: Location.Origin),

                new Search(
                    id: EffectId.Ballista.AimSearch,
                    shape: new Circle(radius: 9),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(FilterFlag.Origin),
                        new SpecificEntity(UnitId.Ballista)
                    },
                    location: Location.Self,
                    usedForValidator: true),

                new ApplyBehaviour(
                    id: EffectId.Radar.MachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Radar.MachineCounter,
                        BehaviourId.Radar.MachineBuff
                    },
                    target: Location.Self),

                new RemoveBehaviour(
                    id: EffectId.Radar.MachineRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourId>
                    {
                        BehaviourId.Radar.MachineBuff
                    },
                    location: Location.Self),

                new CreateEntity(
                    id: EffectId.Radar.ResonatingSweepCreateEntity,
                    entityToCreate: DoodadId.RadarResonatingSweep,
                    initialEntityBehaviours: new List<BehaviourId>
                    {
                        BehaviourId.Radar.ResonatingSweepBuff
                    }),

                new Destroy(
                    id: EffectId.Radar.ResonatingSweepDestroy,
                    target: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Radar.RadioLocationApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Radar.RadioLocationBuff
                    },
                    target: Location.Self),

                new Search(
                    id: EffectId.Radar.RadioLocationSearchDestroy,
                    shape: new Map(),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new SpecificEntity(DoodadId.RadarRedDot)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Radar.RadioLocationDestroy
                    },
                    location: Location.Self),

                new Destroy(
                    id: EffectId.Radar.RadioLocationDestroy,
                    target: Location.Inherited,
                    validators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new BehaviourCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedBehaviour: BehaviourId.Radar.RadioLocationFeatureBuff,
                                behaviourOwner: Location.Origin)
                        })
                    }),

                new Search(
                    id: EffectId.Radar.RadioLocationSearchCreate,
                    shape: new Circle(radius: 15, ignoreRadius: 0),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Radar.RadioLocationCreateEntity
                    },
                    location: Location.Self),

                new CreateEntity(
                    id: EffectId.Radar.RadioLocationCreateEntity,
                    entityToCreate: DoodadId.RadarRedDot,
                    initialEntityBehaviours: new List<BehaviourId>
                    {
                        BehaviourId.Radar.RadioLocationFeatureBuff
                    },
                    behaviourOwner: Location.Origin),

                new ApplyBehaviour(
                    id: EffectId.Vessel.MachineApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Vessel.MachineCounter,
                        BehaviourId.Vessel.MachineBuff
                    },
                    target: Location.Self),

                new RemoveBehaviour(
                    id: EffectId.Vessel.MachineRemoveBehaviour,
                    behavioursToRemove: new List<BehaviourId>
                    {
                        BehaviourId.Vessel.MachineBuff
                    },
                    location: Location.Self),

                new Search(
                    id: EffectId.Vessel.AbsorbentFieldSearch,
                    shape: new Circle(radius: 3),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEnter,
                        SearchFlag.RemovedOnExit
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Vessel.AbsorbentFieldApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Vessel.AbsorbentFieldApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Vessel.AbsorbentFieldInterceptDamage
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    behaviourOwner: Location.Origin),

                new CreateEntity(
                    id: EffectId.Vessel.FortifyCreateEntity,
                    entityToCreate: DoodadId.VesselFortification,
                    initialEntityBehaviours: new List<BehaviourId>
                    {
                        BehaviourId.Vessel.FortifyDestroyBuff
                    }),

                new Destroy(
                    id: EffectId.Vessel.FortifyDestroy,
                    target: Location.Self),

                new Search(
                    id: EffectId.Vessel.FortifySearch,
                    shape: new Circle(radius: 0),
                    searchFlags: new List<SearchFlag>
                    {
                        SearchFlag.AppliedOnEnter,
                        SearchFlag.RemovedOnExit
                    },
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Vessel.FortifyApplyBehaviour
                    },
                    location: Location.Self),

                new ApplyBehaviour(
                    id: EffectId.Vessel.FortifyApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Vessel.FortifyBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),

                new ApplyBehaviour(
                    id: EffectId.Omen.RenditionPlacementApplyBehaviour,
                    behavioursToApply: new List<BehaviourId>
                    {
                        BehaviourId.Omen.RenditionPlacementBuff
                    },
                    target: Location.Actor,
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    waitForInitialEffects: true),

                new ExecuteAbility(
                    id: EffectId.Omen.RenditionPlacementExecuteAbility,
                    abilityToExecute: AbilityId.Omen.RenditionPlacement,
                    executingPlayer: Location.Origin,
                    cancelSynchronised: true),

                new CreateEntity(
                    id: EffectId.Omen.RenditionPlacementCreateEntity,
                    entityToCreate: DoodadId.OmenRendition,
                    initialEntityBehaviours: new List<BehaviourId>
                    {
                        BehaviourId.Omen.RenditionInterceptDamage,
                        BehaviourId.Omen.RenditionBuffDeath,
                        // Order is important, death check should happen first through the FinalEffect, because 
                        // the timer check disables any further Behaviours when it goes through the destroy
                        BehaviourId.Omen.RenditionBuffTimer
                    }),

                new Destroy(
                    id: EffectId.Omen.RenditionDestroy,
                    target: Location.Self,
                    validators: null,
                    blocksBehaviours: true),

                new Search(
                    id: EffectId.Omen.RenditionSearch,
                    shape: new Circle(radius: 1),
                    searchFlags: new List<SearchFlag>(),
                    filters: new List<IFilterItem>
                    {
                        new FilterGroup(
                            policy: Policy.Include, 
                            quantifier: Quantifier.Any, 
                            items: new List<IFilterItem>
                            {
                                new SpecificFlag(value: FilterFlag.Player),
                                new SpecificFlag(value: FilterFlag.Ally),
                            }),
                        new SpecificFlag(value: FilterFlag.Unit)
                    },
                    effects: new List<EffectId>
                    {
                        EffectId.Omen.RenditionDamage,
                        EffectId.Omen.RenditionApplyBehaviourSlow
                    },
                    location: Location.Source),

                new Damage(
                    id: EffectId.Omen.RenditionDamage,
                    damageType: DamageType.Melee,
                    amount: new Amount(flat: 10))

                #endregion
            };
        }
    }
}