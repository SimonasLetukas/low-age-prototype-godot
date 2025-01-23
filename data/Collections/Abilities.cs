using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Common.Shape;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeData.Domain.Resources;
using LowAgeData.Shared;
using low_age_prototype_common;

namespace LowAgeData.Collections
{
    public static class AbilitiesCollection
    {
        public static List<Ability> Get()
        {
            return new List<Ability>
            {
                #region Shared

                new Passive(
                    id: AbilityId.Shared.PassiveIncome,
                    displayName: nameof(AbilityId.Shared.PassiveIncome).CamelCaseToWords(),
                    description: "Provides 3 Scraps and 7 Celestium at the start of each planning phase.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Shared.PassiveIncomeApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Shared.ScrapsIncome,
                    displayName: nameof(AbilityId.Shared.ScrapsIncome).CamelCaseToWords(),
                    description: "At the start of each planning phase provides 5 Scraps.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Shared.ScrapsIncomeApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Shared.CelestiumIncome,
                    displayName: nameof(AbilityId.Shared.CelestiumIncome).CamelCaseToWords(),
                    description: "At the start of each planning phase provides 5 Celestium (-2 for each subsequently " +
                                 "constructed Obelisk, total minimum of 1).",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Shared.CelestiumIncomeApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.Shared.UnitInProduction,
                    displayName: nameof(AbilityId.Shared.UnitInProduction).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Shared.UnitInProductionBuildable),
                
                new Passive(
                    id: AbilityId.Shared.Revelators.BuildingStructure,
                    displayName: nameof(AbilityId.Shared.Revelators.BuildingStructure).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Shared.Revelators.BuildingStructureBuildable),
                
                new Passive(
                    id: AbilityId.Shared.Uee.BuildingStructure,
                    displayName: nameof(AbilityId.Shared.Uee.BuildingStructure).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Shared.Uee.BuildingStructureBuildable),
                
                new Passive(
                    id: AbilityId.Shared.Uee.PowerGenerator,
                    displayName: nameof(AbilityId.Shared.Uee.PowerGenerator).CamelCaseToWords(),
                    description: "UEE faction loses if Battery Core is destroyed.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Shared.Uee.PowerGeneratorApplyBehaviour
                    }),
                
                new Build(
                    id: AbilityId.Shared.Uee.Build,
                    displayName: nameof(AbilityId.Shared.Uee.Build).CamelCaseToWords(),
                    description: "Start building a UEE's structure in vision on a tile with Power. Collector and " +
                                 "Extractor can be built on tiles without Power.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementArea: new Map(),
                    useWalkableTilesAsPlacementArea: false,
                    selection: new List<Selection<EntityId>>
                    {
                        new Selection<EntityId>(name: StructureId.Collector, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 5),
                            new Payment(resource: ResourceId.Celestium, amount: 40)
                        }),
                        new Selection<EntityId>(name: StructureId.Extractor, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 12),
                            new Payment(resource: ResourceId.Celestium, amount: 30)
                        }),
                        new Selection<EntityId>(name: StructureId.PowerPole, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 15),
                            new Payment(resource: ResourceId.Celestium, amount: 38)
                        }),
                        new Selection<EntityId>(name: StructureId.Temple, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 25),
                            new Payment(resource: ResourceId.Celestium, amount: 40)
                        }),
                        new Selection<EntityId>(name: StructureId.MilitaryBase, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 14),
                            new Payment(resource: ResourceId.Celestium, amount: 50)
                        }),
                        new Selection<EntityId>(name: StructureId.Factory, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 22),
                            new Payment(resource: ResourceId.Celestium, amount: 63)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.FusionCoreUpgrade
                        }),
                        new Selection<EntityId>(name: StructureId.Laboratory, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 30),
                            new Payment(resource: ResourceId.Celestium, amount: 76)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.CelestiumCoreUpgrade
                        }),
                        new Selection<EntityId>(name: StructureId.Armoury, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 20),
                            new Payment(resource: ResourceId.Celestium, amount: 50)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.FusionCoreUpgrade
                        }),
                        new Selection<EntityId>(name: StructureId.Wall, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 10),
                            new Payment(resource: ResourceId.Celestium, amount: 50)
                        }),
                        new Selection<EntityId>(name: StructureId.Stairs, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 10),
                            new Payment(resource: ResourceId.Celestium, amount: 50)
                        }),
                        new Selection<EntityId>(name: StructureId.Gate, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 25),
                            new Payment(resource: ResourceId.Celestium, amount: 80)
                        }),
                        new Selection<EntityId>(name: StructureId.Watchtower, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 15),
                            new Payment(resource: ResourceId.Celestium, amount: 70)
                        }),
                        new Selection<EntityId>(name: StructureId.Bastion, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 30),
                            new Payment(resource: ResourceId.Celestium, amount: 120)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.CelestiumCoreUpgrade
                        }),
                    },
                    casterConsumesAction: false,
                    canHelp: false),
                
                new Passive(
                    id: AbilityId.Collector.Building,
                    displayName: nameof(AbilityId.Collector.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Collector.BuildingBuildable),
                
                new Passive(
                    id: AbilityId.Extractor.Building,
                    displayName: nameof(AbilityId.Extractor.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Extractor.BuildingBuildable),

                new Passive(
                    id: AbilityId.Wall.Building,
                    displayName: nameof(AbilityId.Wall.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Wall.BuildingBuildable),
                
                new Passive(
                    id: AbilityId.Shared.Uee.PowerDependency,
                    displayName: nameof(AbilityId.Shared.Uee.PowerDependency).CamelCaseToWords(),
                    description: "All abilities get disabled and loses 5 Health at the start of its action or action " +
                                 "phase if not connected to Power.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Shared.Uee.PowerDependencyApplyBehaviour
                    }),

                #endregion

                #region Structures

                new Passive(
                    id: AbilityId.Citadel.ExecutiveStash,
                    displayName: nameof(AbilityId.Citadel.ExecutiveStash).CamelCaseToWords(),
                    description: "Provides 4 Population and 4 spaces of storage for Weapons.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Citadel.ExecutiveStashApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Citadel.Ascendable,
                    displayName: nameof(AbilityId.Citadel.Ascendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Citadel.AscendableApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Citadel.HighGround,
                    displayName: nameof(AbilityId.Citadel.HighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Citadel.HighGroundApplyBehaviour
                    }),

                new Build(
                    id: AbilityId.Citadel.PromoteGoons,
                    displayName: nameof(AbilityId.Citadel.PromoteGoons).CamelCaseToWords(),
                    description: "Promote a new Revelators goon from the remaining Population.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementArea: new Map(),
                    useWalkableTilesAsPlacementArea: true,
                    selection: new List<Selection<EntityId>>
                    {
                        new Selection<EntityId>(name: UnitId.Slave, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 4),
                            new Payment(resource: ResourceId.MeleeWeapon, amount: 1),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                        new Selection<EntityId>(name: UnitId.Quickdraw, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 9),
                            new Payment(resource: ResourceId.RangedWeapon, amount: 2),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                        new Selection<EntityId>(name: UnitId.Gorger, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 7),
                            new Payment(resource: ResourceId.MeleeWeapon, amount: 2),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                        new Selection<EntityId>(name: UnitId.Camou, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 11),
                            new Payment(resource: ResourceId.MeleeWeapon, amount: 2),
                            new Payment(resource: ResourceId.SpecialWeapon, amount: 1),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                        new Selection<EntityId>(name: UnitId.Shaman, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 10),
                            new Payment(resource: ResourceId.RangedWeapon, amount: 1),
                            new Payment(resource: ResourceId.SpecialWeapon, amount: 2),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                        new Selection<EntityId>(name: UnitId.Pyre, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 15),
                            new Payment(resource: ResourceId.RangedWeapon, amount: 4),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                        new Selection<EntityId>(name: UnitId.BigBadBull, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 14),
                            new Payment(resource: ResourceId.MeleeWeapon, amount: 4),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                        new Selection<EntityId>(name: UnitId.Mummy, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 29),
                            new Payment(resource: ResourceId.SpecialWeapon, amount: 5),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                        new Selection<EntityId>(name: UnitId.Parasite, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 21),
                            new Payment(resource: ResourceId.MeleeWeapon, amount: 2),
                            new Payment(resource: ResourceId.RangedWeapon, amount: 2),
                            new Payment(resource: ResourceId.SpecialWeapon, amount: 2),
                            new Payment(resource: ResourceId.Population, amount: 1)
                        }),
                    },
                    casterConsumesAction: false,
                    canHelp: false),

                new Instant(
                    id: AbilityId.Obelisk.CelestiumDischarge,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityId.Obelisk.CelestiumDischarge).CamelCaseToWords(),
                    description: "Heals all nearby units in 5 Attack Distance by 5 Health. Adjacent units are healed " +
                                 "by 15 Health instead and their vision, Melee and Ranged Armour are all reduced by " +
                                 "3 for 3 actions.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    effects: new List<EffectId>
                    {
                        EffectId.Obelisk.CelestiumDischargeSearchLong,
                        EffectId.Obelisk.CelestiumDischargeSearchShort
                    },
                    cooldown: EndsAt.EndOf.Fourth.ActionPhase),

                new Passive(
                    id: AbilityId.Shack.Accommodation,
                    displayName: nameof(AbilityId.Shack.Accommodation).CamelCaseToWords(),
                    description: "Provides 2 Population",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Shack.AccommodationApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Smith.MeleeWeaponProduction,
                    displayName: nameof(AbilityId.Smith.MeleeWeaponProduction).CamelCaseToWords(),
                    description: "Every 20 Celestium generates a Melee Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Smith.MeleeWeaponProductionApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Fletcher.RangedWeaponProduction,
                    displayName: nameof(AbilityId.Fletcher.RangedWeaponProduction).CamelCaseToWords(),
                    description: "Every 25 Celestium generates a Ranged Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Fletcher.RangedWeaponProductionApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Alchemy.SpecialWeaponProduction,
                    displayName: nameof(AbilityId.Alchemy.SpecialWeaponProduction).CamelCaseToWords(),
                    description: "Every 30 Celestium generates a Special Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Alchemy.SpecialWeaponProductionApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Depot.WeaponStorage,
                    displayName: nameof(AbilityId.Depot.WeaponStorage).CamelCaseToWords(),
                    description: "Provides 4 spaces of storage for Weapons which are used for new unit production.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Depot.WeaponStorageApplyBehaviour
                    }),

                new Research(
                    id: AbilityId.Workshop.Research,
                    displayName: nameof(AbilityId.Workshop.Research).CamelCaseToWords(),
                    description: "Open a selection of research available for Revelators to unlock.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    selectionOfResearchToBeUnlocked: new List<Selection<ResearchId>>
                    {
                        new Selection<ResearchId>(name: ResearchId.Revelators.PoisonedSlits,
                            description: $"Unlock {nameof(AbilityId.Quickdraw.Cripple).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Quickdraw).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 5),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 50)
                            },
                            grayOutIfAlreadyExists: true),
                        new Selection<ResearchId>(name: ResearchId.Revelators.SpikedRope,
                            description: $"Unlock {nameof(AbilityId.Camou.Climb).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Camou).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 20),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 38)
                            },
                            grayOutIfAlreadyExists: true),
                        new Selection<ResearchId>(name: ResearchId.Revelators.QuestionableCargo,
                            description: $"Unlock {nameof(AbilityId.Pyre.PhantomMenace).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Pyre).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 4),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 75)
                            },
                            grayOutIfAlreadyExists: true),
                        new Selection<ResearchId>(name: ResearchId.Revelators.HumanfleshRations,
                            description: $"Unlock {nameof(AbilityId.Mummy.LeapOfHunger).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Mummy).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 15),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 70)
                            },
                            grayOutIfAlreadyExists: true),
                        new Selection<ResearchId>(name: ResearchId.Revelators.AdaptiveDigestion,
                            description: $"Unlock {nameof(AbilityId.Roach.CorrosiveSpit).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Roach).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 15),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 60)
                            },
                            grayOutIfAlreadyExists: true),
                    }),
                
                new Passive(
                    id: AbilityId.Outpost.Ascendable,
                    displayName: nameof(AbilityId.Outpost.Ascendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Outpost.AscendableApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Outpost.HighGround,
                    displayName: nameof(AbilityId.Outpost.HighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Outpost.HighGroundApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.Barricade.ProtectiveShield,
                    displayName: nameof(AbilityId.Barricade.ProtectiveShield).CamelCaseToWords(),
                    description: "Every unit adjacent to the shield of this Barricade receives +2 Range Armour.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.Barricade.ProtectiveShieldSearch),
                    
                new Passive(
                    id: AbilityId.Barricade.Caltrops,
                    displayName: nameof(AbilityId.Barricade.Caltrops).CamelCaseToWords(),
                    description: "Every unit adjacent to the spikes of this Barricade receives 5 Pure Damage at the " +
                                 "start of each action phase.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.Barricade.CaltropsSearch),
                
                new Toggle(
                    id: AbilityId.Barricade.Decompose,
                    displayName: nameof(AbilityId.Barricade.Decompose).CamelCaseToWords(),
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    turnPhase: TurnPhase.Planning, 
                    activationDescription: "Toggle to start inflicting 15 Pure Damage to itself at the start of " +
                                           "each action phase.",
                    onActivatedEffects: new List<EffectId>
                    {
                        EffectId.Barricade.DecomposeApplyBehaviour
                    },
                    onDeactivatedEffects: new List<EffectId>
                    {
                        EffectId.Barricade.DecomposeRemoveBehaviour
                    },
                    deactivationDescription: "Toggle to stop inflicting 15 Pure Damage to itself at the start of " +
                                             "each action phase."),

                new Passive(
                    id: AbilityId.BatteryCore.PowerGrid,
                    displayName: nameof(AbilityId.BatteryCore.PowerGrid).CamelCaseToWords(),
                    description: "Provides Power in 4 Distance.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.BatteryCore.PowerGridApplyBehaviour
                    }),

                new Instant(
                    id: AbilityId.BatteryCore.FusionCoreUpgrade,
                    displayName: nameof(AbilityId.BatteryCore.FusionCoreUpgrade).CamelCaseToWords(),
                    turnPhase: TurnPhase.Planning,
                    description: "Upgrade this Battery Core to Fusion Core.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    effects: new List<EffectId>
                    {
                        EffectId.BatteryCore.FusionCoreUpgradeApplyBehaviour
                    },
                    cost: new List<Payment>
                    {
                        new Payment(resource: ResourceId.Scraps, amount: 10),
                        new Payment(resource: ResourceId.Celestium, amount: 50)
                    }),
                
                new Passive(
                    id: AbilityId.FusionCore.PowerGrid,
                    displayName: nameof(AbilityId.FusionCore.PowerGrid).CamelCaseToWords(),
                    description: "Provides Power in 6 Distance.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.FusionCore.PowerGridApplyBehaviour
                    }),
                
                new Target(
                    id: AbilityId.FusionCore.DefenceProtocol,
                    turnPhase: TurnPhase.Planning, 
                    displayName: nameof(AbilityId.FusionCore.DefenceProtocol).CamelCaseToWords(),
                    description: "Select an enemy unit in 6 Distance. At the start of the action phase the target " +
                                 "receives 3 ranged attacks, each dealing 3 Range Damage.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 6, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.FusionCore.DefenceProtocolDamage,
                        EffectId.FusionCore.DefenceProtocolDamage,
                        EffectId.FusionCore.DefenceProtocolDamage
                    }),

                new Instant(
                    id: AbilityId.FusionCore.CelestiumCoreUpgrade,
                    displayName: nameof(AbilityId.FusionCore.CelestiumCoreUpgrade).CamelCaseToWords(),
                    turnPhase: TurnPhase.Planning,
                    description: "Upgrade this Fusion Core to Celestium Core.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    effects: new List<EffectId>
                    {
                        EffectId.FusionCore.CelestiumCoreUpgradeApplyBehaviour
                    },
                    cost: new List<Payment>
                    {
                        new Payment(resource: ResourceId.Scraps, amount: 10),
                        new Payment(resource: ResourceId.Celestium, amount: 100)
                    }),
                
                new Passive(
                    id: AbilityId.CelestiumCore.PowerGrid,
                    displayName: nameof(AbilityId.CelestiumCore.PowerGrid).CamelCaseToWords(),
                    description: "Provides Power in 8 Distance.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.CelestiumCore.PowerGridApplyBehaviour
                    }),
                
                new Target(
                    id: AbilityId.CelestiumCore.DefenceProtocol,
                    turnPhase: TurnPhase.Planning, 
                    displayName: nameof(AbilityId.CelestiumCore.DefenceProtocol).CamelCaseToWords(),
                    description: "Select an enemy unit in 8 Distance. At the start of the action phase the target " +
                                 "receives 4 ranged attacks, each dealing 4 Range Damage.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 8, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.CelestiumCore.DefenceProtocolDamage,
                        EffectId.CelestiumCore.DefenceProtocolDamage,
                        EffectId.CelestiumCore.DefenceProtocolDamage,
                        EffectId.CelestiumCore.DefenceProtocolDamage
                    }),

                new Instant(
                    id: AbilityId.CelestiumCore.HeightenedConductivity,
                    displayName: nameof(AbilityId.CelestiumCore.HeightenedConductivity).CamelCaseToWords(),
                    turnPhase: TurnPhase.Planning,
                    description: "Unlocks Improved Power Grid ability for all Power Poles.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    effects: new List<EffectId>
                    {
                        EffectId.CelestiumCore.HeightenedConductivityModifyResearch
                    },
                    cost: new List<Payment>
                    {
                        new Payment(resource: ResourceId.Scraps, amount: 6),
                        new Payment(resource: ResourceId.Celestium, amount: 40)
                    }),
                
                new Passive(
                    id: AbilityId.Collector.DirectTransitSystem,
                    displayName: nameof(AbilityId.Collector.DirectTransitSystem).CamelCaseToWords(),
                    description: "Provides +2 Scraps at the start of each planning phase if connected to Power.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Collector.DirectTransitSystemApplyBehaviourInactive
                    }),
                
                new Passive(
                    id: AbilityId.Extractor.ReinforcedInfrastructure,
                    displayName: nameof(AbilityId.Extractor.ReinforcedInfrastructure).CamelCaseToWords(),
                    description: "Gains additional 3 Melee Armour if connected to Power.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Extractor.ReinforcedInfrastructureApplyBehaviourInactive
                    }),
                
                new Passive(
                    id: AbilityId.PowerPole.PowerGrid,
                    displayName: nameof(AbilityId.PowerPole.PowerGrid).CamelCaseToWords(),
                    description: "Provides Power in 4 Distance.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.PowerPole.PowerGridApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.PowerPole.ExcessDistribution,
                    displayName: nameof(AbilityId.PowerPole.ExcessDistribution).CamelCaseToWords(),
                    description: "Regenerates +1 Shields to units and structures in 4 Distance at the start of each " +
                                 "planning phase.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.PowerPole.ExcessDistributionSearch
                    ),
                
                new Passive(
                    id: AbilityId.PowerPole.ImprovedPowerGrid,
                    displayName: nameof(AbilityId.PowerPole.ImprovedPowerGrid).CamelCaseToWords(),
                    description: "Distance of provided Power and shield regeneration is increased to 6.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Uee.HeightenedConductivity
                    },
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.PowerPole.ImprovedPowerGridModifyAbilityPowerGrid,
                        EffectId.PowerPole.ImprovedPowerGridModifyAbilityExcessDistribution
                    }),
                
                new Passive(
                    id: AbilityId.PowerPole.PowerGridImproved,
                    displayName: nameof(AbilityId.PowerPole.PowerGridImproved).CamelCaseToWords(),
                    description: "Provides Power in 6 Distance.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.PowerPole.PowerGridImprovedApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.PowerPole.ExcessDistributionImproved,
                    displayName: nameof(AbilityId.PowerPole.ExcessDistributionImproved).CamelCaseToWords(),
                    description: "Regenerates +1 Shields to units and structures in 6 Distance at the start of each " +
                                 "planning phase.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.PowerPole.ExcessDistributionImprovedSearch),
                
                new Passive(
                    id: AbilityId.Temple.KeepingTheFaith,
                    displayName: nameof(AbilityId.Temple.KeepingTheFaith).CamelCaseToWords(),
                    description: "Provides +2 Movement to friendly units in 6 Distance at the start of each action " +
                                 "phase. Additionally provides +1 Faith. Each point of Faith increases the " +
                                 "Initiative of all owned units by 1.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.Temple.KeepingTheFaithSearch,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Temple.KeepingTheFaithApplyBehaviourIncome
                    }),
                
                new Build(
                    id: AbilityId.MilitaryBase.Train,
                    displayName: nameof(AbilityId.MilitaryBase.Train).CamelCaseToWords(),
                    description: "Select a unit to be created at the specified location using Celestium for " +
                                 "production.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementArea: new Map(),
                    useWalkableTilesAsPlacementArea: true,
                    selection: new List<Selection<EntityId>>
                    {
                        new Selection<EntityId>(name: UnitId.Horrior, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 14),
                            new Payment(resource: ResourceId.Celestium, amount: 52)
                        }),
                        new Selection<EntityId>(name: UnitId.Surfer, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 20),
                            new Payment(resource: ResourceId.Celestium, amount: 52)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.FusionCoreUpgrade
                        }),
                        new Selection<EntityId>(name: UnitId.Marksman, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 10),
                            new Payment(resource: ResourceId.Celestium, amount: 60)
                        }),
                        new Selection<EntityId>(name: UnitId.Mortar, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 16),
                            new Payment(resource: ResourceId.Celestium, amount: 60)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.FusionCoreUpgrade
                        }),
                        new Selection<EntityId>(name: UnitId.Hawk, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 20),
                            new Payment(resource: ResourceId.Celestium, amount: 50)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.CelestiumCoreUpgrade
                        })
                    },
                    casterConsumesAction: true,
                    canHelp: false),
                
                new Build(
                    id: AbilityId.Factory.Train,
                    displayName: nameof(AbilityId.Factory.Train).CamelCaseToWords(),
                    description: "Select a unit to be created at the specified location using Celestium for " +
                                 "production.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementArea: new Map(),
                    useWalkableTilesAsPlacementArea: true,
                    selection: new List<Selection<EntityId>>
                    {
                        new Selection<EntityId>(name: UnitId.Engineer, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 6),
                            new Payment(resource: ResourceId.Celestium, amount: 55)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.FusionCoreUpgrade
                        }),
                        new Selection<EntityId>(name: UnitId.Vessel, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 22),
                            new Payment(resource: ResourceId.Celestium, amount: 99)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.CelestiumCoreUpgrade
                        })
                    },
                    casterConsumesAction: true,
                    canHelp: false),
                
                new Build(
                    id: AbilityId.Laboratory.Train,
                    displayName: nameof(AbilityId.Laboratory.Train).CamelCaseToWords(),
                    description: "Select a unit to be created at the specified location using Celestium for " +
                                 "production.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementArea: new Map(),
                    useWalkableTilesAsPlacementArea: true,
                    selection: new List<Selection<EntityId>>
                    {
                        new Selection<EntityId>(name: UnitId.Omen, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 30),
                            new Payment(resource: ResourceId.Celestium, amount: 90)
                        }, researchNeeded: new List<ResearchId> 
                        {
                            ResearchId.Uee.CelestiumCoreUpgrade
                        })
                    }, 
                    casterConsumesAction: true,
                    canHelp: false),
                
                new Research(
                    id: AbilityId.Armoury.Research,
                    displayName: nameof(AbilityId.Armoury.Research).CamelCaseToWords(),
                    description: "Open a selection of research available for UEE to unlock.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    selectionOfResearchToBeUnlocked: new List<Selection<ResearchId>>
                    {
                        new Selection<ResearchId>(name: ResearchId.Uee.HoverboardReignition,
                            description: $"Unlock {nameof(AbilityId.Horrior.Mount).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Horrior).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 6),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 70)
                            },
                            grayOutIfAlreadyExists: true,
                            researchNeeded: new List<ResearchId>
                            {
                                ResearchId.Uee.FusionCoreUpgrade
                            }),
                        new Selection<ResearchId>(name: ResearchId.Uee.MdPractice,
                            description: $"Unlock {nameof(AbilityId.Hawk.HealthKit).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Hawk).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 12),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 40)
                            },
                            grayOutIfAlreadyExists: true,
                            researchNeeded: new List<ResearchId>
                            {
                                ResearchId.Uee.FusionCoreUpgrade
                            }),
                        new Selection<ResearchId>(name: ResearchId.Uee.CelestiumCoatedMaterials,
                            description: $"Unlock {nameof(AbilityId.Radar.RadioLocation).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Radar).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 8),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 55)
                            },
                            grayOutIfAlreadyExists: true,
                            researchNeeded: new List<ResearchId>
                            {
                                ResearchId.Uee.FusionCoreUpgrade
                            }),
                        new Selection<ResearchId>(name: ResearchId.Uee.ExplosiveShrapnel,
                            description: $"Unlock {nameof(AbilityId.Mortar.PiercingBlast).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Mortar).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 14),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 60)
                            },
                            grayOutIfAlreadyExists: true,
                            researchNeeded: new List<ResearchId>
                            {
                                ResearchId.Uee.CelestiumCoreUpgrade
                            }),
                        new Selection<ResearchId>(name: ResearchId.Uee.HardenedMatrix,
                            description: $"Unlock {nameof(AbilityId.Vessel.Fortify).CamelCaseToWords()} for " +
                                         $"{nameof(UnitId.Vessel).CamelCaseToWords()}.",
                            cost: new List<Payment>
                            {
                                new Payment(resource: ResourceId.Scraps,
                                    amount: 18),
                                new Payment(resource: ResourceId.Celestium,
                                    amount: 65)
                            },
                            grayOutIfAlreadyExists: true,
                            researchNeeded: new List<ResearchId>
                            {
                                ResearchId.Uee.CelestiumCoreUpgrade
                            }),
                    }),
                
                new Passive(
                    id: AbilityId.Wall.HighGround,
                    displayName: nameof(AbilityId.Wall.HighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Wall.HighGroundApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.Stairs.Ascendable,
                    displayName: nameof(AbilityId.Stairs.Ascendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Stairs.AscendableApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.Gate.HighGround,
                    displayName: nameof(AbilityId.Gate.HighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Gate.HighGroundApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.Gate.Ascendable,
                    displayName: nameof(AbilityId.Gate.Ascendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Gate.AscendableApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.Gate.Entrance,
                    displayName: nameof(AbilityId.Gate.Entrance).CamelCaseToWords(),
                    description: "Allows movement through for friendly units and blocks it for enemy units.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Gate.EntranceApplyBehaviour
                    }),
                
                new Passive(
                    id: AbilityId.Watchtower.VantagePoint,
                    displayName: nameof(AbilityId.Watchtower.VantagePoint).CamelCaseToWords(),
                    description: "Provides +2 vision range, +1 Attack Distance and +1 Range Damage to ranged " +
                                 "units on top.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Watchtower.VantagePointApplyBehaviourHighGround
                    }),
                
                new Passive(
                    id: AbilityId.Bastion.Battlement,
                    displayName: nameof(AbilityId.Bastion.Battlement).CamelCaseToWords(),
                    description: "Provides +1 Range Armour, +1 vision range and +1 Attack Distance to all units " +
                                 "on top.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Bastion.BattlementApplyBehaviourHighGround
                    }),

                #endregion
                
                #region Units

                new Passive(
                    id: AbilityId.Leader.AllForOne,
                    displayName: nameof(AbilityId.Leader.AllForOne).CamelCaseToWords(),
                    description: "Revelators faction loses if Leader dies.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Leader.AllForOneApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Leader.MenacingPresence,
                    displayName: nameof(AbilityId.Leader.MenacingPresence).CamelCaseToWords(),
                    description: "All friendly and enemy units that enter 6 Attack Distance around Leader " +
                                 "have their Melee Damage and Ranged Damage reduced by 2 (total minimum of 1).",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.Leader.MenacingPresenceSearch),

                new Target(
                    id: AbilityId.Leader.OneForAll,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityId.Leader.OneForAll).CamelCaseToWords(),
                    description: "Select an adjacent Obelisk and sap its energy to give all friendly units " +
                                 "+2 Health. This Obelisk cannot be sapped again for 10 turns.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Leader.OneForAllApplyBehaviourObelisk
                    }),

                new Build(
                    id: AbilityId.Slave.Build,
                    displayName: nameof(AbilityId.Slave.Build).CamelCaseToWords(),
                    description: "Start building a Revelators' structure on an adjacent tile. Multiple Slaves " +
                                 "can build the structure, each additional one after the first provides half of the " +
                                 "Celestium production to the construction than the previous Slave.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementArea: new Circle(radius: 1, ignoreRadius: 0),
                    useWalkableTilesAsPlacementArea: false,
                    selection: new List<Selection<EntityId>>
                    {
                        new Selection<EntityId>(name: StructureId.Hut, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 5),
                            new Payment(resource: ResourceId.Celestium, amount: 40)
                        }, researchNeeded: new List<ResearchId> { ResearchId.Revelators.HumanfleshRations }), // TODO remove after testing
                        new Selection<EntityId>(name: StructureId.Obelisk, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 12),
                            new Payment(resource: ResourceId.Celestium, amount: 30)
                        }),
                        new Selection<EntityId>(name: StructureId.Shack, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 15),
                            new Payment(resource: ResourceId.Celestium, amount: 40)
                        }),
                        new Selection<EntityId>(name: StructureId.Smith, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 11),
                            new Payment(resource: ResourceId.Celestium, amount: 50)
                        }),
                        new Selection<EntityId>(name: StructureId.Fletcher, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 17),
                            new Payment(resource: ResourceId.Celestium, amount: 75)
                        }),
                        new Selection<EntityId>(name: StructureId.Alchemy, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 23),
                            new Payment(resource: ResourceId.Celestium, amount: 100)
                        }),
                        new Selection<EntityId>(name: StructureId.Depot, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 20),
                            new Payment(resource: ResourceId.Celestium, amount: 65)
                        }),
                        new Selection<EntityId>(name: StructureId.Workshop, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 20),
                            new Payment(resource: ResourceId.Celestium, amount: 50)
                        }),
                        new Selection<EntityId>(name: StructureId.Outpost, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 18),
                            new Payment(resource: ResourceId.Celestium, amount: 45)
                        }),
                        new Selection<EntityId>(name: StructureId.Barricade, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 16),
                            new Payment(resource: ResourceId.Celestium, amount: 35)
                        }),
                    },
                    casterConsumesAction: true,
                    canHelp: true,
                    helpEfficiency: 0.5f),

                new Passive(
                    id: AbilityId.Hut.Building,
                    displayName: nameof(AbilityId.Hut.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Hut.BuildingBuildable),

                new Passive(
                    id: AbilityId.Obelisk.Building,
                    displayName: nameof(AbilityId.Obelisk.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Obelisk.BuildingBuildable),

                new Target(
                    id: AbilityId.Slave.Repair,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityId.Slave.Repair).CamelCaseToWords(),
                    description: "Select an adjacent structure. At the start of the next planning phase the " +
                                 "selected structure receives +1 Health. Multiple Slaves can stack their repairs. Repair can be " +
                                 "interrupted.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Slave.RepairApplyBehaviourStructure
                    }),

                new Target(
                    id: AbilityId.Slave.ManualLabour,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityId.Slave.ManualLabour).CamelCaseToWords(),
                    description: "Select an adjacent Hut. At the start of the next planning phase receive +2 " +
                                 "Scraps. Maximum of one Slave per Hut.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Slave.ManualLabourApplyBehaviourHut
                    }),

                new Passive(
                    id: AbilityId.Quickdraw.Doubleshot,
                    displayName: nameof(AbilityId.Quickdraw.Doubleshot).CamelCaseToWords(),
                    description: "Ranged attacks twice.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Quickdraw.DoubleshotApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Quickdraw.Cripple,
                    displayName: nameof(AbilityId.Quickdraw.Cripple).CamelCaseToWords(),
                    description: "Each ranged attack cripples the target until the end of their action. During " +
                                 "this time target has 60% of their maximum Movement (rounded up) and cannot receive healing " +
                                 "from any sources. Multiple attacks on a crippled target have no additional effects.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Revelators.PoisonedSlits
                    },
                    onHitEffects: new List<EffectId>
                    {
                        EffectId.Quickdraw.CrippleApplyBehaviour
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Ranged
                    }),

                new Passive(
                    id: AbilityId.Gorger.FanaticSuicidePassive,
                    displayName: nameof(AbilityId.Gorger.FanaticSuicidePassive).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onHitEffects: new List<EffectId>
                    {
                        EffectId.Gorger.FanaticSuicideDestroy
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Melee
                    },
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Gorger.FanaticSuicideApplyBehaviourBuff
                    }),

                new Instant(
                    id: AbilityId.Gorger.FanaticSuicide,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Gorger.FanaticSuicide).CamelCaseToWords(),
                    description: "Either as an action, or instead of attacking, or upon getting killed Gorger " +
                                 "detonates, dealing its Melee Damage to all friendly and enemy units in 1 Attack Distance, " +
                                 "killing itself in the process.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    effects: new List<EffectId>
                    {
                        EffectId.Gorger.FanaticSuicideSearch,
                        EffectId.Gorger.FanaticSuicideDestroy
                    }),

                new Passive(
                    id: AbilityId.Camou.SilentAssassin,
                    displayName: nameof(AbilityId.Camou.SilentAssassin).CamelCaseToWords(),
                    description: "Deals 50% of target's lost Health as bonus Melee Damage if there are no friendly " +
                                 "units around Camou in 4 Attack Distance. Additionally, if the target has none of its allies " +
                                 "in the same radius, Camou silences the target for 2 of its actions, disabling the use of any " +
                                 "abilities or passives.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onHitEffects: new List<EffectId>
                    {
                        EffectId.Camou.SilentAssassinOnHitDamage,
                        EffectId.Camou.SilentAssassinOnHitSilence
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Melee
                    }),

                new Target(
                    id: AbilityId.Camou.Climb,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Camou.Climb).CamelCaseToWords(),
                    description: "Select an adjacent unoccupied space on a high ground. This space is considered " +
                                 "occupied until the end of the action phase at which point Camou moves to it. Passively, " +
                                 "Camou can move down from high ground at the additional cost of 1 Movement.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Camou.ClimbTeleport
                    },
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Revelators.SpikedRope
                    }),

                new Passive(
                    id: AbilityId.Camou.ClimbPassive,
                    displayName: nameof(AbilityId.Camou.ClimbPassive).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    periodicEffect: null,
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Revelators.SpikedRope
                    },
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Camou.ClimbApplyBehaviour
                    }),

                new Target(
                    id: AbilityId.Shaman.WondrousGoo,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Shaman.WondrousGoo).CamelCaseToWords(),
                    description: "Select a tile in 4 Attack Distance, which gets contaminated. Any unit in the " +
                                 "contamination has its vision and Attack Distance reduced by 3 (total minimum of 1) and " +
                                 "receives 1 Pure Damage at the start of its turn. At the end of this action phase, the " +
                                 "contamination area expands to adjacent tiles and stays until the end of the next action phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 4),
                    effects: new List<EffectId>
                    {
                        EffectId.Shaman.WondrousGooCreateEntity
                    },
                    cooldown: EndsAt.EndOf.Second.ActionPhase),

                new Passive(
                    id: AbilityId.Pyre.WallOfFlames,
                    displayName: nameof(AbilityId.Pyre.WallOfFlames).CamelCaseToWords(),
                    description: "The cargo leaves a path of flames when moved, which stay until the start of the " +
                                 "next Pyre's action or until death. Any unit which starts its turn or moves onto the flames " +
                                 "receives 5 Melee Damage.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Pyre.CargoCreateEntity
                    }),

                new Passive(
                    id: AbilityId.Pyre.PhantomMenace,
                    displayName: nameof(AbilityId.Pyre.PhantomMenace).CamelCaseToWords(),
                    description: "Can move through enemy units (but not buildings).",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Revelators.QuestionableCargo
                    },
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Pyre.PhantomMenaceApplyBehaviour
                    }),

                new Target(
                    id: AbilityId.BigBadBull.UnleashTheRage,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.BigBadBull.UnleashTheRage).CamelCaseToWords(),
                    description: "Select a direction (1 out of 4) originating from Big Bad Bull. Any two adjacent " +
                                 "units towards the selected direction suffer Bull's Melee Damage and are pushed one tile farther. " +
                                 "If the destination tile is occupied or impassable, the target receives additional 5 Melee Damage.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Custom(areas: new List<Area>
                    {
                        //  oo
                        // oxxo
                        // oxxo
                        //  oo
                        new Area(start: new Vector2<int>(x: 0, y: -1), size: new Vector2<int>(x: 2, y: 1)),
                        new Area(start: new Vector2<int>(x: 2, y: 0), size: new Vector2<int>(x: 1, y: 2)),
                        new Area(start: new Vector2<int>(x: 0, y: 2), size: new Vector2<int>(x: 2, y: 1)),
                        new Area(start: new Vector2<int>(x: -1, y: 0), size: new Vector2<int>(x: 1, y: 2)),
                    }),
                    effects: new List<EffectId>
                    {
                        EffectId.BigBadBull.UnleashTheRageSearch
                    },
                    cooldown: EndsAt.EndOf.Next.ActionPhase),

                new Target(
                    id: AbilityId.Mummy.SpawnRoach,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Mummy.SpawnRoach).CamelCaseToWords(),
                    description: "Select an adjacent tile in which Roach is created.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Mummy.SpawnRoachCreateEntity
                    },
                    researchNeeded: null,
                    cooldown: EndsAt.EndOf.Next.ActionPhase),

                new Passive(
                    id: AbilityId.Mummy.LeapOfHunger,
                    displayName: nameof(AbilityId.Mummy.LeapOfHunger).CamelCaseToWords(),
                    description: "Roach creation range is increased to 4 Distance.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Revelators.HumanfleshRations
                    },
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Mummy.LeapOfHungerModifyAbility
                    }),

                new Target(
                    id: AbilityId.Mummy.SpawnRoachModified,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Mummy.SpawnRoach).CamelCaseToWords(),
                    description: "Select a tile in 4 Distance in which Roach is created.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 4, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Mummy.SpawnRoachCreateEntity
                    },
                    researchNeeded: null,
                    cooldown: EndsAt.EndOf.Next.ActionPhase),

                new Passive(
                    id: AbilityId.Roach.DegradingCarapace,
                    displayName: nameof(AbilityId.Roach.DegradingCarapace).CamelCaseToWords(),
                    description: "At the start of each action loses 1 Health more than the previous action.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Roach.DegradingCarapaceApplyBehaviour
                    }),

                new Target(
                    id: AbilityId.Roach.CorrosiveSpit,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Roach.CorrosiveSpit).CamelCaseToWords(),
                    description: "Perform a ranged attack in 4 Distance dealing 6 (+8 to mechanical) Range Damage.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 4, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Roach.CorrosiveSpitDamage
                    },
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Revelators.AdaptiveDigestion
                    },
                    cooldown: EndsAt.EndOf.Second.ActionPhase),

                new Passive(
                    id: AbilityId.Parasite.ParalysingGrasp,
                    displayName: nameof(AbilityId.Parasite.ParalysingGrasp).CamelCaseToWords(),
                    description:
                    "Instead of attacking, Parasite attaches to the target. Both units occupy the same space and " +
                    "are considered enemy to all players. Parasite can only detach when the target is killed. All units who " +
                    "attack this combined unit do damage to both. On its turn, Parasite can move the target, using target's " +
                    "Movement. On target's turn, it must execute attack action to any friendly or enemy unit in range, " +
                    "otherwise skip turn.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: new List<EffectId>
                    {
                        EffectId.Parasite.ParalysingGraspApplyTetherBehaviour,
                        EffectId.Parasite.ParalysingGraspApplyAttackBehaviour,
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Melee
                    }),

                new Passive(
                    id: AbilityId.Horrior.ExpertFormation,
                    displayName: nameof(AbilityId.Horrior.ExpertFormation).CamelCaseToWords(),
                    description: "Gains +2 Range Armour if at least one other Horrior is adjacent.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.Horrior.ExpertFormationSearch),

                new Instant(
                    id: AbilityId.Horrior.Mount,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityId.Horrior.Mount).CamelCaseToWords(),
                    description:
                    "Spend 3 turns mounting (unable to act) and at the start of the fourth planning phase " +
                    "transform into Surfer.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    effects: new List<EffectId>
                    {
                        EffectId.Horrior.MountApplyBehaviour
                    },
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Uee.HoverboardReignition
                    }),

                new Passive(
                    id: AbilityId.Marksman.CriticalMark,
                    displayName: nameof(AbilityId.Marksman.CriticalMark).CamelCaseToWords(),
                    description:
                    "Each ranged attack marks the target unit. If a friendly non-Marksman unit attacks the marked " +
                    "target, the mark is consumed and the target receives 5 Melee Damage. The mark lasts until the end " +
                    "of the next action phase.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: new List<EffectId>
                    {
                        EffectId.Marksman.CriticalMarkApplyBehaviour
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Ranged
                    }),

                new Passive(
                    id: AbilityId.Surfer.Dismount,
                    displayName: nameof(AbilityId.Surfer.Dismount).CamelCaseToWords(),
                    description: "Upon death, reemerges as Horrior.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Surfer.DismountApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Mortar.DeadlyAmmunition,
                    displayName: nameof(AbilityId.Mortar.DeadlyAmmunition).CamelCaseToWords(),
                    description:
                    "Each ranged attack consumes 1 ammo out of 2 total. Cannot range attack when out of ammo. " +
                    "Each ranged attack deals full Ranged Damage to all adjacent units around the target.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Mortar.DeadlyAmmunitionApplyBehaviour
                    }),

                new Instant(
                    id: AbilityId.Mortar.Reload,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Mortar.Reload).CamelCaseToWords(),
                    description: "Spend this action phase reloading to full ammo.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    effects: new List<EffectId>
                    {
                        EffectId.Mortar.ReloadApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Mortar.PiercingBlast,
                    displayName: nameof(AbilityId.Mortar.PiercingBlast).CamelCaseToWords(),
                    description: "Ranged Armour from the main target is ignored when attacking with Deadly Ammunition.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Uee.ExplosiveShrapnel
                    },
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Mortar.PiercingBlastApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Hawk.TacticalGoggles,
                    displayName: nameof(AbilityId.Hawk.TacticalGoggles).CamelCaseToWords(),
                    description: "Gains +3 Vision range.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Hawk.TacticalGogglesApplyBehaviour
                    }),

                new Target(
                    id: AbilityId.Hawk.Leadership,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Hawk.Leadership).CamelCaseToWords(),
                    description: "Selected ranged adjacent friendly unit gains +1 Attack Distance. The bonus is " +
                                 "lost at the end of the target's next action, or if the targeted unit is no longer adjacent.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Hawk.LeadershipApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Hawk.HealthKit,
                    displayName: nameof(AbilityId.Hawk.HealthKit).CamelCaseToWords(),
                    description:
                    "Restores 1 Health to all adjacent friendly units at the start of each planning phase.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Uee.MdPractice
                    },
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Hawk.HealthKitApplyBehaviour
                    }),

                new Build(
                    id: AbilityId.Engineer.AssembleMachine,
                    displayName: nameof(AbilityId.Engineer.AssembleMachine).CamelCaseToWords(),
                    description:
                    "Start building a Machine on an adjacent tile. Multiple Engineers can build the Machine, " +
                    "up to a number needed to operate the Machine. Each Engineer provides current Celestium " +
                    "production to the construction.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    placementArea: new Circle(radius: 1, ignoreRadius: 0),
                    useWalkableTilesAsPlacementArea: false,
                    selection: new List<Selection<EntityId>>
                    {
                        new Selection<EntityId>(name: UnitId.Cannon, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 18),
                            new Payment(resource: ResourceId.Celestium, amount: 120)
                        }),
                        new Selection<EntityId>(name: UnitId.Ballista, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 7),
                            new Payment(resource: ResourceId.Celestium, amount: 106)
                        }),
                        new Selection<EntityId>(name: UnitId.Radar, cost: new List<Payment>
                        {
                            new Payment(resource: ResourceId.Scraps, amount: 15),
                            new Payment(resource: ResourceId.Celestium, amount: 84)
                        }),
                    },
                    casterConsumesAction: true,
                    canHelp: true,
                    helpEfficiency: 1f),

                new Passive(
                    id: AbilityId.Cannon.Assembling,
                    displayName: nameof(AbilityId.Cannon.Assembling).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Cannon.AssemblingBuildable),

                new Passive(
                    id: AbilityId.Ballista.Assembling,
                    displayName: nameof(AbilityId.Ballista.Assembling).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Ballista.AssemblingBuildable),

                new Passive(
                    id: AbilityId.Radar.Assembling,
                    displayName: nameof(AbilityId.Radar.Assembling).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    sprite: null,
                    onBuildBehaviour: BehaviourId.Radar.AssemblingBuildable),

                new Target(
                    id: AbilityId.Engineer.Operate,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Engineer.Operate).CamelCaseToWords(),
                    description:
                    "Select an adjacent Machine and start operating it if the Machine is built and does not " +
                    "have the maximum number of operating Engineers already.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Engineer.OperateApplyBehaviour
                    }),

                new Target(
                    id: AbilityId.Engineer.Repair,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityId.Engineer.Repair).CamelCaseToWords(),
                    description:
                    "Select an adjacent structure, Machine or Horrior. At the start of the next planning " +
                    "phase the selected structure or Machine receives +2 Health and selected Horrior's mounting " +
                    "time is decreased by 1 turn. Multiple Engineers can stack their repairs. Repair can be " +
                    "interrupted.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Engineer.RepairStructureApplyBehaviour,
                        EffectId.Engineer.RepairMachineApplyBehaviour,
                        EffectId.Engineer.RepairHorriorApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Cannon.Machine,
                    displayName: nameof(AbilityId.Cannon.Machine).CamelCaseToWords(),
                    description: "Can be built and operated by Engineers only. The Machine is functional and can act " +
                                 "only if maximum number of 3 Engineers are operating it.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Cannon.MachineApplyBehaviour
                    }),

                new Target(
                    id: AbilityId.Cannon.HeatUp,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Cannon.HeatUp).CamelCaseToWords(),
                    description:
                    "Instead of a regular ranged attack, select any tile in Attack Distance. This tile is " +
                    "revealed for allies and highlighted as dangerous for enemies. Instead of the next Cannon's " +
                    "action, the attack is triggered which deals massive Range Damage.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 10, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Cannon.HeatUpCreateEntity
                    },
                    researchNeeded: null,
                    cooldown: null,
                    overridesAttacks: new List<Attacks>
                    {
                        Attacks.Ranged
                    },
                    fallbackToAttack: false),

                new Passive(
                    id: AbilityId.Ballista.Machine,
                    displayName: nameof(AbilityId.Ballista.Machine).CamelCaseToWords(),
                    description: "Can be built and operated by Engineers only. The Machine is functional and can act " +
                                 "only if maximum number of 1 Engineer is operating it.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Ballista.MachineApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Ballista.AddOn,
                    displayName: nameof(AbilityId.Ballista.AddOn).CamelCaseToWords(),
                    description: "Can only be built on a Watchtower or Bastion.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png" // TODO
                    ),

                new Target(
                    id: AbilityId.Ballista.Aim,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Ballista.Aim).CamelCaseToWords(),
                    description: "Spends 1 action aiming, when attacking a new target. A dotted line to the target " +
                                 "indicates aiming. The target can stop this process if it moves out of Ballista's Attack " +
                                 "Distance. Once aimed, same target can be attacked each action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 9, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Ballista.AimDamage,
                        EffectId.Ballista.AimApplyBehaviour
                    },
                    researchNeeded: null,
                    cooldown: null,
                    overridesAttacks: new List<Attacks>
                    {
                        Attacks.Ranged
                    },
                    fallbackToAttack: false),

                new Passive(
                    id: AbilityId.Radar.Machine,
                    displayName: nameof(AbilityId.Radar.Machine).CamelCaseToWords(),
                    description: "Can be built and operated by Engineers only. The Machine is functional and can act " +
                                 "only if maximum number of 1 Engineer is operating it.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Radar.MachineApplyBehaviour
                    }),

                new Target(
                    id: AbilityId.Radar.ResonatingSweep,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Radar.ResonatingSweep).CamelCaseToWords(),
                    description:
                    "Selected tile in 15 Attack Distance and all adjacent tiles are revealed until the start " +
                    "of the next planning phase.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 15),
                    effects: new List<EffectId>
                    {
                        EffectId.Radar.ResonatingSweepCreateEntity
                    }),

                new Passive(
                    id: AbilityId.Radar.RadioLocation,
                    displayName: nameof(AbilityId.Radar.RadioLocation).CamelCaseToWords(),
                    description: "Enemy units in 15 Attack Distance are shown as red dots in the fog of war.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.Radar.RadioLocationApplyBehaviour,
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Uee.CelestiumCoatedMaterials
                    }),

                new Passive(
                    id: AbilityId.Vessel.Machine,
                    displayName: nameof(AbilityId.Vessel.Machine).CamelCaseToWords(),
                    description: "Can be operated by Engineers only (after it is built from Factory). The Machine is " +
                                 "functional and can act only if maximum number of 3 Engineers are operating it.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectId>
                    {
                        EffectId.Vessel.MachineApplyBehaviour
                    }),

                new Passive(
                    id: AbilityId.Vessel.AbsorbentField,
                    displayName: nameof(AbilityId.Vessel.AbsorbentField).CamelCaseToWords(),
                    description:
                    "Reduces Melee and Range damage done by 50% to all friendly units in 3 Attack Distance, " +
                    "which is instead dealt to Vessel.",
                    hasButton: true,
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    periodicEffect: EffectId.Vessel.AbsorbentFieldSearch),

                new Instant(
                    id: AbilityId.Vessel.Fortify,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Vessel.Fortify).CamelCaseToWords(),
                    description: "Provide +3 Melee Armour and +3 Range Armour to all friendly units in 3 Attack " +
                                 "Distance until the start of the next Vessel's action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    effects: new List<EffectId>
                    {
                        EffectId.Vessel.FortifyCreateEntity,
                    },
                    researchNeeded: new List<ResearchId>
                    {
                        ResearchId.Uee.HardenedMatrix
                    },
                    cooldown: EndsAt.EndOf.Second.ActionPhase),

                new Target(
                    id: AbilityId.Omen.Rendition,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Omen.Rendition).CamelCaseToWords(),
                    description: "Place a ghostly rendition of a selected enemy unit in 7 Attack Distance to an " +
                                 "unoccupied space in a 3 Attack Distance from the selected target. The rendition has the same " +
                                 "amount of Health, Melee and Range Armour as the selected target, cannot act, can be attacked " +
                                 "and stays for 2 action phases or until the selected target is dead. 50% of all damage done to " +
                                 "the rendition is done as Pure Damage to the selected target. If the rendition is destroyed " +
                                 "before disappearing, the selected target emits a blast which deals 10 Melee Damage and slows " +
                                 "all adjacent enemies by 50% until the end of their next action.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 7, ignoreRadius: 0),
                    effects: new List<EffectId>
                    {
                        EffectId.Omen.RenditionPlacementApplyBehaviour
                    },
                    cooldown: EndsAt.EndOf.Second.ActionPhase),

                new Target(
                    id: AbilityId.Omen.RenditionPlacement,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityId.Omen.RenditionPlacement).CamelCaseToWords(),
                    description: "Select an unoccupied space in a 3 Attack Distance to place the rendition of the " +
                                 "selected target.",
                    sprite: "res://assets/icons/icon_ability_build.png", // TODO
                    targetArea: new Circle(radius: 3),
                    effects: new List<EffectId>
                    {
                        EffectId.Omen.RenditionPlacementCreateEntity
                    })
                
                #endregion
            };
        }
    }
}