using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Resources;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;
using low_age_data.Domain.Shared.Shape;

namespace low_age_data.Collections
{
    public static class Abilities
    {
        public static List<Ability> Get()
        {
            return new List<Ability>
            {
                #region Shared

                new Passive(
                    name: AbilityName.Shared.PassiveIncome,
                    displayName: nameof(AbilityName.Shared.PassiveIncome).CamelCaseToWords(),
                    description: "Provides 3 Scraps and 7 Celestium at the start of each planning phase.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Shared.PassiveIncomeApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Shared.ScrapsIncome,
                    displayName: nameof(AbilityName.Shared.ScrapsIncome).CamelCaseToWords(),
                    description: "At the start of each planning phase provides 5 Scraps.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Shared.ScrapsIncomeApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Shared.CelestiumIncome,
                    displayName: nameof(AbilityName.Shared.CelestiumIncome).CamelCaseToWords(),
                    description: "At the start of each planning phase provides 5 Celestium (-2 for each subsequently " +
                                 "constructed Obelisk, total minimum of 1).",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Shared.CelestiumIncomeApplyBehaviour
                    }),
                
                new Passive(
                    name: AbilityName.Shared.Revelators.Building,
                    displayName: nameof(AbilityName.Shared.Revelators.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Shared.Revelators.BuildingBuildable),
                
                new Passive(
                    name: AbilityName.Shared.Uee.Building,
                    displayName: nameof(AbilityName.Shared.Uee.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Shared.Uee.BuildingBuildable),
                
                new Passive(
                    name: AbilityName.Shared.Uee.PowerGenerator,
                    displayName: nameof(AbilityName.Shared.Uee.PowerGenerator).CamelCaseToWords(),
                    description: "UEE faction loses if Battery Core is destroyed.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Shared.Uee.PowerGeneratorApplyBehaviour
                    }),
                
                new Build(
                    name: AbilityName.Shared.Uee.Build,
                    displayName: nameof(AbilityName.Shared.Uee.Build).CamelCaseToWords(),
                    description: "Start building a UEE's structure in vision on a tile with Power. Collector and " +
                                 "Extractor can be built on tiles without Power.",
                    placementArea: new Map(),
                    selection: new List<Selection<EntityName>>
                    {
                        new(name: StructureName.Collector, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 5),
                            new(resource: ResourceName.Celestium, amount: 40)
                        }),
                        new(name: StructureName.Extractor, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 12),
                            new(resource: ResourceName.Celestium, amount: 30)
                        }),
                        new(name: StructureName.PowerPole, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 15),
                            new(resource: ResourceName.Celestium, amount: 38)
                        }),
                        new(name: StructureName.Temple, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 25),
                            new(resource: ResourceName.Celestium, amount: 40)
                        }),
                        new(name: StructureName.MilitaryBase, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 14),
                            new(resource: ResourceName.Celestium, amount: 50)
                        }),
                        new(name: StructureName.Factory, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 22),
                            new(resource: ResourceName.Celestium, amount: 63)
                        }),
                        new(name: StructureName.Laboratory, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 30),
                            new(resource: ResourceName.Celestium, amount: 76)
                        }),
                        new(name: StructureName.Armoury, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 20),
                            new(resource: ResourceName.Celestium, amount: 50)
                        }),
                        new(name: StructureName.Wall, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 10),
                            new(resource: ResourceName.Celestium, amount: 50)
                        }),
                        new(name: StructureName.Stairs, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 10),
                            new(resource: ResourceName.Celestium, amount: 50)
                        }),
                        new(name: StructureName.Gate, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 25),
                            new(resource: ResourceName.Celestium, amount: 80)
                        }),
                        new(name: StructureName.Watchtower, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 15),
                            new(resource: ResourceName.Celestium, amount: 70)
                        }),
                        new(name: StructureName.Bastion, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 30),
                            new(resource: ResourceName.Celestium, amount: 120)
                        }),
                    },
                    casterConsumesAction: false,
                    canHelp: false),
                
                new Passive(
                    name: AbilityName.Collector.Building,
                    displayName: nameof(AbilityName.Collector.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Collector.BuildingBuildable),
                
                new Passive(
                    name: AbilityName.Extractor.Building,
                    displayName: nameof(AbilityName.Extractor.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Extractor.BuildingBuildable),

                new Passive(
                    name: AbilityName.Wall.Building,
                    displayName: nameof(AbilityName.Wall.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Wall.BuildingBuildable),
                
                new Passive(
                    name: AbilityName.Shared.Uee.PowerDependency,
                    displayName: nameof(AbilityName.Shared.Uee.PowerDependency).CamelCaseToWords(),
                    description: "All abilities get disabled and loses 5 Health at the start of its action or action " +
                                 "phase if not connected to Power.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Shared.Uee.PowerDependencyApplyBehaviour
                    }),

                #endregion

                #region Structures

                new Passive(
                    name: AbilityName.Citadel.ExecutiveStash,
                    displayName: nameof(AbilityName.Citadel.ExecutiveStash).CamelCaseToWords(),
                    description: "Provides 4 Population and 4 spaces of storage for Weapons.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Citadel.ExecutiveStashApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Citadel.Ascendable,
                    displayName: nameof(AbilityName.Citadel.Ascendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Citadel.AscendableApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Citadel.HighGround,
                    displayName: nameof(AbilityName.Citadel.HighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Citadel.HighGroundApplyBehaviour
                    }),

                new Produce(
                    name: AbilityName.Citadel.PromoteGoons,
                    displayName: nameof(AbilityName.Citadel.PromoteGoons).CamelCaseToWords(),
                    description: "Promote a new Revelators goon from the remaining Population.",
                    selection: new List<Selection<EntityName>>
                    {
                        new(name: UnitName.Slave, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 4),
                            new(resource: ResourceName.MeleeWeapon, amount: 1),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                        new(name: UnitName.Quickdraw, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 9),
                            new(resource: ResourceName.RangedWeapon, amount: 2),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                        new(name: UnitName.Gorger, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 7),
                            new(resource: ResourceName.MeleeWeapon, amount: 2),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                        new(name: UnitName.Camou, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 11),
                            new(resource: ResourceName.MeleeWeapon, amount: 2),
                            new(resource: ResourceName.SpecialWeapon, amount: 1),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                        new(name: UnitName.Shaman, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 10),
                            new(resource: ResourceName.RangedWeapon, amount: 1),
                            new(resource: ResourceName.SpecialWeapon, amount: 2),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                        new(name: UnitName.Pyre, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 15),
                            new(resource: ResourceName.RangedWeapon, amount: 4),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                        new(name: UnitName.BigBadBull, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 14),
                            new(resource: ResourceName.MeleeWeapon, amount: 4),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                        new(name: UnitName.Mummy, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 29),
                            new(resource: ResourceName.SpecialWeapon, amount: 5),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                        new(name: UnitName.Parasite, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 21),
                            new(resource: ResourceName.MeleeWeapon, amount: 2),
                            new(resource: ResourceName.RangedWeapon, amount: 2),
                            new(resource: ResourceName.SpecialWeapon, amount: 2),
                            new(resource: ResourceName.Population, amount: 1)
                        }),
                    },
                    canPlaceInWalkableAreaOnly: true,
                    hasQueue: false,
                    producedInstantly: true),

                new Instant(
                    name: AbilityName.Obelisk.CelestiumDischarge,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityName.Obelisk.CelestiumDischarge).CamelCaseToWords(),
                    description: "Heals all nearby units in 5 Attack Distance by 5 Health. Adjacent units are healed " +
                                 "by 15 Health instead and their vision, Melee and Ranged Armour are all reduced by " +
                                 "3 for 3 actions.",
                    effects: new List<EffectName>
                    {
                        EffectName.Obelisk.CelestiumDischargeSearchLong,
                        EffectName.Obelisk.CelestiumDischargeSearchShort
                    },
                    cooldown: EndsAt.EndOf.Fourth.ActionPhase),

                new Passive(
                    name: AbilityName.Shack.Accommodation,
                    displayName: nameof(AbilityName.Shack.Accommodation).CamelCaseToWords(),
                    description: "Provides 2 Population",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Shack.AccommodationApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Smith.MeleeWeaponProduction,
                    displayName: nameof(AbilityName.Smith.MeleeWeaponProduction).CamelCaseToWords(),
                    description: "Every 20 Celestium generates a Melee Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Smith.MeleeWeaponProductionApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Fletcher.RangedWeaponProduction,
                    displayName: nameof(AbilityName.Fletcher.RangedWeaponProduction).CamelCaseToWords(),
                    description: "Every 25 Celestium generates a Ranged Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Fletcher.RangedWeaponProductionApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Alchemy.SpecialWeaponProduction,
                    displayName: nameof(AbilityName.Alchemy.SpecialWeaponProduction).CamelCaseToWords(),
                    description: "Every 30 Celestium generates a Special Weapon and either stores it to an empty " +
                                 "Weapon space or waits until there is a free space available. ",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Alchemy.SpecialWeaponProductionApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Depot.WeaponStorage,
                    displayName: nameof(AbilityName.Depot.WeaponStorage).CamelCaseToWords(),
                    description: "Provides 4 spaces of storage for Weapons which are used for new unit production.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Depot.WeaponStorageApplyBehaviour
                    }),

                new Research(
                    name: AbilityName.Workshop.Research,
                    displayName: nameof(AbilityName.Workshop.Research).CamelCaseToWords(),
                    description: "Open a selection of research available for Revelators to unlock.",
                    selectionOfResearchToBeUnlocked: new List<Selection<ResearchName>>
                    {
                        new(name: ResearchName.Revelators.PoisonedSlits,
                            description: $"Unlock {nameof(AbilityName.Quickdraw.Cripple).CamelCaseToWords()} for " +
                                         $"{nameof(UnitName.Quickdraw).CamelCaseToWords()}.",
                            cost: new List<Cost>
                            {
                                new(resource: ResourceName.Scraps,
                                    amount: 5),
                                new(resource: ResourceName.Celestium,
                                    amount: 50)
                            },
                            grayOutIfAlreadyExists: true),
                        new(name: ResearchName.Revelators.SpikedRope,
                            description: $"Unlock {nameof(AbilityName.Camou.Climb).CamelCaseToWords()} for " +
                                         $"{nameof(UnitName.Camou).CamelCaseToWords()}.",
                            cost: new List<Cost>
                            {
                                new(resource: ResourceName.Scraps,
                                    amount: 20),
                                new(resource: ResourceName.Celestium,
                                    amount: 38)
                            },
                            grayOutIfAlreadyExists: true),
                        new(name: ResearchName.Revelators.QuestionableCargo,
                            description: $"Unlock {nameof(AbilityName.Pyre.PhantomMenace).CamelCaseToWords()} for " +
                                         $"{nameof(UnitName.Pyre).CamelCaseToWords()}.",
                            cost: new List<Cost>
                            {
                                new(resource: ResourceName.Scraps,
                                    amount: 4),
                                new(resource: ResourceName.Celestium,
                                    amount: 75)
                            },
                            grayOutIfAlreadyExists: true),
                        new(name: ResearchName.Revelators.HumanfleshRations,
                            description: $"Unlock {nameof(AbilityName.Mummy.LeapOfHunger).CamelCaseToWords()} for " +
                                         $"{nameof(UnitName.Mummy).CamelCaseToWords()}.",
                            cost: new List<Cost>
                            {
                                new(resource: ResourceName.Scraps,
                                    amount: 15),
                                new(resource: ResourceName.Celestium,
                                    amount: 70)
                            },
                            grayOutIfAlreadyExists: true),
                        new(name: ResearchName.Revelators.AdaptiveDigestion,
                            description: $"Unlock {nameof(AbilityName.Roach.CorrosiveSpit).CamelCaseToWords()} for " +
                                         $"{nameof(UnitName.Roach).CamelCaseToWords()}.",
                            cost: new List<Cost>
                            {
                                new(resource: ResourceName.Scraps,
                                    amount: 15),
                                new(resource: ResourceName.Celestium,
                                    amount: 60)
                            },
                            grayOutIfAlreadyExists: true),
                    }),
                
                new Passive(
                    name: AbilityName.Outpost.Ascendable,
                    displayName: nameof(AbilityName.Outpost.Ascendable).CamelCaseToWords(),
                    description: "Can be navigated through to go up to high ground.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Outpost.AscendableApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Outpost.HighGround,
                    displayName: nameof(AbilityName.Outpost.HighGround).CamelCaseToWords(),
                    description: "Provides an area of high ground to other units, who all gain +1 vision range and " +
                                 "+1 Attack Distance for their ranged attacks.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Outpost.HighGroundApplyBehaviour
                    }),
                
                new Passive(
                    name: AbilityName.Barricade.ProtectiveShield,
                    displayName: nameof(AbilityName.Barricade.ProtectiveShield).CamelCaseToWords(),
                    description: "Every unit adjacent to the shield of this Barricade receives +2 Range Armour.",
                    hasButton: true,
                    periodicEffect: EffectName.Barricade.ProtectiveShieldSearch),
                    
                new Passive(
                    name: AbilityName.Barricade.Caltrops,
                    displayName: nameof(AbilityName.Barricade.Caltrops).CamelCaseToWords(),
                    description: "Every unit adjacent to the spikes of this Barricade receives 5 Pure Damage at the " +
                                 "start of each action phase.",
                    hasButton: true,
                    periodicEffect: EffectName.Barricade.CaltropsSearch),
                
                new Toggle(
                    name: AbilityName.Barricade.Decompose,
                    displayName: nameof(AbilityName.Barricade.Decompose).CamelCaseToWords(),
                    turnPhase: TurnPhase.Planning, 
                    activationDescription: "Toggle to start inflicting 15 Pure Damage to itself at the start of " +
                                           "each action phase.",
                    onActivatedEffects: new List<EffectName>
                    {
                        EffectName.Barricade.DecomposeApplyBehaviour
                    },
                    onDeactivatedEffects: new List<EffectName>
                    {
                        EffectName.Barricade.DecomposeRemoveBehaviour
                    },
                    deactivationDescription: "Toggle to stop inflicting 15 Pure Damage to itself at the start of " +
                                             "each action phase."),

                new Passive(
                    name: AbilityName.BatteryCore.PowerGrid,
                    displayName: nameof(AbilityName.BatteryCore.PowerGrid).CamelCaseToWords(),
                    description: "Provides Power in 4 Distance.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.BatteryCore.PowerGridApplyBehaviour
                    }),

                new Instant(
                    name: AbilityName.BatteryCore.FusionCoreUpgrade,
                    displayName: nameof(AbilityName.BatteryCore.FusionCoreUpgrade).CamelCaseToWords(),
                    turnPhase: TurnPhase.Planning,
                    description: "Upgrade this Battery Core to Fusion Core.",
                    effects: new List<EffectName>
                    {
                        EffectName.BatteryCore.FusionCoreUpgradeApplyBehaviour
                    },
                    cost: new List<Cost>
                    {
                        new(resource: ResourceName.Scraps, amount: 10),
                        new(resource: ResourceName.Celestium, amount: 50)
                    }),
                
                new Passive(
                    name: AbilityName.FusionCore.PowerGrid,
                    displayName: nameof(AbilityName.FusionCore.PowerGrid).CamelCaseToWords(),
                    description: "Provides Power in 6 Distance.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.FusionCore.PowerGridApplyBehaviour
                    }),
                
                new Target(
                    name: AbilityName.FusionCore.DefenceProtocol,
                    turnPhase: TurnPhase.Planning, 
                    displayName: nameof(AbilityName.FusionCore.DefenceProtocol).CamelCaseToWords(),
                    description: "Select an enemy unit in 6 Distance. At the start of the action phase the target " +
                                 "receives 3 ranged attacks, each dealing 3 Range Damage.",
                    targetArea: new Circle(radius: 6, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.FusionCore.DefenceProtocolDamage,
                        EffectName.FusionCore.DefenceProtocolDamage,
                        EffectName.FusionCore.DefenceProtocolDamage
                    }),

                new Instant(
                    name: AbilityName.FusionCore.CelestiumCoreUpgrade,
                    displayName: nameof(AbilityName.FusionCore.CelestiumCoreUpgrade).CamelCaseToWords(),
                    turnPhase: TurnPhase.Planning,
                    description: "Upgrade this Fusion Core to Celestium Core.",
                    effects: new List<EffectName>
                    {
                        EffectName.FusionCore.CelestiumCoreUpgradeApplyBehaviour
                    },
                    cost: new List<Cost>
                    {
                        new(resource: ResourceName.Scraps, amount: 10),
                        new(resource: ResourceName.Celestium, amount: 100)
                    }),
                
                new Passive(
                    name: AbilityName.CelestiumCore.PowerGrid,
                    displayName: nameof(AbilityName.CelestiumCore.PowerGrid).CamelCaseToWords(),
                    description: "Provides Power in 8 Distance.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.CelestiumCore.PowerGridApplyBehaviour
                    }),
                
                new Target(
                    name: AbilityName.CelestiumCore.DefenceProtocol,
                    turnPhase: TurnPhase.Planning, 
                    displayName: nameof(AbilityName.CelestiumCore.DefenceProtocol).CamelCaseToWords(),
                    description: "Select an enemy unit in 8 Distance. At the start of the action phase the target " +
                                 "receives 4 ranged attacks, each dealing 4 Range Damage.",
                    targetArea: new Circle(radius: 8, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.CelestiumCore.DefenceProtocolDamage,
                        EffectName.CelestiumCore.DefenceProtocolDamage,
                        EffectName.CelestiumCore.DefenceProtocolDamage,
                        EffectName.CelestiumCore.DefenceProtocolDamage
                    }),

                new Instant(
                    name: AbilityName.CelestiumCore.HeightenedConductivity,
                    displayName: nameof(AbilityName.CelestiumCore.HeightenedConductivity).CamelCaseToWords(),
                    turnPhase: TurnPhase.Planning,
                    description: "Unlocks Improved Power Grid ability for all Power Poles.",
                    effects: new List<EffectName>
                    {
                        EffectName.CelestiumCore.HeightenedConductivityModifyResearch
                    },
                    cost: new List<Cost>
                    {
                        new(resource: ResourceName.Scraps, amount: 6),
                        new(resource: ResourceName.Celestium, amount: 40)
                    }),
                
                new Passive(
                    name: AbilityName.Collector.DirectTransitSystem,
                    displayName: nameof(AbilityName.Collector.DirectTransitSystem).CamelCaseToWords(),
                    description: "Provides +2 Scraps at the start of each planning phase if connected to Power.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Collector.DirectTransitSystemApplyBehaviourInactive
                    }),
                
                new Passive(
                    name: AbilityName.Extractor.ReinforcedInfrastructure,
                    displayName: nameof(AbilityName.Extractor.ReinforcedInfrastructure).CamelCaseToWords(),
                    description: "Gains additional 3 Melee Armour if connected to Power.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Extractor.ReinforcedInfrastructureApplyBehaviourInactive
                    }),
                
                new Passive(
                    name: AbilityName.PowerPole.PowerGrid,
                    displayName: nameof(AbilityName.PowerPole.PowerGrid).CamelCaseToWords(),
                    description: "Provides Power in 4 Distance.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.PowerPole.PowerGridApplyBehaviour
                    }),
                
                new Passive(
                    name: AbilityName.PowerPole.ExcessDistribution,
                    displayName: nameof(AbilityName.PowerPole.ExcessDistribution).CamelCaseToWords(),
                    description: "Regenerates +1 Shields to units and structures in 4 Distance at the start of each " +
                                 "planning phase.",
                    hasButton: true,
                    periodicEffect: EffectName.PowerPole.ExcessDistributionSearch
                    ),
                
                new Passive(
                    name: AbilityName.PowerPole.ImprovedPowerGrid,
                    displayName: nameof(AbilityName.PowerPole.ImprovedPowerGrid).CamelCaseToWords(),
                    description: "Distance of provided Power and shield regeneration is increased to 6.",
                    hasButton: true,
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Uee.HeightenedConductivity
                    },
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.PowerPole.ImprovedPowerGridModifyAbilityPowerGrid,
                        EffectName.PowerPole.ImprovedPowerGridModifyAbilityExcessDistribution
                    }),
                
                new Passive(
                    name: AbilityName.PowerPole.PowerGridImproved,
                    displayName: nameof(AbilityName.PowerPole.PowerGridImproved).CamelCaseToWords(),
                    description: "Provides Power in 6 Distance.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.PowerPole.PowerGridImprovedApplyBehaviour
                    }),
                
                new Passive(
                    name: AbilityName.PowerPole.ExcessDistributionImproved,
                    displayName: nameof(AbilityName.PowerPole.ExcessDistributionImproved).CamelCaseToWords(),
                    description: "Regenerates +1 Shields to units and structures in 6 Distance at the start of each " +
                                 "planning phase.",
                    hasButton: true,
                    periodicEffect: EffectName.PowerPole.ExcessDistributionImprovedSearch),
                
                new Passive(
                    name: AbilityName.Temple.KeepingTheFaith,
                    displayName: nameof(AbilityName.Temple.KeepingTheFaith).CamelCaseToWords(),
                    description: "Provides +2 Movement to friendly units in 6 Distance at the start of each action " +
                                 "phase. Additionally provides +1 Faith. Each point of Faith increases the " +
                                 "Initiative of all owned units by 1.",
                    hasButton: true,
                    periodicEffect: EffectName.Temple.KeepingTheFaithSearch,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Temple.KeepingTheFaithApplyBehaviourIncome
                    }),

                #endregion
                
                #region Units

                new Passive(
                    name: AbilityName.Leader.AllForOne,
                    displayName: nameof(AbilityName.Leader.AllForOne).CamelCaseToWords(),
                    description: "Revelators faction loses if Leader dies.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Leader.AllForOneApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Leader.MenacingPresence,
                    displayName: nameof(AbilityName.Leader.MenacingPresence).CamelCaseToWords(),
                    description: "All friendly and enemy units that enter 6 Attack Distance around Leader " +
                                 "have their Melee Damage and Ranged Damage reduced by 2 (total minimum of 1).",
                    hasButton: true,
                    periodicEffect: EffectName.Leader.MenacingPresenceSearch),

                new Target(
                    name: AbilityName.Leader.OneForAll,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityName.Leader.OneForAll).CamelCaseToWords(),
                    description: "Select an adjacent Obelisk and sap its energy to give all friendly units " +
                                 "+2 Health. This Obelisk cannot be sapped again for 10 turns.",
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Leader.OneForAllApplyBehaviourObelisk
                    }),

                new Build(
                    name: AbilityName.Slave.Build,
                    displayName: nameof(AbilityName.Slave.Build).CamelCaseToWords(),
                    description: "Start building a Revelators' structure on an adjacent tile. Multiple Slaves " +
                                 "can build the structure, each additional one after the first provides half of the " +
                                 "Celestium production to the construction than the previous Slave.",
                    placementArea: new Circle(radius: 1, ignoreRadius: 0),
                    selection: new List<Selection<EntityName>>
                    {
                        new(name: StructureName.Hut, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 5),
                            new(resource: ResourceName.Celestium, amount: 40)
                        }),
                        new(name: StructureName.Obelisk, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 12),
                            new(resource: ResourceName.Celestium, amount: 30)
                        }),
                        new(name: StructureName.Shack, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 15),
                            new(resource: ResourceName.Celestium, amount: 40)
                        }),
                        new(name: StructureName.Smith, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 11),
                            new(resource: ResourceName.Celestium, amount: 50)
                        }),
                        new(name: StructureName.Fletcher, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 17),
                            new(resource: ResourceName.Celestium, amount: 75)
                        }),
                        new(name: StructureName.Alchemy, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 23),
                            new(resource: ResourceName.Celestium, amount: 100)
                        }),
                        new(name: StructureName.Depot, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 20),
                            new(resource: ResourceName.Celestium, amount: 65)
                        }),
                        new(name: StructureName.Workshop, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 20),
                            new(resource: ResourceName.Celestium, amount: 50)
                        }),
                        new(name: StructureName.Outpost, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 18),
                            new(resource: ResourceName.Celestium, amount: 45)
                        }),
                        new(name: StructureName.Barricade, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 16),
                            new(resource: ResourceName.Celestium, amount: 35)
                        }),
                    },
                    casterConsumesAction: true,
                    canHelp: true,
                    helpEfficiency: 0.5f),

                new Passive(
                    name: AbilityName.Hut.Building,
                    displayName: nameof(AbilityName.Hut.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Hut.BuildingBuildable),

                new Passive(
                    name: AbilityName.Obelisk.Building,
                    displayName: nameof(AbilityName.Obelisk.Building).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Obelisk.BuildingBuildable),

                new Target(
                    name: AbilityName.Slave.Repair,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityName.Slave.Repair).CamelCaseToWords(),
                    description: "Select an adjacent structure. At the start of the next planning phase the " +
                                 "selected structure receives +1 Health. Multiple Slaves can stack their repairs. Repair can be " +
                                 "interrupted.",
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Slave.RepairApplyBehaviourStructure
                    }),

                new Target(
                    name: AbilityName.Slave.ManualLabour,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityName.Slave.ManualLabour).CamelCaseToWords(),
                    description: "Select an adjacent Hut. At the start of the next planning phase receive +2 " +
                                 "Scraps. Maximum of one Slave per Hut.",
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Slave.ManualLabourApplyBehaviourHut
                    }),

                new Passive(
                    name: AbilityName.Quickdraw.Doubleshot,
                    displayName: nameof(AbilityName.Quickdraw.Doubleshot).CamelCaseToWords(),
                    description: "Ranged attacks twice.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Quickdraw.DoubleshotApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Quickdraw.Cripple,
                    displayName: nameof(AbilityName.Quickdraw.Cripple).CamelCaseToWords(),
                    description: "Each ranged attack cripples the target until the end of their action. During " +
                                 "this time target has 60% of their maximum Movement (rounded up) and cannot receive healing " +
                                 "from any sources. Multiple attacks on a crippled target have no additional effects.",
                    hasButton: true,
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Revelators.PoisonedSlits
                    },
                    onHitEffects: new List<EffectName>
                    {
                        EffectName.Quickdraw.CrippleApplyBehaviour
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Ranged
                    }),

                new Passive(
                    name: AbilityName.Gorger.FanaticSuicidePassive,
                    displayName: nameof(AbilityName.Gorger.FanaticSuicidePassive).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onHitEffects: new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideDestroy
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Melee
                    },
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideApplyBehaviourBuff
                    }),

                new Instant(
                    name: AbilityName.Gorger.FanaticSuicide,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Gorger.FanaticSuicide).CamelCaseToWords(),
                    description: "Either as an action, or instead of attacking, or upon getting killed Gorger " +
                                 "detonates, dealing its Melee Damage to all friendly and enemy units in 1 Attack Distance, " +
                                 "killing itself in the process.",
                    effects: new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideSearch,
                        EffectName.Gorger.FanaticSuicideDestroy
                    }),

                new Passive(
                    name: AbilityName.Camou.SilentAssassin,
                    displayName: nameof(AbilityName.Camou.SilentAssassin).CamelCaseToWords(),
                    description: "Deals 50% of target's lost Health as bonus Melee Damage if there are no friendly " +
                                 "units around Camou in 4 Attack Distance. Additionally, if the target has none of its allies " +
                                 "in the same radius, Camou silences the target for 2 of its actions, disabling the use of any " +
                                 "abilities or passives.",
                    hasButton: true,
                    onHitEffects: new List<EffectName>
                    {
                        EffectName.Camou.SilentAssassinOnHitDamage,
                        EffectName.Camou.SilentAssassinOnHitSilence
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Melee
                    }),

                new Target(
                    name: AbilityName.Camou.Climb,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Camou.Climb).CamelCaseToWords(),
                    description: "Select an adjacent unoccupied space on a high ground. This space is considered " +
                                 "occupied until the end of the action phase at which point Camou moves to it. Passively, " +
                                 "Camou can move down from high ground at the additional cost of 1 Movement.",
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Camou.ClimbTeleport
                    },
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Revelators.SpikedRope
                    }),

                new Passive(
                    name: AbilityName.Camou.ClimbPassive,
                    displayName: nameof(AbilityName.Camou.ClimbPassive).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    periodicEffect: null,
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Revelators.SpikedRope
                    },
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Camou.ClimbApplyBehaviour
                    }),

                new Target(
                    name: AbilityName.Shaman.WondrousGoo,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Shaman.WondrousGoo).CamelCaseToWords(),
                    description: "Select a tile in 4 Attack Distance, which gets contaminated. Any unit in the " +
                                 "contamination has its vision and Attack Distance reduced by 3 (total minimum of 1) and " +
                                 "receives 1 Pure Damage at the start of its turn. At the end of this action phase, the " +
                                 "contamination area expands to adjacent tiles and stays until the end of the next action phase.",
                    targetArea: new Circle(radius: 4),
                    effects: new List<EffectName>
                    {
                        EffectName.Shaman.WondrousGooCreateEntity
                    },
                    cooldown: EndsAt.EndOf.Second.ActionPhase),

                new Passive(
                    name: AbilityName.Pyre.WallOfFlames,
                    displayName: nameof(AbilityName.Pyre.WallOfFlames).CamelCaseToWords(),
                    description: "The cargo leaves a path of flames when moved, which stay until the start of the " +
                                 "next Pyre's action or until death. Any unit which starts its turn or moves onto the flames " +
                                 "receives 5 Melee Damage.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Pyre.CargoCreateEntity
                    }),

                new Passive(
                    name: AbilityName.Pyre.PhantomMenace,
                    displayName: nameof(AbilityName.Pyre.PhantomMenace).CamelCaseToWords(),
                    description: "Can move through enemy units (but not buildings).",
                    hasButton: true,
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Revelators.QuestionableCargo
                    },
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Pyre.PhantomMenaceApplyBehaviour
                    }),

                new Target(
                    name: AbilityName.BigBadBull.UnleashTheRage,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.BigBadBull.UnleashTheRage).CamelCaseToWords(),
                    description: "Select a direction (1 out of 4) originating from Big Bad Bull. Any two adjacent " +
                                 "units towards the selected direction suffer Bull's Melee Damage and are pushed one tile farther. " +
                                 "If the destination tile is occupied or impassable, the target receives additional 5 Melee Damage.",
                    targetArea: new Custom(areas: new List<Area>
                    {
                        //  oo
                        // oxxo
                        // oxxo
                        //  oo
                        new(start: new Vector2<int>(x: 0, y: -1), size: new Vector2<int>(x: 2, y: 1)),
                        new(start: new Vector2<int>(x: 2, y: 0), size: new Vector2<int>(x: 1, y: 2)),
                        new(start: new Vector2<int>(x: 0, y: 2), size: new Vector2<int>(x: 2, y: 1)),
                        new(start: new Vector2<int>(x: -1, y: 0), size: new Vector2<int>(x: 1, y: 2)),
                    }),
                    effects: new List<EffectName>
                    {
                        EffectName.BigBadBull.UnleashTheRageSearch
                    },
                    cooldown: EndsAt.EndOf.Next.ActionPhase),

                new Target(
                    name: AbilityName.Mummy.SpawnRoach,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Mummy.SpawnRoach).CamelCaseToWords(),
                    description: "Select an adjacent tile in which Roach is created.",
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Mummy.SpawnRoachCreateEntity
                    },
                    researchNeeded: null,
                    cooldown: EndsAt.EndOf.Next.ActionPhase),

                new Passive(
                    name: AbilityName.Mummy.LeapOfHunger,
                    displayName: nameof(AbilityName.Mummy.LeapOfHunger).CamelCaseToWords(),
                    description: "Roach creation range is increased to 4 Distance.",
                    hasButton: true,
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Revelators.HumanfleshRations
                    },
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Mummy.LeapOfHungerModifyAbility
                    }),

                new Target(
                    name: AbilityName.Mummy.SpawnRoachModified,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Mummy.SpawnRoach).CamelCaseToWords(),
                    description: "Select a tile in 4 Distance in which Roach is created.",
                    targetArea: new Circle(radius: 4, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Mummy.SpawnRoachCreateEntity
                    },
                    researchNeeded: null,
                    cooldown: EndsAt.EndOf.Next.ActionPhase),

                new Passive(
                    name: AbilityName.Roach.DegradingCarapace,
                    displayName: nameof(AbilityName.Roach.DegradingCarapace).CamelCaseToWords(),
                    description: "At the start of each action loses 1 Health more than the previous action.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Roach.DegradingCarapaceApplyBehaviour
                    }),

                new Target(
                    name: AbilityName.Roach.CorrosiveSpit,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Roach.CorrosiveSpit).CamelCaseToWords(),
                    description: "Perform a ranged attack in 4 Distance dealing 6 (+8 to mechanical) Range Damage.",
                    targetArea: new Circle(radius: 4, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Roach.CorrosiveSpitDamage
                    },
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Revelators.AdaptiveDigestion
                    },
                    cooldown: EndsAt.EndOf.Second.ActionPhase),

                new Passive(
                    name: AbilityName.Parasite.ParalysingGrasp,
                    displayName: nameof(AbilityName.Parasite.ParalysingGrasp).CamelCaseToWords(),
                    description:
                    "Instead of attacking, Parasite attaches to the target. Both units occupy the same space and " +
                    "are considered enemy to all players. Parasite can only detach when the target is killed. All units who " +
                    "attack this combined unit do damage to both. On its turn, Parasite can move the target, using target's " +
                    "Movement. On target's turn, it must execute attack action to any friendly or enemy unit in range, " +
                    "otherwise skip turn.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: new List<EffectName>
                    {
                        EffectName.Parasite.ParalysingGraspApplyTetherBehaviour,
                        EffectName.Parasite.ParalysingGraspApplyAttackBehaviour,
                        EffectName.Parasite.ParalysingGraspApplySelfBehaviour,
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Melee
                    }),

                new Passive(
                    name: AbilityName.Horrior.ExpertFormation,
                    displayName: nameof(AbilityName.Horrior.ExpertFormation).CamelCaseToWords(),
                    description: "Gains +2 Range Armour if at least one other Horrior is adjacent.",
                    hasButton: true,
                    periodicEffect: EffectName.Horrior.ExpertFormationSearch),

                new Instant(
                    name: AbilityName.Horrior.Mount,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityName.Horrior.Mount).CamelCaseToWords(),
                    description:
                    "Spend 3 turns mounting (unable to act) and at the start of the fourth planning phase " +
                    "transform into Surfer.",
                    effects: new List<EffectName>
                    {
                        EffectName.Horrior.MountApplyBehaviour
                    },
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Uee.HoverboardReignition
                    }),

                new Passive(
                    name: AbilityName.Marksman.CriticalMark,
                    displayName: nameof(AbilityName.Marksman.CriticalMark).CamelCaseToWords(),
                    description:
                    "Each ranged attack marks the target unit. If a friendly non-Marksman unit attacks the marked " +
                    "target, the mark is consumed and the target receives 5 Melee Damage. The mark lasts until the end " +
                    "of the next action phase.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: new List<EffectName>
                    {
                        EffectName.Marksman.CriticalMarkApplyBehaviour
                    },
                    onHitAttackTypes: new List<Attacks>
                    {
                        Attacks.Ranged
                    }),

                new Passive(
                    name: AbilityName.Surfer.Dismount,
                    displayName: nameof(AbilityName.Surfer.Dismount).CamelCaseToWords(),
                    description: "Upon death, reemerges as Horrior.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Surfer.DismountApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Mortar.DeadlyAmmunition,
                    displayName: nameof(AbilityName.Mortar.DeadlyAmmunition).CamelCaseToWords(),
                    description:
                    "Each ranged attack consumes 1 ammo out of 2 total. Cannot range attack when out of ammo. " +
                    "Each ranged attack deals full Ranged Damage to all adjacent units around the target.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Mortar.DeadlyAmmunitionApplyBehaviour
                    }),

                new Instant(
                    name: AbilityName.Mortar.Reload,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Mortar.Reload).CamelCaseToWords(),
                    description: "Spend this action phase reloading to full ammo.",
                    effects: new List<EffectName>
                    {
                        EffectName.Mortar.ReloadApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Mortar.PiercingBlast,
                    displayName: nameof(AbilityName.Mortar.PiercingBlast).CamelCaseToWords(),
                    description: "Ranged Armour from the main target is ignored when attacking with Deadly Ammunition.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Uee.ExplosiveShrapnel
                    },
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Mortar.PiercingBlastApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Hawk.TacticalGoggles,
                    displayName: nameof(AbilityName.Hawk.TacticalGoggles).CamelCaseToWords(),
                    description: "Gains +3 Vision range.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Hawk.TacticalGogglesApplyBehaviour
                    }),

                new Target(
                    name: AbilityName.Hawk.Leadership,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Hawk.Leadership).CamelCaseToWords(),
                    description: "Selected ranged adjacent friendly unit gains +1 Attack Distance. The bonus is " +
                                 "lost at the end of the target's next action, or if the targeted unit is no longer adjacent.",
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Hawk.LeadershipApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Hawk.HealthKit,
                    displayName: nameof(AbilityName.Hawk.HealthKit).CamelCaseToWords(),
                    description:
                    "Restores 1 Health to all adjacent friendly units at the start of each planning phase.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Uee.MdPractice
                    },
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Hawk.HealthKitApplyBehaviour
                    }),

                new Build(
                    name: AbilityName.Engineer.AssembleMachine,
                    displayName: nameof(AbilityName.Engineer.AssembleMachine).CamelCaseToWords(),
                    description:
                    "Start building a Machine on an adjacent tile. Multiple Engineers can build the Machine, " +
                    "up to a number needed to operate the Machine. Each Engineer provides current Celestium " +
                    "production to the construction.",
                    placementArea: new Circle(radius: 1, ignoreRadius: 0),
                    selection: new List<Selection<EntityName>>
                    {
                        new(name: UnitName.Cannon, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 18),
                            new(resource: ResourceName.Celestium, amount: 120)
                        }),
                        new(name: UnitName.Ballista, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 7),
                            new(resource: ResourceName.Celestium, amount: 106)
                        }),
                        new(name: UnitName.Radar, cost: new List<Cost>
                        {
                            new(resource: ResourceName.Scraps, amount: 15),
                            new(resource: ResourceName.Celestium, amount: 84)
                        }),
                    },
                    casterConsumesAction: true,
                    canHelp: true,
                    helpEfficiency: 1f),

                new Passive(
                    name: AbilityName.Cannon.Assembling,
                    displayName: nameof(AbilityName.Cannon.Assembling).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Cannon.AssemblingBuildable),

                new Passive(
                    name: AbilityName.Ballista.Assembling,
                    displayName: nameof(AbilityName.Ballista.Assembling).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Ballista.AssemblingBuildable),

                new Passive(
                    name: AbilityName.Radar.Assembling,
                    displayName: nameof(AbilityName.Radar.Assembling).CamelCaseToWords(),
                    description: "",
                    hasButton: false,
                    onBuildBehaviour: BehaviourName.Radar.AssemblingBuildable),

                new Target(
                    name: AbilityName.Engineer.Operate,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Engineer.Operate).CamelCaseToWords(),
                    description:
                    "Select an adjacent Machine and start operating it if the Machine is built and does not " +
                    "have the maximum number of operating Engineers already.",
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Engineer.OperateApplyBehaviour
                    }),

                new Target(
                    name: AbilityName.Engineer.Repair,
                    turnPhase: TurnPhase.Planning,
                    displayName: nameof(AbilityName.Engineer.Repair).CamelCaseToWords(),
                    description:
                    "Select an adjacent structure, Machine or Horrior. At the start of the next planning " +
                    "phase the selected structure or Machine receives +2 Health and selected Horrior's mounting " +
                    "time is decreased by 1 turn. Multiple Engineers can stack their repairs. Repair can be " +
                    "interrupted.",
                    targetArea: new Circle(radius: 1, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Engineer.RepairStructureApplyBehaviour,
                        EffectName.Engineer.RepairMachineApplyBehaviour,
                        EffectName.Engineer.RepairHorriorApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Cannon.Machine,
                    displayName: nameof(AbilityName.Cannon.Machine).CamelCaseToWords(),
                    description: "Can be built and operated by Engineers only. The Machine is functional and can act " +
                                 "only if maximum number of 3 Engineers are operating it.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Cannon.MachineApplyBehaviour
                    }),

                new Target(
                    name: AbilityName.Cannon.HeatUp,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Cannon.HeatUp).CamelCaseToWords(),
                    description:
                    "Instead of a regular ranged attack, select any tile in Attack Distance. This tile is " +
                    "revealed for allies and highlighted as dangerous for enemies. Instead of the next Cannon's " +
                    "action, the attack is triggered which deals massive Range Damage.",
                    targetArea: new Circle(radius: 10, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Cannon.HeatUpCreateEntity
                    },
                    researchNeeded: null,
                    cooldown: null,
                    overridesAttacks: new List<Attacks>
                    {
                        Attacks.Ranged
                    },
                    fallbackToAttack: false),

                new Passive(
                    name: AbilityName.Ballista.Machine,
                    displayName: nameof(AbilityName.Ballista.Machine).CamelCaseToWords(),
                    description: "Can be built and operated by Engineers only. The Machine is functional and can act " +
                                 "only if maximum number of 1 Engineer is operating it.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Ballista.MachineApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Ballista.AddOn,
                    displayName: nameof(AbilityName.Ballista.AddOn).CamelCaseToWords(),
                    description: "Can only be built on a Watchtower.",
                    hasButton: true),

                new Target(
                    name: AbilityName.Ballista.Aim,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Ballista.Aim).CamelCaseToWords(),
                    description: "Spends 1 action aiming, when attacking a new target. A dotted line to the target " +
                                 "indicates aiming. The target can stop this process if it moves out of Ballista's Attack " +
                                 "Distance. Once aimed, same target can be attacked each action.",
                    targetArea: new Circle(radius: 9, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Ballista.AimDamage,
                        EffectName.Ballista.AimApplyBehaviour
                    },
                    researchNeeded: null,
                    cooldown: null,
                    overridesAttacks: new List<Attacks>
                    {
                        Attacks.Ranged
                    },
                    fallbackToAttack: false),

                new Passive(
                    name: AbilityName.Radar.Machine,
                    displayName: nameof(AbilityName.Radar.Machine).CamelCaseToWords(),
                    description: "Can be built and operated by Engineers only. The Machine is functional and can act " +
                                 "only if maximum number of 1 Engineer is operating it.",
                    hasButton: true,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Radar.MachineApplyBehaviour
                    }),

                new Target(
                    name: AbilityName.Radar.ResonatingSweep,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Radar.ResonatingSweep).CamelCaseToWords(),
                    description:
                    "Selected tile in 15 Attack Distance and all adjacent tiles are revealed until the start " +
                    "of the next planning phase.",
                    targetArea: new Circle(radius: 15),
                    effects: new List<EffectName>
                    {
                        EffectName.Radar.ResonatingSweepCreateEntity
                    }),

                new Passive(
                    name: AbilityName.Radar.RadioLocation,
                    displayName: nameof(AbilityName.Radar.RadioLocation).CamelCaseToWords(),
                    description: "Enemy units in 15 Attack Distance are shown as red dots in the fog of war.",
                    hasButton: true,
                    periodicEffect: EffectName.Radar.RadioLocationApplyBehaviour,
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Uee.CelestiumCoatedMaterials
                    }),

                new Passive(
                    name: AbilityName.Vessel.Machine,
                    displayName: nameof(AbilityName.Vessel.Machine).CamelCaseToWords(),
                    description: "Can be operated by Engineers only (after it is built from Factory). The Machine is " +
                                 "functional and can act only if maximum number of 3 Engineers are operating it.",
                    hasButton: true,
                    periodicEffect: null,
                    researchNeeded: null,
                    onHitEffects: null,
                    onHitAttackTypes: null,
                    onBirthEffects: new List<EffectName>
                    {
                        EffectName.Vessel.MachineApplyBehaviour
                    }),

                new Passive(
                    name: AbilityName.Vessel.AbsorbentField,
                    displayName: nameof(AbilityName.Vessel.AbsorbentField).CamelCaseToWords(),
                    description:
                    "Reduces Melee and Range damage done by 50% to all friendly units in 3 Attack Distance, " +
                    "which is instead dealt to Vessel.",
                    hasButton: true,
                    periodicEffect: EffectName.Vessel.AbsorbentFieldSearch),

                new Instant(
                    name: AbilityName.Vessel.Fortify,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Vessel.Fortify).CamelCaseToWords(),
                    description: "Provide +3 Melee Armour and +3 Range Armour to all friendly units in 3 Attack " +
                                 "Distance until the start of the next Vessel's action.",
                    effects: new List<EffectName>
                    {
                        EffectName.Vessel.FortifyCreateEntity,
                    },
                    researchNeeded: new List<ResearchName>
                    {
                        ResearchName.Uee.HardenedMatrix
                    },
                    cooldown: EndsAt.EndOf.Second.ActionPhase),

                new Target(
                    name: AbilityName.Omen.Rendition,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Omen.Rendition).CamelCaseToWords(),
                    description: "Place a ghostly rendition of a selected enemy unit in 7 Attack Distance to an " +
                                 "unoccupied space in a 3 Attack Distance from the selected target. The rendition has the same " +
                                 "amount of Health, Melee and Range Armour as the selected target, cannot act, can be attacked " +
                                 "and stays for 2 action phases or until the selected target is dead. 50% of all damage done to " +
                                 "the rendition is done as Pure Damage to the selected target. If the rendition is destroyed " +
                                 "before disappearing, the selected target emits a blast which deals 10 Melee Damage and slows " +
                                 "all adjacent enemies by 50% until the end of their next action.",
                    targetArea: new Circle(radius: 7, ignoreRadius: 0),
                    effects: new List<EffectName>
                    {
                        EffectName.Omen.RenditionPlacementApplyBehaviour
                    },
                    cooldown: EndsAt.EndOf.Second.ActionPhase),

                new Target(
                    name: AbilityName.Omen.RenditionPlacement,
                    turnPhase: TurnPhase.Action,
                    displayName: nameof(AbilityName.Omen.RenditionPlacement).CamelCaseToWords(),
                    description: "Select an unoccupied space in a 3 Attack Distance to place the rendition of the " +
                                 "selected target.",
                    targetArea: new Circle(radius: 3),
                    effects: new List<EffectName>
                    {
                        EffectName.Omen.RenditionPlacementCreateEntity
                    })
                
                #endregion
            };
        }
    }
}