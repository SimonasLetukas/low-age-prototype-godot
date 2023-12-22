using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Effects
{
    [JsonConverter(typeof(EffectIdJsonConverter))]
    public class EffectId : Id
    {
        [JsonConstructor]
        public EffectId(string value, bool addPrefix = false) : base(addPrefix ? $"effect-{value}" : value)
        {
        }

        public static class Shared
        {
            public static EffectId HighGroundSearch => new EffectId($"{nameof(Shared)}{nameof(HighGroundSearch)}".ToKebabCase(), true);
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId PassiveIncomeApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(PassiveIncomeApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId ScrapsIncomeApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(ScrapsIncomeApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId CelestiumIncomeApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(CelestiumIncomeApplyBehaviour)}".ToKebabCase(), true);

            public static class Revelators
            {
                public static EffectId NoPopulationSpaceSearch => new EffectId($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceSearch)}".ToKebabCase(), true);
                public static EffectId NoPopulationSpaceApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceApplyBehaviour)}".ToKebabCase(), true);
            }

            public static class Uee
            {
                public static EffectId PowerGeneratorApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorApplyBehaviour)}".ToKebabCase(), true);
                public static EffectId PowerGeneratorModifyPlayer => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorModifyPlayer)}".ToKebabCase(), true);
                public static EffectId PowerDependencyApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviour)}".ToKebabCase(), true);
                public static EffectId PowerDependencyDamage => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyDamage)}".ToKebabCase(), true);
                public static EffectId PowerDependencyApplyBehaviourDisable => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviourDisable)}".ToKebabCase(), true);
                public static EffectId PowerDependencyApplyBehaviourInactive => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviourInactive)}".ToKebabCase(), true);
                public static EffectId PositiveFaithSearch => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithSearch)}".ToKebabCase(), true);
                public static EffectId PositiveFaithApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithApplyBehaviour)}".ToKebabCase(), true);
            }
        }
        
        public static class Citadel
        {
            public static EffectId ExecutiveStashApplyBehaviour => new EffectId($"{nameof(Citadel)}{nameof(ExecutiveStashApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId AscendableApplyBehaviour => new EffectId($"{nameof(Citadel)}{nameof(AscendableApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Citadel)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Hut
        {
        }
        
        public static class Obelisk
        {
            public static EffectId CelestiumDischargeSearchLong => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeSearchLong)}".ToKebabCase(), true);
            public static EffectId CelestiumDischargeApplyBehaviourLong => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourLong)}".ToKebabCase(), true);
            public static EffectId CelestiumDischargeSearchShort => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeSearchShort)}".ToKebabCase(), true);
            public static EffectId CelestiumDischargeApplyBehaviourShort => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourShort)}".ToKebabCase(), true);
            public static EffectId CelestiumDischargeApplyBehaviourNegative => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourNegative)}".ToKebabCase(), true);
        }

        public static class Shack
        {
            public static EffectId AccommodationApplyBehaviour => new EffectId($"{nameof(Shack)}{nameof(AccommodationApplyBehaviour)}".ToKebabCase(), true);
        }
        
        public static class Smith
        {
            public static EffectId MeleeWeaponProductionApplyBehaviour => new EffectId($"{nameof(Smith)}{nameof(MeleeWeaponProductionApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Fletcher
        {
            public static EffectId RangedWeaponProductionApplyBehaviour => new EffectId($"{nameof(Fletcher)}{nameof(RangedWeaponProductionApplyBehaviour)}".ToKebabCase(), true);
        }
        
        public static class Alchemy
        {
            public static EffectId SpecialWeaponProductionApplyBehaviour => new EffectId($"{nameof(Alchemy)}{nameof(SpecialWeaponProductionApplyBehaviour)}".ToKebabCase(), true);
        }
        
        public static class Depot
        {
            public static EffectId WeaponStorageApplyBehaviour => new EffectId($"{nameof(Depot)}{nameof(WeaponStorageApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Workshop
        {
        }

        public static class Outpost
        {
            public static EffectId AscendableApplyBehaviour => new EffectId($"{nameof(Outpost)}{nameof(AscendableApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Outpost)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Barricade
        {
            public static EffectId ProtectiveShieldSearch => new EffectId($"{nameof(Barricade)}{nameof(ProtectiveShieldSearch)}".ToKebabCase(), true);
            public static EffectId ProtectiveShieldApplyBehaviour => new EffectId($"{nameof(Barricade)}{nameof(ProtectiveShieldApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId CaltropsSearch => new EffectId($"{nameof(Barricade)}{nameof(CaltropsSearch)}".ToKebabCase(), true);
            public static EffectId CaltropsDamage => new EffectId($"{nameof(Barricade)}{nameof(CaltropsDamage)}".ToKebabCase(), true);
            public static EffectId DecomposeApplyBehaviour => new EffectId($"{nameof(Barricade)}{nameof(DecomposeApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId DecomposeRemoveBehaviour => new EffectId($"{nameof(Barricade)}{nameof(DecomposeRemoveBehaviour)}".ToKebabCase(), true);
            public static EffectId DecomposeDamage => new EffectId($"{nameof(Barricade)}{nameof(DecomposeDamage)}".ToKebabCase(), true);
        }
        
        public static class BatteryCore
        {
            public static EffectId PowerGridApplyBehaviour => new EffectId($"{nameof(BatteryCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId FusionCoreUpgradeApplyBehaviour => new EffectId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId FusionCoreUpgradeCreateEntity => new EffectId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeCreateEntity)}".ToKebabCase(), true);
            public static EffectId FusionCoreUpgradeDestroy => new EffectId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeDestroy)}".ToKebabCase(), true);
            public static EffectId FusionCoreUpgradeModifyResearch => new EffectId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeModifyResearch)}".ToKebabCase(), true);
        }
        
        public static class FusionCore
        {
            public static EffectId PowerGridApplyBehaviour => new EffectId($"{nameof(FusionCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId DefenceProtocolDamage => new EffectId($"{nameof(FusionCore)}{nameof(DefenceProtocolDamage)}".ToKebabCase(), true);
            public static EffectId CelestiumCoreUpgradeApplyBehaviour => new EffectId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId CelestiumCoreUpgradeCreateEntity => new EffectId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeCreateEntity)}".ToKebabCase(), true);
            public static EffectId CelestiumCoreUpgradeDestroy => new EffectId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeDestroy)}".ToKebabCase(), true);
            public static EffectId CelestiumCoreUpgradeModifyResearch => new EffectId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeModifyResearch)}".ToKebabCase(), true);
        }
        
        public static class CelestiumCore
        {
            public static EffectId PowerGridApplyBehaviour => new EffectId($"{nameof(CelestiumCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId DefenceProtocolDamage => new EffectId($"{nameof(CelestiumCore)}{nameof(DefenceProtocolDamage)}".ToKebabCase(), true);
            public static EffectId HeightenedConductivityModifyResearch => new EffectId($"{nameof(CelestiumCore)}{nameof(HeightenedConductivityModifyResearch)}".ToKebabCase(), true);
        }
        
        public static class Collector
        {
            public static EffectId DirectTransitSystemApplyBehaviourInactive => new EffectId($"{nameof(Collector)}{nameof(DirectTransitSystemApplyBehaviourInactive)}".ToKebabCase(), true);
            public static EffectId DirectTransitSystemApplyBehaviourActive => new EffectId($"{nameof(Collector)}{nameof(DirectTransitSystemApplyBehaviourActive)}".ToKebabCase(), true);
        }
        
        public static class Extractor
        {
            public static EffectId ReinforcedInfrastructureApplyBehaviourInactive => new EffectId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureApplyBehaviourInactive)}".ToKebabCase(), true);
            public static EffectId ReinforcedInfrastructureApplyBehaviourActive => new EffectId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureApplyBehaviourActive)}".ToKebabCase(), true);
        }
        
        public static class PowerPole
        {
            public static EffectId PowerGridApplyBehaviour => new EffectId($"{nameof(PowerPole)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId ExcessDistributionSearch => new EffectId($"{nameof(PowerPole)}{nameof(ExcessDistributionSearch)}".ToKebabCase(), true);
            public static EffectId ExcessDistributionApplyBehaviour => new EffectId($"{nameof(PowerPole)}{nameof(ExcessDistributionApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId ImprovedPowerGridModifyAbilityPowerGrid => new EffectId($"{nameof(PowerPole)}{nameof(ImprovedPowerGridModifyAbilityPowerGrid)}".ToKebabCase(), true);
            public static EffectId ImprovedPowerGridModifyAbilityExcessDistribution => new EffectId($"{nameof(PowerPole)}{nameof(ImprovedPowerGridModifyAbilityExcessDistribution)}".ToKebabCase(), true);
            public static EffectId PowerGridImprovedApplyBehaviour => new EffectId($"{nameof(PowerPole)}{nameof(PowerGridImprovedApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId ExcessDistributionImprovedSearch => new EffectId($"{nameof(PowerPole)}{nameof(ExcessDistributionImprovedSearch)}".ToKebabCase(), true);
        }
        
        public static class Temple
        {
            public static EffectId KeepingTheFaithSearch => new EffectId($"{nameof(Temple)}{nameof(KeepingTheFaithSearch)}".ToKebabCase(), true);
            public static EffectId KeepingTheFaithApplyBehaviourBuff => new EffectId($"{nameof(Temple)}{nameof(KeepingTheFaithApplyBehaviourBuff)}".ToKebabCase(), true);
            public static EffectId KeepingTheFaithApplyBehaviourIncome => new EffectId($"{nameof(Temple)}{nameof(KeepingTheFaithApplyBehaviourIncome)}".ToKebabCase(), true);
        }
        
        public static class MilitaryBase
        {
        }
        
        public static class Factory
        {
        }
        
        public static class Laboratory
        {
        }
        
        public static class Armoury
        {
        }
        
        public static class Wall
        {
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Wall)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase(), true);
        }
        
        public static class Stairs
        {
            public static EffectId AscendableApplyBehaviour => new EffectId($"{nameof(Stairs)}{nameof(AscendableApplyBehaviour)}".ToKebabCase(), true);
        }
        
        public static class Gate
        {
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Gate)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId AscendableApplyBehaviour => new EffectId($"{nameof(Gate)}{nameof(AscendableApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId EntranceApplyBehaviour => new EffectId($"{nameof(Gate)}{nameof(EntranceApplyBehaviour)}".ToKebabCase(), true);
        }
        
        public static class Watchtower
        {
            public static EffectId VantagePointSearch => new EffectId($"{nameof(Watchtower)}{nameof(VantagePointSearch)}".ToKebabCase(), true);
            public static EffectId VantagePointApplyBehaviour => new EffectId($"{nameof(Watchtower)}{nameof(VantagePointApplyBehaviour)}".ToKebabCase(), true);
        }
        
        public static class Bastion
        {
            public static EffectId BattlementSearch => new EffectId($"{nameof(Bastion)}{nameof(BattlementSearch)}".ToKebabCase(), true);
            public static EffectId BattlementApplyBehaviour => new EffectId($"{nameof(Bastion)}{nameof(BattlementApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Leader
        {
            public static EffectId AllForOneApplyBehaviour => new EffectId($"{nameof(Leader)}{nameof(AllForOneApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId AllForOneModifyPlayer => new EffectId($"{nameof(Leader)}{nameof(AllForOneModifyPlayer)}".ToKebabCase(), true);
            public static EffectId MenacingPresenceSearch => new EffectId($"{nameof(Leader)}{nameof(MenacingPresenceSearch)}".ToKebabCase(), true);
            public static EffectId MenacingPresenceApplyBehaviour => new EffectId($"{nameof(Leader)}{nameof(MenacingPresenceApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId OneForAllApplyBehaviourObelisk => new EffectId($"{nameof(Leader)}{nameof(OneForAllApplyBehaviourObelisk)}".ToKebabCase(), true);
            public static EffectId OneForAllSearch => new EffectId($"{nameof(Leader)}{nameof(OneForAllSearch)}".ToKebabCase(), true);
            public static EffectId OneForAllApplyBehaviourHeal => new EffectId($"{nameof(Leader)}{nameof(OneForAllApplyBehaviourHeal)}".ToKebabCase(), true);
        }

        public static class Slave
        {
            public static EffectId RepairApplyBehaviourStructure => new EffectId($"{nameof(Slave)}{nameof(RepairApplyBehaviourStructure)}".ToKebabCase(), true);
            public static EffectId RepairApplyBehaviourSelf => new EffectId($"{nameof(Slave)}{nameof(RepairApplyBehaviourSelf)}".ToKebabCase(), true);
            public static EffectId ManualLabourApplyBehaviourHut => new EffectId($"{nameof(Slave)}{nameof(ManualLabourApplyBehaviourHut)}".ToKebabCase(), true);
            public static EffectId ManualLabourApplyBehaviourSelf => new EffectId($"{nameof(Slave)}{nameof(ManualLabourApplyBehaviourSelf)}".ToKebabCase(), true);
            public static EffectId ManualLabourModifyPlayer => new EffectId($"{nameof(Slave)}{nameof(ManualLabourModifyPlayer)}".ToKebabCase(), true);
        }

        public static class Quickdraw
        {
            public static EffectId DoubleshotApplyBehaviour => new EffectId($"{nameof(Quickdraw)}{nameof(DoubleshotApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId CrippleApplyBehaviour => new EffectId($"{nameof(Quickdraw)}{nameof(CrippleApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Gorger
        {
            public static EffectId FanaticSuicideApplyBehaviourBuff => new EffectId($"{nameof(Gorger)}{nameof(FanaticSuicideApplyBehaviourBuff)}".ToKebabCase(), true);
            public static EffectId FanaticSuicideDestroy => new EffectId($"{nameof(Gorger)}{nameof(FanaticSuicideDestroy)}".ToKebabCase(), true);
            public static EffectId FanaticSuicideSearch => new EffectId($"{nameof(Gorger)}{nameof(FanaticSuicideSearch)}".ToKebabCase(), true);
            public static EffectId FanaticSuicideDamage => new EffectId($"{nameof(Gorger)}{nameof(FanaticSuicideDamage)}".ToKebabCase(), true);
        }

        public static class Camou
        {
            public static EffectId SilentAssassinOnHitDamage => new EffectId($"{nameof(Camou)}{nameof(SilentAssassinOnHitDamage)}".ToKebabCase(), true);
            public static EffectId SilentAssassinOnHitSilence => new EffectId($"{nameof(Camou)}{nameof(SilentAssassinOnHitSilence)}".ToKebabCase(), true);
            public static EffectId SilentAssassinSearchFriendly => new EffectId($"{nameof(Camou)}{nameof(SilentAssassinSearchFriendly)}".ToKebabCase(), true);
            public static EffectId SilentAssassinSearchEnemy => new EffectId($"{nameof(Camou)}{nameof(SilentAssassinSearchEnemy)}".ToKebabCase(), true);
            public static EffectId ClimbTeleport => new EffectId($"{nameof(Camou)}{nameof(ClimbTeleport)}".ToKebabCase(), true);
            public static EffectId ClimbApplyBehaviour => new EffectId($"{nameof(Camou)}{nameof(ClimbApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Shaman
        {
            public static EffectId WondrousGooCreateEntity => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooCreateEntity)}".ToKebabCase(), true);
            public static EffectId WondrousGooSearch => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooSearch)}".ToKebabCase(), true);
            public static EffectId WondrousGooApplyBehaviour => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId WondrousGooDestroy => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooDestroy)}".ToKebabCase(), true);
            public static EffectId WondrousGooDamage => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooDamage)}".ToKebabCase(), true);
        }

        public static class Pyre
        {
            public static EffectId CargoCreateEntity => new EffectId($"{nameof(Pyre)}{nameof(CargoCreateEntity)}".ToKebabCase(), true);
            public static EffectId WallOfFlamesCreateEntity => new EffectId($"{nameof(Pyre)}{nameof(WallOfFlamesCreateEntity)}".ToKebabCase(), true);
            public static EffectId WallOfFlamesDestroy => new EffectId($"{nameof(Pyre)}{nameof(WallOfFlamesDestroy)}".ToKebabCase(), true);
            public static EffectId WallOfFlamesDamage => new EffectId($"{nameof(Pyre)}{nameof(WallOfFlamesDamage)}".ToKebabCase(), true);
            public static EffectId PhantomMenaceApplyBehaviour => new EffectId($"{nameof(Pyre)}{nameof(PhantomMenaceApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class BigBadBull
        {
            public static EffectId UnleashTheRageSearch => new EffectId($"{nameof(BigBadBull)}{nameof(UnleashTheRageSearch)}".ToKebabCase(), true);
            public static EffectId UnleashTheRageDamage => new EffectId($"{nameof(BigBadBull)}{nameof(UnleashTheRageDamage)}".ToKebabCase(), true);
            public static EffectId UnleashTheRageForce => new EffectId($"{nameof(BigBadBull)}{nameof(UnleashTheRageForce)}".ToKebabCase(), true);
            public static EffectId UnleashTheRageForceDamage => new EffectId($"{nameof(BigBadBull)}{nameof(UnleashTheRageForceDamage)}".ToKebabCase(), true);
        }

        public static class Mummy
        {
            public static EffectId SpawnRoachCreateEntity => new EffectId($"{nameof(Mummy)}{nameof(SpawnRoachCreateEntity)}".ToKebabCase(), true);
            public static EffectId LeapOfHungerModifyAbility => new EffectId($"{nameof(Mummy)}{nameof(LeapOfHungerModifyAbility)}".ToKebabCase(), true);
        }

        public static class Roach
        {
            public static EffectId DegradingCarapaceApplyBehaviour => new EffectId($"{nameof(Roach)}{nameof(DegradingCarapaceApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId DegradingCarapacePeriodicApplyBehaviour => new EffectId($"{nameof(Roach)}{nameof(DegradingCarapacePeriodicApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId DegradingCarapaceSelfDamage => new EffectId($"{nameof(Roach)}{nameof(DegradingCarapaceSelfDamage)}".ToKebabCase(), true);
            public static EffectId CorrosiveSpitDamage => new EffectId($"{nameof(Roach)}{nameof(CorrosiveSpitDamage)}".ToKebabCase(), true);
        }

        public static class Parasite
        {
            public static EffectId ParalysingGraspApplyTetherBehaviour => new EffectId($"{nameof(Parasite)}{nameof(ParalysingGraspApplyTetherBehaviour)}".ToKebabCase(), true);
            public static EffectId ParalysingGraspApplyAttackBehaviour => new EffectId($"{nameof(Parasite)}{nameof(ParalysingGraspApplyAttackBehaviour)}".ToKebabCase(), true);
            public static EffectId ParalysingGraspApplySelfBehaviour => new EffectId($"{nameof(Parasite)}{nameof(ParalysingGraspApplySelfBehaviour)}".ToKebabCase(), true);
        }

        public static class Horrior
        {
            public static EffectId ExpertFormationSearch => new EffectId($"{nameof(Horrior)}{nameof(ExpertFormationSearch)}".ToKebabCase(), true);
            public static EffectId ExpertFormationApplyBehaviour => new EffectId($"{nameof(Horrior)}{nameof(ExpertFormationApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId MountApplyBehaviour => new EffectId($"{nameof(Horrior)}{nameof(MountApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId MountCreateEntity => new EffectId($"{nameof(Horrior)}{nameof(MountCreateEntity)}".ToKebabCase(), true);
            public static EffectId MountDestroy => new EffectId($"{nameof(Horrior)}{nameof(MountDestroy)}".ToKebabCase(), true);
        }

        public static class Marksman
        {
            public static EffectId CriticalMarkApplyBehaviour => new EffectId($"{nameof(Marksman)}{nameof(CriticalMarkApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId CriticalMarkDamage => new EffectId($"{nameof(Marksman)}{nameof(CriticalMarkDamage)}".ToKebabCase(), true);
        }

        public static class Surfer
        {
            public static EffectId DismountApplyBehaviour => new EffectId($"{nameof(Surfer)}{nameof(DismountApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId DismountCreateEntity => new EffectId($"{nameof(Surfer)}{nameof(DismountCreateEntity)}".ToKebabCase(), true);
        }
        
        public static class Mortar
        {
            public static EffectId DeadlyAmmunitionApplyBehaviour => new EffectId($"{nameof(Mortar)}{nameof(DeadlyAmmunitionApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId DeadlyAmmunitionSearch => new EffectId($"{nameof(Mortar)}{nameof(DeadlyAmmunitionSearch)}".ToKebabCase(), true);
            public static EffectId DeadlyAmmunitionDamage => new EffectId($"{nameof(Mortar)}{nameof(DeadlyAmmunitionDamage)}".ToKebabCase(), true);
            public static EffectId ReloadApplyBehaviour => new EffectId($"{nameof(Mortar)}{nameof(ReloadApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId ReloadReload => new EffectId($"{nameof(Mortar)}{nameof(ReloadReload)}".ToKebabCase(), true);
            public static EffectId PiercingBlastApplyBehaviour => new EffectId($"{nameof(Mortar)}{nameof(PiercingBlastApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Hawk
        {
            public static EffectId TacticalGogglesApplyBehaviour => new EffectId($"{nameof(Hawk)}{nameof(TacticalGogglesApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId LeadershipApplyBehaviour => new EffectId($"{nameof(Hawk)}{nameof(LeadershipApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId HealthKitApplyBehaviour => new EffectId($"{nameof(Hawk)}{nameof(HealthKitApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId HealthKitSearch => new EffectId($"{nameof(Hawk)}{nameof(HealthKitSearch)}".ToKebabCase(), true);
            public static EffectId HealthKitHealApplyBehaviour => new EffectId($"{nameof(Hawk)}{nameof(HealthKitHealApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Engineer
        {
            public static EffectId OperateApplyBehaviour => new EffectId($"{nameof(Engineer)}{nameof(OperateApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId OperateModifyCounter => new EffectId($"{nameof(Engineer)}{nameof(OperateModifyCounter)}".ToKebabCase(), true);
            public static EffectId OperateDestroy => new EffectId($"{nameof(Engineer)}{nameof(OperateDestroy)}".ToKebabCase(), true);
            public static EffectId RepairStructureApplyBehaviour => new EffectId($"{nameof(Engineer)}{nameof(RepairStructureApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId RepairMachineApplyBehaviour => new EffectId($"{nameof(Engineer)}{nameof(RepairMachineApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId RepairHorriorApplyBehaviour => new EffectId($"{nameof(Engineer)}{nameof(RepairHorriorApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId RepairApplyBehaviourSelf => new EffectId($"{nameof(Engineer)}{nameof(RepairApplyBehaviourSelf)}".ToKebabCase(), true);
        }
        
        public static class Cannon
        {
            public static EffectId MachineApplyBehaviour => new EffectId($"{nameof(Cannon)}{nameof(MachineApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId MachineRemoveBehaviour => new EffectId($"{nameof(Cannon)}{nameof(MachineRemoveBehaviour)}".ToKebabCase(), true);
            public static EffectId HeatUpCreateEntity => new EffectId($"{nameof(Cannon)}{nameof(HeatUpCreateEntity)}".ToKebabCase(), true);
            public static EffectId HeatUpApplyWaitBehaviour => new EffectId($"{nameof(Cannon)}{nameof(HeatUpApplyWaitBehaviour)}".ToKebabCase(), true);
            public static EffectId HeatUpSearch => new EffectId($"{nameof(Cannon)}{nameof(HeatUpSearch)}".ToKebabCase(), true);
            public static EffectId HeatUpDamage => new EffectId($"{nameof(Cannon)}{nameof(HeatUpDamage)}".ToKebabCase(), true);
            public static EffectId HeatUpDestroy => new EffectId($"{nameof(Cannon)}{nameof(HeatUpDestroy)}".ToKebabCase(), true);
            public static EffectId HeatUpRemoveBehaviour => new EffectId($"{nameof(Cannon)}{nameof(HeatUpRemoveBehaviour)}".ToKebabCase(), true);
        }

        public static class Ballista
        {
            public static EffectId MachineApplyBehaviour => new EffectId($"{nameof(Ballista)}{nameof(MachineApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId MachineRemoveBehaviour => new EffectId($"{nameof(Ballista)}{nameof(MachineRemoveBehaviour)}".ToKebabCase(), true);
            public static EffectId AimDamage => new EffectId($"{nameof(Ballista)}{nameof(AimDamage)}".ToKebabCase(), true);
            public static EffectId AimApplyBehaviour => new EffectId($"{nameof(Ballista)}{nameof(AimApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId AimSearch => new EffectId($"{nameof(Ballista)}{nameof(AimSearch)}".ToKebabCase(), true);
        }
        
        public static class Radar
        {
            public static EffectId MachineApplyBehaviour => new EffectId($"{nameof(Radar)}{nameof(MachineApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId MachineRemoveBehaviour => new EffectId($"{nameof(Radar)}{nameof(MachineRemoveBehaviour)}".ToKebabCase(), true);
            public static EffectId ResonatingSweepCreateEntity => new EffectId($"{nameof(Radar)}{nameof(ResonatingSweepCreateEntity)}".ToKebabCase(), true);
            public static EffectId ResonatingSweepDestroy => new EffectId($"{nameof(Radar)}{nameof(ResonatingSweepDestroy)}".ToKebabCase(), true);
            public static EffectId RadioLocationApplyBehaviour => new EffectId($"{nameof(Radar)}{nameof(RadioLocationApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId RadioLocationSearchDestroy => new EffectId($"{nameof(Radar)}{nameof(RadioLocationSearchDestroy)}".ToKebabCase(), true);
            public static EffectId RadioLocationDestroy => new EffectId($"{nameof(Radar)}{nameof(RadioLocationDestroy)}".ToKebabCase(), true);
            public static EffectId RadioLocationSearchCreate => new EffectId($"{nameof(Radar)}{nameof(RadioLocationSearchCreate)}".ToKebabCase(), true);
            public static EffectId RadioLocationCreateEntity => new EffectId($"{nameof(Radar)}{nameof(RadioLocationCreateEntity)}".ToKebabCase(), true);
        }

        public static class Vessel
        {
            public static EffectId MachineApplyBehaviour => new EffectId($"{nameof(Vessel)}{nameof(MachineApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId MachineRemoveBehaviour => new EffectId($"{nameof(Vessel)}{nameof(MachineRemoveBehaviour)}".ToKebabCase(), true);
            public static EffectId AbsorbentFieldSearch => new EffectId($"{nameof(Vessel)}{nameof(AbsorbentFieldSearch)}".ToKebabCase(), true);
            public static EffectId AbsorbentFieldApplyBehaviour => new EffectId($"{nameof(Vessel)}{nameof(AbsorbentFieldApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId FortifyCreateEntity => new EffectId($"{nameof(Vessel)}{nameof(FortifyCreateEntity)}".ToKebabCase(), true);
            public static EffectId FortifyDestroy => new EffectId($"{nameof(Vessel)}{nameof(FortifyDestroy)}".ToKebabCase(), true);
            public static EffectId FortifySearch => new EffectId($"{nameof(Vessel)}{nameof(FortifySearch)}".ToKebabCase(), true);
            public static EffectId FortifyApplyBehaviour => new EffectId($"{nameof(Vessel)}{nameof(FortifyApplyBehaviour)}".ToKebabCase(), true);
        }

        public static class Omen
        {
            public static EffectId RenditionPlacementApplyBehaviour => new EffectId($"{nameof(Omen)}{nameof(RenditionPlacementApplyBehaviour)}".ToKebabCase(), true);
            public static EffectId RenditionPlacementExecuteAbility => new EffectId($"{nameof(Omen)}{nameof(RenditionPlacementExecuteAbility)}".ToKebabCase(), true);
            public static EffectId RenditionPlacementCreateEntity => new EffectId($"{nameof(Omen)}{nameof(RenditionPlacementCreateEntity)}".ToKebabCase(), true);
            public static EffectId RenditionDestroy => new EffectId($"{nameof(Omen)}{nameof(RenditionDestroy)}".ToKebabCase(), true);
            public static EffectId RenditionSearch => new EffectId($"{nameof(Omen)}{nameof(RenditionSearch)}".ToKebabCase(), true);
            public static EffectId RenditionDamage => new EffectId($"{nameof(Omen)}{nameof(RenditionDamage)}".ToKebabCase(), true);
            public static EffectId RenditionApplyBehaviourSlow => new EffectId($"{nameof(Omen)}{nameof(RenditionApplyBehaviourSlow)}".ToKebabCase(), true);
        }
        
        private class EffectIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(EffectId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (EffectId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new EffectId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
