using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Logic;
using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Common.Modifications;
using LowAgeData.Domain.Common.Shape;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Masks;
using LowAgeData.Domain.Resources;
using LowAgeData.Domain.Tiles;
using LowAgeCommon;

namespace LowAgeData.Collections
{
    public static class BehavioursCollection
    {
        public static List<Behaviour> Get()
        {
            return new List<Behaviour>
            {
                #region Shared

                new Buff(
                    id: BehaviourId.Shared.HighGroundBuff,
                    displayName: nameof(BehaviourId.Shared.HighGroundBuff).CamelCaseToWords(),
                    description: "Unit is on high ground and has +1 vision range and +1 Attack Distance for " +
                                 "ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new AttackModification(
                            change: Change.AddMax,
                            amount: 1,
                            attackType: Attacks.Ranged,
                            attribute: AttackAttribute.MaxDistance),
                        new StatModification(
                            change: Change.AddMax,
                            amount: 1,
                            statType: StatType.Vision)
                    },
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    restoreChangesOnEnd: true),

                new Income(
                    id: BehaviourId.Shared.PassiveIncomeIncome,
                    displayName: nameof(BehaviourId.Shared.PassiveIncomeIncome).CamelCaseToWords(),
                    description: "Provides 3 Scraps and 7 Celestium at the start of each planning phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 3,
                            resource: ResourceId.Scraps),
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 7,
                            resource: ResourceId.Celestium)
                    }),

                new Income(
                    id: BehaviourId.Shared.ScrapsIncomeIncome,
                    displayName: nameof(BehaviourId.Shared.ScrapsIncomeIncome).CamelCaseToWords(),
                    description: "At the start of each planning phase provides 5 Scraps.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 5,
                            resource: ResourceId.Scraps)
                    }),

                new Income(
                    id: BehaviourId.Shared.CelestiumIncomeIncome,
                    displayName: nameof(BehaviourId.Shared.CelestiumIncomeIncome).CamelCaseToWords(),
                    description: "At the start of each planning phase provides 5 Celestium (-2 for each subsequently " +
                                 "constructed Obelisk, total minimum of 1).",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 5,
                            resource: ResourceId.Celestium)
                    },
                    diminishingReturn: 2),
                
                new Buildable(
                    id: BehaviourId.Shared.UnitInProductionBuildable,
                    displayName: nameof(BehaviourId.Shared.UnitInProductionBuildable).CamelCaseToWords(),
                    description: "This unit should be placed in the allowed placement area.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                    }),
                
                new Buildable(
                    id: BehaviourId.Shared.Revelators.BuildingStructureBuildable,
                    displayName: nameof(BehaviourId.Shared.Revelators.BuildingStructureBuildable).CamelCaseToWords(),
                    description: "This building can only be placed on the low ground and can be built by multiple " +
                                 "Slaves simultaneously. The placement can include a maximum of 2 marsh tiles.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Marsh, amountOfTilesRequired: 3)
                        }),
                    }),

                new InterceptDamage(
                    id: BehaviourId.Shared.Revelators.NoPopulationSpaceInterceptDamage,
                    displayName: nameof(BehaviourId.Shared.Revelators.NoPopulationSpaceInterceptDamage).CamelCaseToWords(),
                    description: "This unit receives double damage from all sources, because there's not enough " +
                                 "population space.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.EndOf.This.Planning,
                    amountDealtInstead: new Amount(
                        flat: 0,
                        multiplier: 2)),
                
                new Buildable(
                    id: BehaviourId.Shared.Uee.BuildingStructureBuildable,
                    displayName: nameof(BehaviourId.Shared.Uee.BuildingStructureBuildable).CamelCaseToWords(),
                    description: "This building can only be placed on the low ground tiles with Power. Cannot be " +
                                 "placed on marsh tiles.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Marsh)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new MaskCondition(
                                conditionFlag: ConditionFlag.Exists, 
                                conditionedMask: MaskId.Power)
                        }),
                    }),
                
                new Buff(
                    id: BehaviourId.Shared.Uee.PowerGeneratorBuff,
                    displayName: nameof(BehaviourId.Shared.Uee.PowerGeneratorBuff).CamelCaseToWords(),
                    description: "UEE faction loses when this structure is destroyed.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Shared.Uee.PowerGeneratorModifyPlayer
                    }),
                
                new Buff(
                    id: BehaviourId.Shared.Uee.PowerDependencyBuff,
                    displayName: nameof(BehaviourId.Shared.Uee.PowerDependencyBuff).CamelCaseToWords(),
                    description: "If this unit is not connected to Power, all abilities get disabled and it loses 5 " +
                                 "Health at the start of its action or action phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Neutral,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityStartedActionPhase
                        }, validators: new List<Validator> 
                        {
                            new Validator(conditions: new List<Condition>
                            {
                                new MaskCondition(
                                    conditionFlag: ConditionFlag.DoesNotExist, 
                                    conditionedMask: MaskId.Power)
                            })
                        }), 
                        // OR
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityStartedAction
                        }, validators: new List<Validator> 
                        {
                            new Validator(conditions: new List<Condition>
                            {
                                new MaskCondition(
                                    conditionFlag: ConditionFlag.DoesNotExist, 
                                    conditionedMask: MaskId.Power)
                            })
                        })
                    },
                    removeOnConditionsMet: true, // this buff will get recreated after PowerDependencyBuffInactive ends
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Shared.Uee.PowerDependencyDamage,
                        EffectId.Shared.Uee.PowerDependencyApplyBehaviourDisable,
                        EffectId.Shared.Uee.PowerDependencyApplyBehaviourInactive
                    }),

                new Buff(
                    id: BehaviourId.Shared.Uee.PowerDependencyBuffDisable,
                    displayName: nameof(BehaviourId.Shared.Uee.PowerDependencyBuffDisable).CamelCaseToWords(),
                    description: "All abilities are disabled because Power is not supplied.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.AbilitiesDisabled
                    },
                    endsAt: EndsAt.StartOf.Next.ActionPhase,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityMaskChanged
                        }, validators: new List<Validator> 
                        {
                            new Validator(conditions: new List<Condition>
                            {
                                new MaskCondition(
                                    conditionFlag: ConditionFlag.Exists, 
                                    conditionedMask: MaskId.Power)
                            })
                        })
                    },
                    removeOnConditionsMet: true,
                    restoreChangesOnEnd: true),
                
                new Buff( // needed so that damage is not applied twice per action phase when there is no power
                    id: BehaviourId.Shared.Uee.PowerDependencyBuffInactive,
                    displayName: nameof(BehaviourId.Shared.Uee.PowerDependencyBuffInactive).CamelCaseToWords(),
                    description: "If this unit is not connected to Power, all abilities get disabled and it loses 5 " +
                                 "Health at the start of its action or action phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Shared.Uee.PowerDependencyApplyBehaviour
                    },
                    endsAt: EndsAt.StartOf.Next.Planning,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Neutral,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityMaskChanged
                        }, validators: new List<Validator> 
                        {
                            new Validator(conditions: new List<Condition>
                            {
                                new MaskCondition(
                                    conditionFlag: ConditionFlag.Exists, 
                                    conditionedMask: MaskId.Power)
                            })
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Shared.Uee.PowerDependencyApplyBehaviour
                    }),
                
                new Buff(
                    id: BehaviourId.Shared.Uee.PositiveFaithBuff,
                    displayName: nameof(BehaviourId.Shared.Uee.PositiveFaithBuff).CamelCaseToWords(),
                    description: "This unit has increased Initiative because of the positive Faith.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(change: Change.AddMax, amount: 1, statType: StatType.Initiative),
                    },
                    endsAt: EndsAt.StartOf.Next.Planning,
                    canStack: true,
                    canResetDuration: true,
                    alignment: Alignment.Positive,
                    restoreChangesOnEnd: true),
                
                #endregion

                #region Structures

                new Income(
                    id: BehaviourId.Citadel.ExecutiveStashIncome,
                    displayName: nameof(BehaviourId.Citadel.ExecutiveStashIncome).CamelCaseToWords(),
                    description: "Provides 4 Population and 4 spaces of storage for Weapons.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 4,
                            resource: ResourceId.Population),
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 4,
                            resource: ResourceId.Weapons)
                    }),

                new Ascendable(
                    id: BehaviourId.Citadel.AscendableAscendable,
                    displayName: nameof(BehaviourId.Citadel.AscendableAscendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    path: new List<HighGroundArea>
                    {
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 1, y: 1), 
                                size: new Vector2<int>(x: 1, y: 1)),
                            spriteOffset: new Vector2<int>(x: 0, y: 9)),
                    },
                    closingEnabled: false),

                new HighGround(
                    id: BehaviourId.Citadel.HighGroundHighGround,
                    displayName: nameof(BehaviourId.Citadel.HighGroundHighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    highGroundAreas: new List<HighGroundArea>
                    {
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 0, y: 0), 
                                size: new Vector2<int>(x: 2, y: 3)),
                            spriteOffset: new Vector2<int>(x: 0, y: 9)),
                    }),

                new Buff(
                    id: BehaviourId.Obelisk.CelestiumDischargeBuffLong,
                    displayName: nameof(BehaviourId.Obelisk.CelestiumDischargeBuffLong).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddCurrent,
                            amount: 5,
                            statType: StatType.Health)
                    },
                    endsAt: EndsAt.Instant,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Buff(
                    id: BehaviourId.Obelisk.CelestiumDischargeBuffShort,
                    displayName: nameof(BehaviourId.Obelisk.CelestiumDischargeBuffShort).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddCurrent,
                            amount: 15,
                            statType: StatType.Health)
                    },
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Obelisk.CelestiumDischargeApplyBehaviourNegative
                    },
                    endsAt: EndsAt.Instant,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Buff(
                    id: BehaviourId.Obelisk.CelestiumDischargeBuffNegative,
                    displayName: nameof(BehaviourId.Obelisk.CelestiumDischargeBuffNegative).CamelCaseToWords(),
                    description: "This unit has its vision, Melee and Range Armour all reduced by 3 for 3 actions.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.SubtractCurrent,
                            amount: 3,
                            statType: StatType.Vision),
                        new StatModification(
                            change: Change.SubtractCurrent,
                            amount: 3,
                            statType: StatType.MeleeArmour),
                        new StatModification(
                            change: Change.SubtractCurrent,
                            amount: 3,
                            statType: StatType.RangedArmour)
                    },
                    endsAt: EndsAt.EndOf.Third.Action,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative,
                    restoreChangesOnEnd: true),

                new Income(
                    id: BehaviourId.Shack.AccommodationIncome,
                    displayName: nameof(BehaviourId.Shack.AccommodationIncome).CamelCaseToWords(),
                    description: "Provides 2 Population.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 2,
                            resource: ResourceId.Population)
                    }),

                new Income(
                    id: BehaviourId.Smith.MeleeWeaponProductionIncome,
                    displayName: nameof(BehaviourId.Smith.MeleeWeaponProductionIncome).CamelCaseToWords(),
                    description: "Every 20 Celestium generates a Melee Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 1,
                            resource: ResourceId.MeleeWeapon)
                    },
                    cost: new List<Payment>
                    {
                        new Payment(resource: ResourceId.Celestium,
                            amount: 20)
                    },
                    waitForAvailableStorage: true),

                new Income(
                    id: BehaviourId.Fletcher.RangedWeaponProductionIncome,
                    displayName: nameof(BehaviourId.Fletcher.RangedWeaponProductionIncome).CamelCaseToWords(),
                    description: "Every 25 Celestium generates a Ranged Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 1,
                            resource: ResourceId.RangedWeapon)
                    },
                    cost: new List<Payment>
                    {
                        new Payment(resource: ResourceId.Celestium,
                            amount: 25)
                    },
                    waitForAvailableStorage: true),

                new Income(
                    id: BehaviourId.Alchemy.SpecialWeaponProductionIncome,
                    displayName: nameof(BehaviourId.Alchemy.SpecialWeaponProductionIncome).CamelCaseToWords(),
                    description: "Every 30 Celestium generates a Special Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 1,
                            resource: ResourceId.SpecialWeapon)
                    },
                    cost: new List<Payment>
                    {
                        new Payment(resource: ResourceId.Celestium,
                            amount: 30)
                    },
                    waitForAvailableStorage: true),

                new Income(
                    id: BehaviourId.Depot.WeaponStorageIncome,
                    displayName: nameof(BehaviourId.Depot.WeaponStorageIncome).CamelCaseToWords(),
                    description: "Provides 4 spaces of storage for Weapons.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 4,
                            resource: ResourceId.Weapons)
                    }),
                
                new Ascendable(
                    id: BehaviourId.Outpost.AscendableAscendable,
                    displayName: nameof(BehaviourId.Outpost.AscendableAscendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    path: new List<HighGroundArea>
                    {
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 0, y: 0), 
                                size: new Vector2<int>(x: 1, y: 1)),
                            spriteOffset: new Vector2<int>(x: 0, y: 12)),
                    },
                    closingEnabled: true),

                new HighGround(
                    id: BehaviourId.Outpost.HighGroundHighGround,
                    displayName: nameof(BehaviourId.Outpost.HighGroundHighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    highGroundAreas: new List<HighGroundArea>
                    {
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 0, y: 0), 
                                size: new Vector2<int>(x: 1, y: 1)),
                            spriteOffset: new Vector2<int>(x: 0, y: 12)),
                    }),
                
                new Buff(
                    id: BehaviourId.Barricade.ProtectiveShieldBuff,
                    displayName: nameof(BehaviourId.Barricade.ProtectiveShieldBuff).CamelCaseToWords(),
                    description: "Range Armour for this unit is increased by 2 because of a nearby Barricade's shield.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddMax,
                            amount: 2,
                            statType: StatType.RangedArmour)
                    },
                    endsAt: EndsAt.EndOf.This.Planning,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Positive,
                    removeOnConditionsMet: false,
                    restoreChangesOnEnd: true),
                
                new Buff(
                    id: BehaviourId.Barricade.DecomposeBuff,
                    displayName: nameof(BehaviourId.Barricade.DecomposeBuff).CamelCaseToWords(),
                    description: "This Barricade is decomposing and will receive 15 Pure Damage at the start of " +
                                 "each action phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Barricade.DecomposeDamage,
                        EffectId.Barricade.DecomposeApplyBehaviour
                    },
                    endsAt: EndsAt.StartOf.Next.ActionPhase,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative),

                new MaskProvider(
                    id: BehaviourId.BatteryCore.PowerGridMaskProvider,
                    displayName: nameof(BehaviourId.BatteryCore.PowerGridMaskProvider).CamelCaseToWords(),
                    description: "Provides Power in 4 Distance.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maskCreated: MaskId.Power, 
                    maskShape: new Circle(radius: 4, ignoreRadius: 0)),
                
                new Buildable(
                    id: BehaviourId.Collector.BuildingBuildable,
                    displayName: nameof(BehaviourId.Collector.BuildingBuildable).CamelCaseToWords(),
                    description: "This building can only be placed on the low ground Scraps tiles.", 
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedTile: TileId.Scraps,
                                amountOfTilesRequired: 2)
                        })
                    }),

                new Buildable(
                    id: BehaviourId.Extractor.BuildingBuildable,
                    displayName: nameof(BehaviourId.Extractor.BuildingBuildable).CamelCaseToWords(),
                    description: "This building can only be placed on the low ground Celestium tiles.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedTile: TileId.Celestium,
                                amountOfTilesRequired: 2)
                        })
                    }),

                new Buildable(
                    id: BehaviourId.Wall.BuildingBuildable,
                    displayName: nameof(BehaviourId.Wall.BuildingBuildable).CamelCaseToWords(),
                    description: "This building can only be placed on the low ground tiles with Power.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new MaskCondition(
                                conditionFlag: ConditionFlag.Exists, 
                                conditionedMask: MaskId.Power)
                        }),
                    },
                    canBeDragged: true),
                
                new Buff(
                    id: BehaviourId.BatteryCore.FusionCoreUpgradeBuff,
                    displayName: nameof(BehaviourId.BatteryCore.FusionCoreUpgradeBuff).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialEffects: new List<EffectId>
                    {
                        EffectId.BatteryCore.FusionCoreUpgradeCreateEntity,
                        EffectId.BatteryCore.FusionCoreUpgradeModifyResearch
                    },
                    finalEffects: new List<EffectId>
                    {
                        EffectId.BatteryCore.FusionCoreUpgradeDestroy
                    },
                    endsAt: EndsAt.Instant),
                
                new MaskProvider(
                    id: BehaviourId.FusionCore.PowerGridMaskProvider,
                    displayName: nameof(BehaviourId.FusionCore.PowerGridMaskProvider).CamelCaseToWords(),
                    description: "Provides Power in 6 Distance.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maskCreated: MaskId.Power, 
                    maskShape: new Circle(radius: 6, ignoreRadius: 0)),
                
                new Buff(
                    id: BehaviourId.FusionCore.CelestiumCoreUpgradeBuff,
                    displayName: nameof(BehaviourId.FusionCore.CelestiumCoreUpgradeBuff).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialEffects: new List<EffectId>
                    {
                        EffectId.FusionCore.CelestiumCoreUpgradeCreateEntity,
                        EffectId.FusionCore.CelestiumCoreUpgradeModifyResearch
                    },
                    finalEffects: new List<EffectId>
                    {
                        EffectId.FusionCore.CelestiumCoreUpgradeDestroy
                    },
                    endsAt: EndsAt.Instant),
                
                new MaskProvider(
                    id: BehaviourId.CelestiumCore.PowerGridMaskProvider,
                    displayName: nameof(BehaviourId.CelestiumCore.PowerGridMaskProvider).CamelCaseToWords(),
                    description: "Provides Power in 8 Distance.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maskCreated: MaskId.Power, 
                    maskShape: new Circle(radius: 8, ignoreRadius: 0)),
                
                new Buff(
                    id: BehaviourId.Collector.DirectTransitSystemInactiveBuff,
                    displayName: nameof(BehaviourId.Collector.DirectTransitSystemInactiveBuff).CamelCaseToWords(),
                    description: "Will provide +2 Scraps at the start of each planning phase when connected to Power.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Neutral,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityStartedActionPhase
                        }, validators: new List<Validator> 
                        {
                            new Validator(conditions: new List<Condition>
                            {
                                new MaskCondition(
                                    conditionFlag: ConditionFlag.Exists, 
                                    conditionedMask: MaskId.Power)
                            })
                        }), 
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Collector.DirectTransitSystemApplyBehaviourActive
                    },
                    restoreChangesOnEnd: false),
                
                new Income(
                    id: BehaviourId.Collector.DirectTransitSystemActiveIncome,
                    displayName: nameof(BehaviourId.Collector.DirectTransitSystemActiveIncome).CamelCaseToWords(),
                    description: "Provides +2 Scraps at the start of each planning phase because it's connected to " +
                                 "Power.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent,
                            amount: 2,
                            resource: ResourceId.Scraps),
                    },
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityStartedActionPhase
                        }, validators: new List<Validator> 
                        {
                            new Validator(conditions: new List<Condition>
                            {
                                new MaskCondition(
                                    conditionFlag: ConditionFlag.DoesNotExist, 
                                    conditionedMask: MaskId.Power)
                            })
                        }), 
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Collector.DirectTransitSystemApplyBehaviourInactive
                    }),
                
                new Buff(
                    id: BehaviourId.Extractor.ReinforcedInfrastructureInactiveBuff,
                    displayName: nameof(BehaviourId.Extractor.ReinforcedInfrastructureInactiveBuff).CamelCaseToWords(),
                    description: "Will gain additional 3 Melee Armour if connected to Power.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Neutral,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityStartedActionPhase
                        }, validators: new List<Validator> 
                        {
                            new Validator(conditions: new List<Condition>
                            {
                                new MaskCondition(
                                    conditionFlag: ConditionFlag.Exists, 
                                    conditionedMask: MaskId.Power)
                            })
                        }), 
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Extractor.ReinforcedInfrastructureApplyBehaviourActive
                    },
                    restoreChangesOnEnd: false),
                
                new Buff(
                    id: BehaviourId.Extractor.ReinforcedInfrastructureActiveBuff,
                    displayName: nameof(BehaviourId.Extractor.ReinforcedInfrastructureActiveBuff).CamelCaseToWords(),
                    description: "Gains additional 3 Melee Armour because it's connected to Power.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddMax,
                            amount: 3,
                            statType: StatType.MeleeArmour)
                    },
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityStartedActionPhase
                        }, validators: new List<Validator> 
                        {
                            new Validator(conditions: new List<Condition>
                            {
                                new MaskCondition(
                                    conditionFlag: ConditionFlag.DoesNotExist, 
                                    conditionedMask: MaskId.Power)
                            })
                        }), 
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Extractor.ReinforcedInfrastructureApplyBehaviourInactive
                    },
                    restoreChangesOnEnd: true),
                
                new MaskProvider(
                    id: BehaviourId.PowerPole.PowerGridMaskProvider,
                    displayName: nameof(BehaviourId.PowerPole.PowerGridMaskProvider).CamelCaseToWords(),
                    description: "Provides Power in 4 Distance.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maskCreated: MaskId.Power, 
                    maskShape: new Circle(radius: 4, ignoreRadius: 0)),
                
                new Buff(
                    id: BehaviourId.PowerPole.ExcessDistributionBuff,
                    displayName: nameof(BehaviourId.PowerPole.ExcessDistributionBuff).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddCurrent, 
                            amount: 1, 
                            statType: StatType.Shields)
                    },
                    endsAt: EndsAt.Instant,
                    canStack: false,
                    canResetDuration: false,
                    restoreChangesOnEnd: false),
                
                new MaskProvider(
                    id: BehaviourId.PowerPole.PowerGridImprovedMaskProvider,
                    displayName: nameof(BehaviourId.PowerPole.PowerGridImprovedMaskProvider).CamelCaseToWords(),
                    description: "Provides Power in 6 Distance.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maskCreated: MaskId.Power, 
                    maskShape: new Circle(radius: 6, ignoreRadius: 0)),
                
                new Buff(
                    id: BehaviourId.Temple.KeepingTheFaithBuff,
                    displayName: nameof(BehaviourId.Temple.KeepingTheFaithBuff).CamelCaseToWords(),
                    description: "This unit has +2 Movement because of a nearby Temple.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(change: Change.AddMax, amount: 2, statType: StatType.Movement)
                    },
                    endsAt: EndsAt.EndOf.This.ActionPhase,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Positive,
                    removeOnConditionsMet: true),
                
                new Income(
                    id: BehaviourId.Temple.KeepingTheFaithIncome,
                    displayName: nameof(BehaviourId.Temple.KeepingTheFaithIncome).CamelCaseToWords(),
                    description: "Provides 1 Faith. Each point of Faith increases the Initiative of all of this " +
                                 "player units.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    resources: new List<ResourceModification>
                    {
                        new ResourceModification(change: Change.AddCurrent, amount: 1, resource: ResourceId.Faith)
                    }),
                
                new HighGround(
                    id: BehaviourId.Wall.HighGroundHighGround,
                    displayName: nameof(BehaviourId.Wall.HighGroundHighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    highGroundAreas: new List<HighGroundArea>
                    {
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 0, y: 0), 
                                size: new Vector2<int>(x: 1, y: 1)),
                            spriteOffset: new Vector2<int>(x: 0, y: 16)),
                    }),
                
                new Ascendable(
                    id: BehaviourId.Stairs.AscendableAscendable,
                    displayName: nameof(BehaviourId.Stairs.AscendableAscendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    path: new List<HighGroundArea>
                    {
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 0, y: 0), 
                                size: new Vector2<int>(x: 1, y: 1)),
                            spriteOffset: new Vector2<int>(x: 0, y: 13)), 
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 1, y: 0), 
                                size: new Vector2<int>(x: 1, y: 1)),
                            spriteOffset: new Vector2<int>(x: 0, y: 6)),
                    },
                    closingEnabled: false),
                
                new HighGround(
                    id: BehaviourId.Gate.HighGroundHighGround,
                    displayName: nameof(BehaviourId.Gate.HighGroundHighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    highGroundAreas: new List<HighGroundArea>
                    {
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 0, y: 0), 
                                size: new Vector2<int>(x: 1, y: 4)),
                            spriteOffset: new Vector2<int>(x: 0, y: 16)),
                    }),
                
                new Ascendable(
                    id: BehaviourId.Gate.AscendableAscendable,
                    displayName: nameof(BehaviourId.Gate.AscendableAscendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    path: new List<HighGroundArea>
                    {
                        new HighGroundArea(area: new Area(
                                start: new Vector2<int>(x: 0, y: 1), 
                                size: new Vector2<int>(x: 1, y: 2)),
                            spriteOffset: new Vector2<int>(x: 0, y: 16)),
                    },
                    closingEnabled: true),
                
                new MovementBlock(
                    id: BehaviourId.Gate.EntranceMovementBlock,
                    displayName: nameof(BehaviourId.Gate.EntranceMovementBlock).CamelCaseToWords(),
                    description: "Allows movement through for friendly units and blocks it for enemy units.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    blockedAreas: new List<Area>
                    {
                        new Area(start: new Vector2<int>(x: 0, y: 1),
                            size: new Vector2<int>(x: 1, y: 2)),
                    },
                    filters: new List<IFilterItem>
                    {
                        new SpecificFlag(value: FilterFlag.Enemy),
                        new SpecificFlag(value: FilterFlag.Unit)
                    }),
                
                new HighGround(
                    id: BehaviourId.Watchtower.VantagePointHighGround,
                    displayName: nameof(BehaviourId.Watchtower.VantagePointHighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +2 vision range, " +
                                 "+1 Attack Distance and +1 Range Damage for their ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    highGroundAreas: new List<HighGroundArea>
                    {
                        new HighGroundArea(
                            area: new Area(size: new Vector2<int>(x: 1, y: 1)), 
                            spriteOffset: new Vector2<int>(x: 0, y: 22))
                    },
                    onCollisionEffects: new List<EffectId>
                    {
                        EffectId.Watchtower.VantagePointSearch
                    }),
                
                new Buff(
                    id: BehaviourId.Watchtower.VantagePointBuff,
                    displayName: nameof(BehaviourId.Watchtower.VantagePointBuff).CamelCaseToWords(),
                    description: "Unit is on a Watchtower and has +2 vision range, +1 Attack Distance and +1 Range " +
                                 "Damage for ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new AttackModification(
                            change: Change.AddMax,
                            amount: 1,
                            attackType: Attacks.Ranged,
                            attribute: AttackAttribute.MaxAmount),
                        new AttackModification(
                            change: Change.AddMax,
                            amount: 1,
                            attackType: Attacks.Ranged,
                            attribute: AttackAttribute.MaxDistance),
                        new StatModification(
                            change: Change.AddMax,
                            amount: 2,
                            statType: StatType.Vision)
                    },
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    restoreChangesOnEnd: true),
                
                new HighGround(
                    id: BehaviourId.Bastion.BattlementHighGround,
                    displayName: nameof(BehaviourId.Bastion.BattlementHighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 Range Armour, " +
                                 "+1 vision range and +1 Attack Distance for their ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    highGroundAreas: new List<HighGroundArea>
                    {
                        new HighGroundArea(
                            area: new Area(size: new Vector2<int>(x: 2, y: 2)), 
                            spriteOffset: new Vector2<int>(x: 0, y: 15))
                    },
                    onCollisionEffects: new List<EffectId>
                    {
                        EffectId.Bastion.BattlementSearch
                    }),
                
                new Buff(
                    id: BehaviourId.Bastion.BattlementBuff,
                    displayName: nameof(BehaviourId.Bastion.BattlementBuff).CamelCaseToWords(),
                    description: "Unit is on a Bastion and has +1 Range Armour in addition to the default high " +
                                 "ground bonus: +1 vision range and +1 Attack Distance for the ranged attacks.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddMax,
                            amount: 1,
                            statType: StatType.RangedArmour),
                        new AttackModification(
                            change: Change.AddMax,
                            amount: 1,
                            attackType: Attacks.Ranged,
                            attribute: AttackAttribute.MaxAmount),
                        new StatModification(
                            change: Change.AddMax,
                            amount: 1,
                            statType: StatType.Vision)
                    },
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    restoreChangesOnEnd: true),

                #endregion

                #region Units

                new Buff(
                    id: BehaviourId.Leader.AllForOneBuff,
                    displayName: nameof(BehaviourId.Leader.AllForOneBuff).CamelCaseToWords(),
                    description: "Revelators faction loses when this unit dies.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Leader.AllForOneModifyPlayer
                    }),

                new Buff(
                    id: BehaviourId.Leader.MenacingPresenceBuff,
                    displayName: nameof(BehaviourId.Leader.MenacingPresenceBuff).CamelCaseToWords(),
                    description: "Melee and Range Damage for this unit is reduced by 2.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    initialModifications: new List<Modification>
                    {
                        new AttackModification(
                            change: Change.SubtractMax,
                            amount: 2,
                            attackType: Attacks.Melee,
                            attribute: AttackAttribute.MaxAmount),
                        new AttackModification(
                            change: Change.SubtractMax,
                            amount: 2,
                            attackType: Attacks.Ranged,
                            attribute: AttackAttribute.MaxAmount)
                    },
                    endsAt: EndsAt.EndOf.This.Action,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative,
                    removeOnConditionsMet: false,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Leader.OneForAllObeliskBuff,
                    displayName: nameof(BehaviourId.Leader.OneForAllObeliskBuff).CamelCaseToWords(),
                    description: "This unit has recently been sapped for health.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Leader.OneForAllSearch
                    },
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.StartOf.Tenth.Planning,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Negative),

                new Buff(
                    id: BehaviourId.Leader.OneForAllHealBuff,
                    displayName: nameof(BehaviourId.Leader.OneForAllHealBuff).CamelCaseToWords(),
                    description: "Heals for 2 Health.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddCurrent,
                            amount: 2,
                            statType: StatType.Health)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Instant,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Buildable(
                    id: BehaviourId.Hut.BuildingBuildable,
                    displayName: nameof(BehaviourId.Hut.BuildingBuildable).CamelCaseToWords(),
                    description: "This building can only be placed on the low ground Scraps tiles, and can be built " +
                                 "by multiple Slaves simultaneously.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedTile: TileId.Scraps,
                                amountOfTilesRequired: 2)
                        })
                    }),

                new Buildable(
                    id: BehaviourId.Obelisk.BuildingBuildable,
                    displayName: nameof(BehaviourId.Obelisk.BuildingBuildable).CamelCaseToWords(),
                    description: "This building can only be placed on the low ground Celestium tiles, and can be " +
                                 "built by multiple Slaves simultaneously.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedTile: TileId.Celestium,
                                amountOfTilesRequired: 2)
                        })
                    }),

                new Buff(
                    id: BehaviourId.Slave.RepairStructureBuff,
                    displayName: nameof(BehaviourId.Slave.RepairStructureBuff).CamelCaseToWords(),
                    description: "This structure will be repaired by +1 Health at the start of the planning phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Slave.RepairApplyBehaviourSelf
                    },
                    finalModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddCurrent,
                            amount: 1,
                            statType: StatType.Health)
                    },
                    finalEffects: null,
                    endsAt: EndsAt.StartOf.Next.Planning,
                    canStack: true,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsInterrupted
                        })
                    },
                    removeOnConditionsMet: true),

                new Wait(
                    id: BehaviourId.Slave.RepairWait,
                    displayName: nameof(BehaviourId.Slave.RepairWait).CamelCaseToWords(),
                    description: "Currently repairing a structure.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.StartOf.Next.Planning),

                new Buff(
                    id: BehaviourId.Slave.ManualLabourBuff,
                    displayName: nameof(BehaviourId.Slave.ManualLabourBuff).CamelCaseToWords(),
                    description:
                    "Slave is working on this Hut to provide +2 Scraps at the start of the planning phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Slave.ManualLabourApplyBehaviourSelf
                    },
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Slave.ManualLabourModifyPlayer
                    },
                    endsAt: EndsAt.StartOf.Next.Planning,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    removeOnConditionsMet: true),

                new Wait(
                    id: BehaviourId.Slave.ManualLabourWait,
                    displayName: nameof(BehaviourId.Slave.ManualLabourWait).CamelCaseToWords(),
                    description: "Currently working on a nearby Hut.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.StartOf.Next.Planning),

                new ExtraAttack(
                    id: BehaviourId.Quickdraw.DoubleshotExtraAttack,
                    displayName: nameof(BehaviourId.Quickdraw.DoubleshotExtraAttack).CamelCaseToWords(),
                    description: "Ranged attacks twice.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    attackTypes: new List<Attacks>
                    {
                        Attacks.Ranged
                    }),

                new Buff(
                    id: BehaviourId.Quickdraw.CrippleBuff,
                    displayName: nameof(BehaviourId.Quickdraw.CrippleBuff).CamelCaseToWords(),
                    description:
                    "This unit has only 60% of their maximum Movement (rounded up) and cannot receive healing " +
                    "from any sources until the end of its action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.CannotBeHealed
                    },
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.MultiplyMax,
                            amount: 0.6f,
                            statType: StatType.Movement)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.EndOf.This.Action,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative,
                    triggers: null,
                    removeOnConditionsMet: false,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Gorger.FanaticSuicideBuff,
                    displayName: nameof(BehaviourId.Gorger.FanaticSuicideBuff).CamelCaseToWords(),
                    description:
                    "Upon getting killed or executing a melee attack Gorger explodes dealing its Melee Damage " +
                    "to all friendly and enemy units in 1 Distance.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Gorger.FanaticSuicideSearch
                    }),

                new Buff(
                    id: BehaviourId.Camou.SilentAssassinBuff,
                    displayName: nameof(BehaviourId.Camou.SilentAssassinBuff).CamelCaseToWords(),
                    description: "This unit is silenced (use of any abilities is disabled).",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.AbilitiesDisabled
                    },
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.EndOf.Second.Action,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative,
                    triggers: null,
                    removeOnConditionsMet: false,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Wait(
                    id: BehaviourId.Camou.ClimbWait,
                    displayName: nameof(BehaviourId.Camou.ClimbWait).CamelCaseToWords(),
                    description:
                    "Camou will complete climbing on an adjacent high ground space at the end of this action phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.EndOf.This.ActionPhase),

                new Buff(
                    id: BehaviourId.Camou.ClimbBuff,
                    displayName: nameof(BehaviourId.Camou.ClimbBuff).CamelCaseToWords(),
                    description: "Camou can move down from high ground at the additional cost of 1 Movement.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.ClimbsDown
                    },
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Wait(
                    id: BehaviourId.Shaman.WondrousGooFeatureWait,
                    displayName: nameof(BehaviourId.Shaman.WondrousGooFeatureWait).CamelCaseToWords(),
                    description: "Effect area will expand at the end of this action phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.EndOf.This.ActionPhase,
                    nextBehaviour: BehaviourId.Shaman.WondrousGooFeatureBuff),

                new Buff(
                    id: BehaviourId.Shaman.WondrousGooFeatureBuff,
                    displayName: nameof(BehaviourId.Shaman.WondrousGooFeatureBuff).CamelCaseToWords(),
                    description: "Effect area will disappear at the end of this action phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new SizeModification(
                            change: Change.SetMax,
                            amount: 3)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Shaman.WondrousGooDestroy
                    },
                    endsAt: EndsAt.EndOf.This.ActionPhase),

                new Buff(
                    id: BehaviourId.Shaman.WondrousGooBuff,
                    displayName: nameof(BehaviourId.Shaman.WondrousGooBuff).CamelCaseToWords(),
                    description: "Unit has its vision and Attack Distance reduced by 3 (total minimum of 1) " +
                                 "and receives 1 Pure Damage at the start of its turn, at which point the effect ends.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new AttackModification(
                            change: Change.SubtractMax,
                            amount: 3,
                            attackType: Attacks.Melee,
                            attribute: AttackAttribute.MaxDistance),
                        new AttackModification(
                            change: Change.SubtractMax,
                            amount: 3,
                            attackType: Attacks.Ranged,
                            attribute: AttackAttribute.MaxDistance)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Shaman.WondrousGooDamage
                    },
                    endsAt: EndsAt.StartOf.Next.Action,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Negative,
                    triggers: null,
                    removeOnConditionsMet: false,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Tether(
                    id: BehaviourId.Pyre.CargoTether,
                    displayName: nameof(BehaviourId.Pyre.CargoTether).CamelCaseToWords(),
                    description: "The cargo follows Pyre.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    source: Location.Origin,
                    extendsSelection: true,
                    sharedDamage: true,
                    maximumLeashRange: 1,
                    calculatedForSourcePathfinding: true),

                new Buff(
                    id: BehaviourId.Pyre.CargoWallOfFlamesBuff,
                    displayName: nameof(BehaviourId.Pyre.CargoWallOfFlamesBuff).CamelCaseToWords(),
                    description:
                    "The cargo leaves a path of flames when moved, which stay until the start of the next " +
                    "Pyre's action or until death. Any unit which starts its turn or moves onto the flames receives " +
                    "5 Melee Damage.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: null,
                    canStack: false,
                    canResetDuration: false,
                    alignment: null,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityIsAboutToMove
                        })
                    },
                    removeOnConditionsMet: false,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Pyre.WallOfFlamesCreateEntity
                    }),

                new Buff(
                    id: BehaviourId.Pyre.WallOfFlamesBuff,
                    displayName: nameof(BehaviourId.Pyre.WallOfFlamesBuff).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Pyre.WallOfFlamesDestroy
                    },
                    endsAt: EndsAt.StartOf.Next.Action,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Negative,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Pyre.WallOfFlamesDestroy
                    }),

                new Buff(
                    id: BehaviourId.Pyre.PhantomMenaceBuff,
                    displayName: nameof(BehaviourId.Pyre.PhantomMenaceBuff).CamelCaseToWords(),
                    description: "Pyre can move through enemy units (but not buildings).",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.MovesThroughEnemyUnits
                    }),

                new Buff(
                    id: BehaviourId.Roach.DegradingCarapaceBuff,
                    displayName: nameof(BehaviourId.Roach.DegradingCarapaceBuff).CamelCaseToWords(),
                    description: "Increases periodic damage by 1 at the start of each action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Roach.DegradingCarapacePeriodicApplyBehaviour,
                        EffectId.Roach.DegradingCarapaceApplyBehaviour
                    },
                    endsAt: EndsAt.StartOf.Next.Action,
                    canStack: true,
                    canResetDuration: false,
                    alignment: Alignment.Negative),

                new Buff(
                    id: BehaviourId.Roach.DegradingCarapacePeriodicDamageBuff,
                    displayName: nameof(BehaviourId.Roach.DegradingCarapacePeriodicDamageBuff).CamelCaseToWords(),
                    description: "This unit will continue to receive 1 damage at every start of its action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Roach.DegradingCarapaceSelfDamage
                    },
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Roach.DegradingCarapacePeriodicApplyBehaviour
                    },
                    endsAt: EndsAt.StartOf.Next.Action,
                    canStack: true,
                    canResetDuration: false,
                    alignment: Alignment.Negative),

                new Tether(
                    id: BehaviourId.Parasite.ParalysingGraspTether,
                    displayName: nameof(BehaviourId.Parasite.ParalysingGraspTether).CamelCaseToWords(),
                    description:
                    "This unit is possessed by Parasite. On Parasite turn, it moves both units using the movement " +
                    "speed that the possessed unit has. Any damage received is shared between both.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    source: Location.Inherited,
                    extendsSelection: false,
                    sharedDamage: true,
                    maximumLeashRange: 0,
                    calculatedForSourcePathfinding: true),

                new Buff(
                    id: BehaviourId.Parasite.ParalysingGraspBuff,
                    displayName: nameof(BehaviourId.Parasite.ParalysingGraspBuff).CamelCaseToWords(),
                    description:
                    "This unit is possessed by Parasite. On its turn, the possessed unit controls the attack which it " +
                    "must do unless there are no legal targets.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.CanAttackAnyTeam,
                        ModificationFlag.MovementDisabled,
                        ModificationFlag.MustExecuteAttack
                    },
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Parasite.ParalysingGraspApplySelfBehaviour
                    },
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: null,
                    canStack: null,
                    canResetDuration: null,
                    alignment: Alignment.Negative,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Parasite.ParalysingGraspSelfBuff,
                    displayName: nameof(BehaviourId.Parasite.ParalysingGraspSelfBuff).CamelCaseToWords(),
                    description:
                    "Parasite has possessed the unit on top, gaining its movement speed and the ability to move both " +
                    "units on Parasite's turn.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.AbilitiesDisabled,
                        ModificationFlag.CannotAttack
                    },
                    initialModifications: new List<Modification>
                    {
                        new StatCopyModification(
                            change: Change.SetMax,
                            copyFrom: Location.Source,
                            additionalAmount: 0,
                            statType: StatType.Movement)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: null,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.SourceIsDestroyed
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Horrior.ExpertFormationBuff,
                    displayName: nameof(BehaviourId.Horrior.ExpertFormationBuff).CamelCaseToWords(),
                    description:
                    "Range Armour for this unit is increased by 2 because it is in formation with an adjacent Horrior.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddMax,
                            amount: 2,
                            statType: StatType.RangedArmour)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.EndOf.This.ActionPhase,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Positive,
                    triggers: null,
                    removeOnConditionsMet: false,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Wait(
                    id: BehaviourId.Horrior.MountWait,
                    displayName: nameof(BehaviourId.Horrior.MountWait).CamelCaseToWords(),
                    description: "This unit is mounting up to transform into Surfer.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.StartOf.Fourth.Planning,
                    nextBehaviour: BehaviourId.Horrior.MountBuff),

                new Buff(
                    id: BehaviourId.Horrior.MountBuff,
                    displayName: nameof(BehaviourId.Horrior.MountBuff).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Horrior.MountCreateEntity
                    },
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Horrior.MountDestroy
                    },
                    endsAt: EndsAt.Instant),

                new Buff(
                    id: BehaviourId.Marksman.CriticalMarkBuff,
                    displayName: nameof(BehaviourId.Marksman.CriticalMarkBuff).CamelCaseToWords(),
                    description: "Marksman has marked this target until the end of the next action phase. The mark " +
                                 "is consumed dealing 5 Melee Damage, when an ally of Marksman attacks this unit.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.EndOf.Next.ActionPhase,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityIsAttacked
                        }, validators: new List<Validator>
                        {
                            new Validator(
                                conditions: new List<Condition>
                                {
                                    new Condition(conditionFlag: ConditionFlag.TargetIsDifferentTypeThanOrigin)
                                },
                                filters: new List<IFilterItem>
                                {
                                    new SpecificFlag(value: FilterFlag.Enemy),
                                    new SpecificFlag(value: FilterFlag.Unit)
                                })
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Marksman.CriticalMarkDamage
                    },
                    restoreChangesOnEnd: false),

                new Buff(
                    id: BehaviourId.Surfer.DismountBuff,
                    displayName: nameof(BehaviourId.Surfer.DismountBuff).CamelCaseToWords(),
                    description: "Upon death, reemerges as Horrior.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Surfer.DismountCreateEntity
                    },
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Ammunition(
                    id: BehaviourId.Mortar.DeadlyAmmunitionAmmunition,
                    displayName: nameof(BehaviourId.Mortar.DeadlyAmmunitionAmmunition).CamelCaseToWords(),
                    description: "Each ranged attack consumes 1 ammo out of 2 total. Cannot range attack when out " +
                                 "of ammo. Each ranged attack deals full Ranged Damage to all adjacent units around " +
                                 "the target.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maxAmmunitionAmount: 2,
                    ammunitionAttackTypes: new List<Attacks>
                    {
                        Attacks.Ranged
                    },
                    ammunitionAmountLostOnHit: 1,
                    onHitEffects: new List<EffectId>
                    {
                        EffectId.Mortar.DeadlyAmmunitionSearch
                    },
                    ammunitionRecoveredOnReload: 2,
                    applyOriginalAttackToTarget: false),

                new Wait(
                    id: BehaviourId.Mortar.ReloadWait,
                    displayName: nameof(BehaviourId.Mortar.ReloadWait).CamelCaseToWords(),
                    description: "Mortar will reload its ammunition at the end of this action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.EndOf.This.Action,
                    nextBehaviour: BehaviourId.Mortar.ReloadBuff),

                new Buff(
                    id: BehaviourId.Mortar.ReloadBuff,
                    displayName: nameof(BehaviourId.Mortar.ReloadBuff).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Mortar.ReloadReload
                    },
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Instant),

                new Buff(
                    id: BehaviourId.Mortar.PiercingBlastBuff,
                    displayName: nameof(BehaviourId.Mortar.PiercingBlastBuff).CamelCaseToWords(),
                    description: "Ranged Armour from the main target is ignored when attacking with Deadly Ammunition.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new AttackModification(
                            attackType: Attacks.Ranged,
                            modificationFlags: new List<ModificationFlag>
                            {
                                ModificationFlag.IgnoreArmour
                            })
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Death,
                    canStack: null,
                    canResetDuration: null,
                    alignment: Alignment.Positive),

                new Buff(
                    id: BehaviourId.Hawk.TacticalGogglesBuff,
                    displayName: nameof(BehaviourId.Hawk.TacticalGogglesBuff).CamelCaseToWords(),
                    description: "Gains +3 Vision range.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new StatModification(change: Change.AddMax, amount: 3, statType: StatType.Vision)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: null,
                    alignment: Alignment.Positive),

                new Buff(
                    id: BehaviourId.Hawk.LeadershipBuff,
                    displayName: nameof(BehaviourId.Hawk.LeadershipBuff).CamelCaseToWords(),
                    description: "Gains +1 Attack Distance range from nearby Hawk. Bonus will be lost at the end of " +
                                 "the next action or if Hawk is not adjacent anymore.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new AttackModification(
                            change: Change.AddMax,
                            amount: 1,
                            attackType: Attacks.Ranged,
                            attribute: AttackAttribute.MaxDistance)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.EndOf.This.Action,
                    canStack: false,
                    canResetDuration: null,
                    alignment: Alignment.Positive,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.SourceIsNotAdjacent
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Hawk.HealthKitBuff,
                    displayName: nameof(BehaviourId.Hawk.HealthKitBuff).CamelCaseToWords(),
                    description:
                    "Restores 1 Health to all adjacent friendly units at the start of each planning phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Hawk.HealthKitSearch,
                        EffectId.Hawk.HealthKitApplyBehaviour // Reapply the same buff to achieve periodic effect
                    },
                    endsAt: EndsAt.StartOf.Next.Planning,
                    canStack: true, // Only needed because final effects happen right before this behaviour is destroyed
                    canResetDuration: null,
                    alignment: Alignment.Positive),

                new Buff(
                    id: BehaviourId.Hawk.HealthKitHealBuff,
                    displayName: nameof(BehaviourId.Hawk.HealthKitHealBuff).CamelCaseToWords(),
                    description: "Heals for 1 Health.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddCurrent,
                            amount: 1,
                            statType: StatType.Health)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Instant,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Buildable(
                    id: BehaviourId.Cannon.AssemblingBuildable,
                    displayName: nameof(BehaviourId.Cannon.AssemblingBuildable).CamelCaseToWords(),
                    description:
                    "This machine can only be placed on the low ground and can be assembled by a maximum of " +
                    "3 Engineers at once.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                    },
                    maximumHelpers: 3),

                new Buildable(
                    id: BehaviourId.Ballista.AssemblingBuildable,
                    displayName: nameof(BehaviourId.Ballista.AssemblingBuildable).CamelCaseToWords(),
                    description:
                    "This machine can only be placed on a Watchtower or Bastion and can be assembled by a maximum " +
                    "of 1 Engineer at once.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new EntityCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedEntity: StructureId.Watchtower), 
                            // OR
                            new EntityCondition(
                                conditionFlag: ConditionFlag.Exists,
                                conditionedEntity: StructureId.Bastion)
                        })
                    },
                    maximumHelpers: 1),

                new Buildable(
                    id: BehaviourId.Radar.AssemblingBuildable,
                    displayName: nameof(BehaviourId.Radar.AssemblingBuildable).CamelCaseToWords(),
                    description:
                    "This machine can only be placed on the low ground and can be assembled by a maximum of " +
                    "1 Engineer at once.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementValidators: new List<Validator>
                    {
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsLowGround)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new Condition(conditionFlag: ConditionFlag.TargetIsUnoccupied)
                        }),
                        new Validator(conditions: new List<Condition>
                        {
                            new TileCondition(conditionFlag: ConditionFlag.DoesNotExist, conditionedTile: TileId.Mountains)
                        }),
                    },
                    maximumHelpers: 1),

                new Buff(
                    id: BehaviourId.Engineer.OperateBuff,
                    displayName: nameof(BehaviourId.Engineer.OperateBuff).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Engineer.OperateModifyCounter
                    },
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Engineer.OperateDestroy
                    },
                    endsAt: EndsAt.Instant),

                new Buff(
                    id: BehaviourId.Engineer.RepairStructureOrMachineBuff,
                    displayName: nameof(BehaviourId.Engineer.RepairStructureOrMachineBuff).CamelCaseToWords(),
                    description:
                    "This structure or machine will be repaired by +2 Health at the start of the planning phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Engineer.RepairApplyBehaviourSelf
                    },
                    finalModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddCurrent,
                            amount: 1,
                            statType: StatType.Health)
                    },
                    finalEffects: null,
                    endsAt: EndsAt.StartOf.Next.Planning,
                    canStack: true,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsInterrupted
                        })
                    },
                    removeOnConditionsMet: true),

                new Buff(
                    id: BehaviourId.Engineer.RepairHorriorBuff,
                    displayName: nameof(BehaviourId.Engineer.RepairHorriorBuff).CamelCaseToWords(),
                    description:
                    "This Horrior will have their Mount duration reduced by one turn at the start of the " +
                    "planning phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Engineer.RepairApplyBehaviourSelf
                    },
                    finalModifications: new List<Modification>
                    {
                        new DurationModification(
                            change: Change.SubtractCurrent,
                            amount: 1f,
                            behaviourToModify: BehaviourId.Horrior.MountWait)
                    },
                    finalEffects: null,
                    endsAt: EndsAt.StartOf.Next.Planning,
                    canStack: true,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsInterrupted
                        })
                    },
                    removeOnConditionsMet: true),

                new Wait(
                    id: BehaviourId.Engineer.RepairWait,
                    displayName: nameof(BehaviourId.Engineer.RepairWait).CamelCaseToWords(),
                    description: "Currently repairing.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.StartOf.Next.Planning),

                new Counter(
                    id: BehaviourId.Cannon.MachineCounter,
                    displayName: nameof(BehaviourId.Cannon.MachineCounter).CamelCaseToWords(),
                    description: "Needs 3 Engineers to operate this machine.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maxAmount: 3,
                    triggerAmount: 3,
                    triggeredEffects: new List<EffectId>
                    {
                        EffectId.Cannon.MachineRemoveBehaviour
                    }),

                new Buff(
                    id: BehaviourId.Cannon.MachineBuff,
                    displayName: nameof(BehaviourId.Cannon.MachineBuff).CamelCaseToWords(),
                    description:
                    "This machine is disabled until it is fully operated by the required number of Engineers.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.FullyDisabled
                    },
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: null,
                    alignment: Alignment.Negative,
                    triggers: null,
                    removeOnConditionsMet: false,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Cannon.HeatUpDangerZoneBuff,
                    displayName: nameof(BehaviourId.Cannon.HeatUpDangerZoneBuff).CamelCaseToWords(),
                    description:
                    "This tile will receive massive damage on the next Cannon's turn. Until then, Cannon's " +
                    "owner has vision of this tile.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.ProvidesVision
                    },
                    initialModifications: new List<Modification>
                    {
                        new StatModification(change: Change.SetCurrent, amount: 1, statType: StatType.Vision)
                    },
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Cannon.HeatUpApplyWaitBehaviour
                    },
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Cannon.HeatUpSearch,
                        EffectId.Cannon.HeatUpDestroy
                    },
                    endsAt: EndsAt.EndOf.Next.Action,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsInterrupted
                        }),
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Cannon.MachineRemoveBehaviour,
                        EffectId.Cannon.HeatUpDestroy
                    },
                    restoreChangesOnEnd: true),

                new Wait(
                    id: BehaviourId.Cannon.HeatUpWait,
                    displayName: nameof(BehaviourId.Cannon.HeatUpWait).CamelCaseToWords(),
                    description: "This Cannon is heating up for a blast at the danger zone.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.EndOf.Next.Action),

                new Counter(
                    id: BehaviourId.Ballista.MachineCounter,
                    displayName: nameof(BehaviourId.Ballista.MachineCounter).CamelCaseToWords(),
                    description: "Needs 1 Engineer to operate this machine.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maxAmount: 3,
                    triggerAmount: 3,
                    triggeredEffects: new List<EffectId>
                    {
                        EffectId.Ballista.MachineRemoveBehaviour
                    }),

                new Buff(
                    id: BehaviourId.Ballista.MachineBuff,
                    displayName: nameof(BehaviourId.Ballista.MachineBuff).CamelCaseToWords(),
                    description:
                    "This machine is disabled until it is fully operated by the required number of Engineers.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.FullyDisabled
                    },
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: null,
                    alignment: Alignment.Negative,
                    triggers: null,
                    removeOnConditionsMet: false,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Ballista.AimBuff,
                    displayName: nameof(BehaviourId.Ballista.AimBuff).CamelCaseToWords(),
                    description:
                    "This unit is aimed by a Ballista, which allows it to shoot every turn as long as this " +
                    "unit remains in range.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.EndOf.Next.ActionPhase,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.EntityFinishedMoving
                        }, validators: new List<Validator>
                        {
                            new ResultValidator(
                                searchEffect: EffectId.Ballista.AimSearch,
                                conditions: new List<Condition>
                                {
                                    new Condition(conditionFlag: ConditionFlag.NoActorsFoundFromEffect)
                                })
                        }),
                        new Trigger(events: new List<Event>
                        {
                            Event.OriginIsDestroyed
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: null,
                    restoreChangesOnEnd: null,
                    ownerAllowed: true,
                    hasSameInstanceForAllOwners: false),

                new Counter(
                    id: BehaviourId.Radar.MachineCounter,
                    displayName: nameof(BehaviourId.Radar.MachineCounter).CamelCaseToWords(),
                    description: "Needs 1 Engineer to operate this machine.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maxAmount: 3,
                    triggerAmount: 3,
                    triggeredEffects: new List<EffectId>
                    {
                        EffectId.Radar.MachineRemoveBehaviour
                    }),

                new Buff(
                    id: BehaviourId.Radar.MachineBuff,
                    displayName: nameof(BehaviourId.Radar.MachineBuff).CamelCaseToWords(),
                    description:
                    "This machine is disabled until it is fully operated by the required number of Engineers.", 
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.FullyDisabled
                    },
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: null,
                    alignment: Alignment.Negative,
                    triggers: null,
                    removeOnConditionsMet: false,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Radar.ResonatingSweepBuff,
                    displayName: nameof(BehaviourId.Radar.ResonatingSweepBuff).CamelCaseToWords(),
                    description: "Provides vision to the Radar's owner.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.ProvidesVision
                    },
                    initialModifications: new List<Modification>
                    {
                        new StatModification(change: Change.SetCurrent, amount: 1, statType: StatType.Vision)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Radar.ResonatingSweepDestroy
                    },
                    endsAt: EndsAt.StartOf.Next.Planning,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Buff(
                    id: BehaviourId.Radar.RadioLocationBuff,
                    displayName: nameof(BehaviourId.Radar.RadioLocationBuff).CamelCaseToWords(),
                    description: "",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Radar.RadioLocationSearchDestroy
                    },
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Radar.RadioLocationSearchCreate
                    },
                    endsAt: EndsAt.Instant,
                    canStack: false,
                    canResetDuration: false),

                new Buff(
                    id: BehaviourId.Radar.RadioLocationFeatureBuff,
                    displayName: nameof(BehaviourId.Radar.RadioLocationFeatureBuff).CamelCaseToWords(),
                    description: "This red dot is able to detect enemy units in the fog of war.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.OnlyVisibleToAllies,
                        ModificationFlag.OnlyVisibleInFogOfWar
                    },
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Radar.RadioLocationDestroy
                    },
                    endsAt: EndsAt.EndOf.This.Planning),

                new Counter(
                    id: BehaviourId.Vessel.MachineCounter,
                    displayName: nameof(BehaviourId.Vessel.MachineCounter).CamelCaseToWords(),
                    description: "Needs 3 Engineers to operate this machine.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    maxAmount: 3,
                    triggerAmount: 3,
                    triggeredEffects: new List<EffectId>
                    {
                        EffectId.Vessel.MachineRemoveBehaviour
                    }),

                new Buff(
                    id: BehaviourId.Vessel.MachineBuff,
                    displayName: nameof(BehaviourId.Vessel.MachineBuff).CamelCaseToWords(),
                    description:
                    "This machine is disabled until it is fully operated by the required number of Engineers.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: new List<ModificationFlag>
                    {
                        ModificationFlag.FullyDisabled
                    },
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: null,
                    alignment: Alignment.Negative,
                    triggers: null,
                    removeOnConditionsMet: false,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true),

                new InterceptDamage(
                    id: BehaviourId.Vessel.AbsorbentFieldInterceptDamage,
                    displayName: nameof(BehaviourId.Vessel.AbsorbentFieldInterceptDamage).CamelCaseToWords(),
                    description:
                    "The melee and ranged damage this unit receives is reduced by 50%, but this amount is " +
                    "also dealt to a nearby Vessel.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.Death,
                    numberOfInterceptions: 0,
                    damageTypes: new List<DamageType>
                    {
                        DamageType.Melee,
                        DamageType.Ranged
                    },
                    amountDealtInstead: null,
                    damageTypeDealtInstead: null,
                    shareWith: Location.Origin,
                    amountShared: new Amount(flat: 0, multiplier: 0.5f),
                    damageTypeShared: null,
                    reduceByTheSharedAmount: true,
                    ownerAllowed: true,
                    hasSameInstanceForAllOwners: false),

                new Buff(
                    id: BehaviourId.Vessel.FortifyDestroyBuff,
                    displayName: nameof(BehaviourId.Vessel.FortifyDestroyBuff).CamelCaseToWords(),
                    description: "Provides +3 Melee Armour and +3 Range Armour to all friendly units until the start " +
                                 "of Vessel's action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Vessel.FortifyDestroy
                    },
                    endsAt: EndsAt.StartOf.Next.Action,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Buff(
                    id: BehaviourId.Vessel.FortifyBuff,
                    displayName: nameof(BehaviourId.Vessel.FortifyBuff).CamelCaseToWords(),
                    description: "Has +3 Melee Armour and +3 Range Armour due to a nearby fortified Vessel",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.AddCurrent,
                            amount: 3,
                            statType: StatType.MeleeArmour),
                        new StatModification(
                            change: Change.AddCurrent,
                            amount: 3,
                            statType: StatType.RangedArmour)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.StartOf.Next.Action,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive,
                    restoreChangesOnEnd: true),

                new Buff(
                    id: BehaviourId.Omen.RenditionPlacementBuff,
                    displayName: nameof(BehaviourId.Omen.RenditionPlacementBuff).CamelCaseToWords(),
                    description: "This unit is about to have its rendition placed in a 3 Attack Distance.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: new List<EffectId>
                    {
                        EffectId.Omen.RenditionPlacementExecuteAbility
                    },
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.Instant,
                    canStack: true,
                    canResetDuration: false,
                    alignment: Alignment.Negative),

                new InterceptDamage(
                    id: BehaviourId.Omen.RenditionInterceptDamage,
                    displayName: nameof(BehaviourId.Omen.RenditionPlacementBuff).CamelCaseToWords(),
                    description:
                    "50% of any damage dealt will be dealt as Pure Damage to the unit that was the target of " +
                    "this rendition.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    endsAt: EndsAt.Death,
                    numberOfInterceptions: 0,
                    damageTypes: null,
                    amountDealtInstead: null,
                    damageTypeDealtInstead: null,
                    shareWith: Location.Source,
                    amountShared: new Amount(
                        flat: 0,
                        multiplier: 0.5f),
                    damageTypeShared: DamageType.Pure),

                new Buff(
                    id: BehaviourId.Omen.RenditionBuffTimer,
                    displayName: nameof(BehaviourId.Omen.RenditionBuffTimer).CamelCaseToWords(),
                    description: "The rendition stays for 2 action phases and disappears afterwards.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Omen.RenditionDestroy
                    },
                    endsAt: EndsAt.EndOf.Next.ActionPhase,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Negative,
                    triggers: new List<Trigger>
                    {
                        new Trigger(events: new List<Event>
                        {
                            Event.SourceIsDestroyed
                        })
                    },
                    removeOnConditionsMet: true,
                    conditionalEffects: new List<EffectId>
                    {
                        EffectId.Omen.RenditionDestroy
                    }),

                new Buff(
                    id: BehaviourId.Omen.RenditionBuffDeath,
                    displayName: nameof(BehaviourId.Omen.RenditionBuffDeath).CamelCaseToWords(),
                    description: "The unit that was the target of this rendition will emit a blast if this rendition " +
                                 "is destroyed. This blast would deal 10 Melee Damage and slow all adjacent enemy units by 50% " +
                                 "until the end of their next action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: null,
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: new List<EffectId>
                    {
                        EffectId.Omen.RenditionSearch
                    },
                    endsAt: EndsAt.Death,
                    canStack: false,
                    canResetDuration: false,
                    alignment: Alignment.Positive),

                new Buff(
                    id: BehaviourId.Omen.RenditionBuffDeath,
                    displayName: nameof(BehaviourId.Omen.RenditionBuffDeath).CamelCaseToWords(),
                    description: "This unit is slowed by 50% until the end of its next action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    modificationFlags: null,
                    initialModifications: new List<Modification>
                    {
                        new StatModification(
                            change: Change.MultiplyMax,
                            amount: 0.5f,
                            statType: StatType.Movement)
                    },
                    initialEffects: null,
                    finalModifications: null,
                    finalEffects: null,
                    endsAt: EndsAt.EndOf.Next.Action,
                    canStack: false,
                    canResetDuration: true,
                    alignment: Alignment.Negative,
                    triggers: null,
                    removeOnConditionsMet: null,
                    conditionalEffects: null,
                    restoreChangesOnEnd: true)
                
                #endregion Units
            };
        }
    }
}