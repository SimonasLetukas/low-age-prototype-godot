using low_age_data.Common;

namespace low_age_data.Domain.Effects
{
    public class EffectId : Id
    {
        private EffectId(string value) : base($"effect-{value}")
        {
        }

        public static class Shared
        {
            public static EffectId HighGroundSearch => new EffectId($"{nameof(Shared)}{nameof(HighGroundSearch)}".ToKebabCase());
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
            public static EffectId PassiveIncomeApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(PassiveIncomeApplyBehaviour)}".ToKebabCase());
            public static EffectId ScrapsIncomeApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(ScrapsIncomeApplyBehaviour)}".ToKebabCase());
            public static EffectId CelestiumIncomeApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(CelestiumIncomeApplyBehaviour)}".ToKebabCase());

            public static class Revelators
            {
                public static EffectId NoPopulationSpaceSearch => new EffectId($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceSearch)}".ToKebabCase());
                public static EffectId NoPopulationSpaceApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceApplyBehaviour)}".ToKebabCase());
            }

            public static class Uee
            {
                public static EffectId PowerGeneratorApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorApplyBehaviour)}".ToKebabCase());
                public static EffectId PowerGeneratorModifyPlayer => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorModifyPlayer)}".ToKebabCase());
                public static EffectId PowerDependencyApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviour)}".ToKebabCase());
                public static EffectId PowerDependencyDamage => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyDamage)}".ToKebabCase());
                public static EffectId PowerDependencyApplyBehaviourDisable => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviourDisable)}".ToKebabCase());
                public static EffectId PowerDependencyApplyBehaviourInactive => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviourInactive)}".ToKebabCase());
                public static EffectId PositiveFaithSearch => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithSearch)}".ToKebabCase());
                public static EffectId PositiveFaithApplyBehaviour => new EffectId($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithApplyBehaviour)}".ToKebabCase());
            }
        }
        
        public static class Citadel
        {
            public static EffectId ExecutiveStashApplyBehaviour => new EffectId($"{nameof(Citadel)}{nameof(ExecutiveStashApplyBehaviour)}".ToKebabCase());
            public static EffectId AscendableApplyBehaviour => new EffectId($"{nameof(Citadel)}{nameof(AscendableApplyBehaviour)}".ToKebabCase());
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Citadel)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
        }

        public static class Hut
        {
        }
        
        public static class Obelisk
        {
            public static EffectId CelestiumDischargeSearchLong => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeSearchLong)}".ToKebabCase());
            public static EffectId CelestiumDischargeApplyBehaviourLong => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourLong)}".ToKebabCase());
            public static EffectId CelestiumDischargeSearchShort => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeSearchShort)}".ToKebabCase());
            public static EffectId CelestiumDischargeApplyBehaviourShort => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourShort)}".ToKebabCase());
            public static EffectId CelestiumDischargeApplyBehaviourNegative => new EffectId($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourNegative)}".ToKebabCase());
        }

        public static class Shack
        {
            public static EffectId AccommodationApplyBehaviour => new EffectId($"{nameof(Shack)}{nameof(AccommodationApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Smith
        {
            public static EffectId MeleeWeaponProductionApplyBehaviour => new EffectId($"{nameof(Smith)}{nameof(MeleeWeaponProductionApplyBehaviour)}".ToKebabCase());
        }

        public static class Fletcher
        {
            public static EffectId RangedWeaponProductionApplyBehaviour => new EffectId($"{nameof(Fletcher)}{nameof(RangedWeaponProductionApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Alchemy
        {
            public static EffectId SpecialWeaponProductionApplyBehaviour => new EffectId($"{nameof(Alchemy)}{nameof(SpecialWeaponProductionApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Depot
        {
            public static EffectId WeaponStorageApplyBehaviour => new EffectId($"{nameof(Depot)}{nameof(WeaponStorageApplyBehaviour)}".ToKebabCase());
        }

        public static class Workshop
        {
        }

        public static class Outpost
        {
            public static EffectId AscendableApplyBehaviour => new EffectId($"{nameof(Outpost)}{nameof(AscendableApplyBehaviour)}".ToKebabCase());
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Outpost)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
        }

        public static class Barricade
        {
            public static EffectId ProtectiveShieldSearch => new EffectId($"{nameof(Barricade)}{nameof(ProtectiveShieldSearch)}".ToKebabCase());
            public static EffectId ProtectiveShieldApplyBehaviour => new EffectId($"{nameof(Barricade)}{nameof(ProtectiveShieldApplyBehaviour)}".ToKebabCase());
            public static EffectId CaltropsSearch => new EffectId($"{nameof(Barricade)}{nameof(CaltropsSearch)}".ToKebabCase());
            public static EffectId CaltropsDamage => new EffectId($"{nameof(Barricade)}{nameof(CaltropsDamage)}".ToKebabCase());
            public static EffectId DecomposeApplyBehaviour => new EffectId($"{nameof(Barricade)}{nameof(DecomposeApplyBehaviour)}".ToKebabCase());
            public static EffectId DecomposeRemoveBehaviour => new EffectId($"{nameof(Barricade)}{nameof(DecomposeRemoveBehaviour)}".ToKebabCase());
            public static EffectId DecomposeDamage => new EffectId($"{nameof(Barricade)}{nameof(DecomposeDamage)}".ToKebabCase());
        }
        
        public static class BatteryCore
        {
            public static EffectId PowerGridApplyBehaviour => new EffectId($"{nameof(BatteryCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase());
            public static EffectId FusionCoreUpgradeApplyBehaviour => new EffectId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeApplyBehaviour)}".ToKebabCase());
            public static EffectId FusionCoreUpgradeCreateEntity => new EffectId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeCreateEntity)}".ToKebabCase());
            public static EffectId FusionCoreUpgradeDestroy => new EffectId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeDestroy)}".ToKebabCase());
            public static EffectId FusionCoreUpgradeModifyResearch => new EffectId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeModifyResearch)}".ToKebabCase());
        }
        
        public static class FusionCore
        {
            public static EffectId PowerGridApplyBehaviour => new EffectId($"{nameof(FusionCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase());
            public static EffectId DefenceProtocolDamage => new EffectId($"{nameof(FusionCore)}{nameof(DefenceProtocolDamage)}".ToKebabCase());
            public static EffectId CelestiumCoreUpgradeApplyBehaviour => new EffectId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeApplyBehaviour)}".ToKebabCase());
            public static EffectId CelestiumCoreUpgradeCreateEntity => new EffectId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeCreateEntity)}".ToKebabCase());
            public static EffectId CelestiumCoreUpgradeDestroy => new EffectId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeDestroy)}".ToKebabCase());
            public static EffectId CelestiumCoreUpgradeModifyResearch => new EffectId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeModifyResearch)}".ToKebabCase());
        }
        
        public static class CelestiumCore
        {
            public static EffectId PowerGridApplyBehaviour => new EffectId($"{nameof(CelestiumCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase());
            public static EffectId DefenceProtocolDamage => new EffectId($"{nameof(CelestiumCore)}{nameof(DefenceProtocolDamage)}".ToKebabCase());
            public static EffectId HeightenedConductivityModifyResearch => new EffectId($"{nameof(CelestiumCore)}{nameof(HeightenedConductivityModifyResearch)}".ToKebabCase());
        }
        
        public static class Collector
        {
            public static EffectId DirectTransitSystemApplyBehaviourInactive => new EffectId($"{nameof(Collector)}{nameof(DirectTransitSystemApplyBehaviourInactive)}".ToKebabCase());
            public static EffectId DirectTransitSystemApplyBehaviourActive => new EffectId($"{nameof(Collector)}{nameof(DirectTransitSystemApplyBehaviourActive)}".ToKebabCase());
        }
        
        public static class Extractor
        {
            public static EffectId ReinforcedInfrastructureApplyBehaviourInactive => new EffectId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureApplyBehaviourInactive)}".ToKebabCase());
            public static EffectId ReinforcedInfrastructureApplyBehaviourActive => new EffectId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureApplyBehaviourActive)}".ToKebabCase());
        }
        
        public static class PowerPole
        {
            public static EffectId PowerGridApplyBehaviour => new EffectId($"{nameof(PowerPole)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase());
            public static EffectId ExcessDistributionSearch => new EffectId($"{nameof(PowerPole)}{nameof(ExcessDistributionSearch)}".ToKebabCase());
            public static EffectId ExcessDistributionApplyBehaviour => new EffectId($"{nameof(PowerPole)}{nameof(ExcessDistributionApplyBehaviour)}".ToKebabCase());
            public static EffectId ImprovedPowerGridModifyAbilityPowerGrid => new EffectId($"{nameof(PowerPole)}{nameof(ImprovedPowerGridModifyAbilityPowerGrid)}".ToKebabCase());
            public static EffectId ImprovedPowerGridModifyAbilityExcessDistribution => new EffectId($"{nameof(PowerPole)}{nameof(ImprovedPowerGridModifyAbilityExcessDistribution)}".ToKebabCase());
            public static EffectId PowerGridImprovedApplyBehaviour => new EffectId($"{nameof(PowerPole)}{nameof(PowerGridImprovedApplyBehaviour)}".ToKebabCase());
            public static EffectId ExcessDistributionImprovedSearch => new EffectId($"{nameof(PowerPole)}{nameof(ExcessDistributionImprovedSearch)}".ToKebabCase());
        }
        
        public static class Temple
        {
            public static EffectId KeepingTheFaithSearch => new EffectId($"{nameof(Temple)}{nameof(KeepingTheFaithSearch)}".ToKebabCase());
            public static EffectId KeepingTheFaithApplyBehaviourBuff => new EffectId($"{nameof(Temple)}{nameof(KeepingTheFaithApplyBehaviourBuff)}".ToKebabCase());
            public static EffectId KeepingTheFaithApplyBehaviourIncome => new EffectId($"{nameof(Temple)}{nameof(KeepingTheFaithApplyBehaviourIncome)}".ToKebabCase());
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
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Wall)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Stairs
        {
            public static EffectId AscendableApplyBehaviour => new EffectId($"{nameof(Stairs)}{nameof(AscendableApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Gate
        {
            public static EffectId HighGroundApplyBehaviour => new EffectId($"{nameof(Gate)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
            public static EffectId AscendableApplyBehaviour => new EffectId($"{nameof(Gate)}{nameof(AscendableApplyBehaviour)}".ToKebabCase());
            public static EffectId EntranceApplyBehaviour => new EffectId($"{nameof(Gate)}{nameof(EntranceApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Watchtower
        {
            public static EffectId VantagePointSearch => new EffectId($"{nameof(Watchtower)}{nameof(VantagePointSearch)}".ToKebabCase());
            public static EffectId VantagePointApplyBehaviour => new EffectId($"{nameof(Watchtower)}{nameof(VantagePointApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Bastion
        {
            public static EffectId BattlementSearch => new EffectId($"{nameof(Bastion)}{nameof(BattlementSearch)}".ToKebabCase());
            public static EffectId BattlementApplyBehaviour => new EffectId($"{nameof(Bastion)}{nameof(BattlementApplyBehaviour)}".ToKebabCase());
        }

        public static class Leader
        {
            public static EffectId AllForOneApplyBehaviour => new EffectId($"{nameof(Leader)}{nameof(AllForOneApplyBehaviour)}".ToKebabCase());
            public static EffectId AllForOneModifyPlayer => new EffectId($"{nameof(Leader)}{nameof(AllForOneModifyPlayer)}".ToKebabCase());
            public static EffectId MenacingPresenceSearch => new EffectId($"{nameof(Leader)}{nameof(MenacingPresenceSearch)}".ToKebabCase());
            public static EffectId MenacingPresenceApplyBehaviour => new EffectId($"{nameof(Leader)}{nameof(MenacingPresenceApplyBehaviour)}".ToKebabCase());
            public static EffectId OneForAllApplyBehaviourObelisk => new EffectId($"{nameof(Leader)}{nameof(OneForAllApplyBehaviourObelisk)}".ToKebabCase());
            public static EffectId OneForAllSearch => new EffectId($"{nameof(Leader)}{nameof(OneForAllSearch)}".ToKebabCase());
            public static EffectId OneForAllApplyBehaviourHeal => new EffectId($"{nameof(Leader)}{nameof(OneForAllApplyBehaviourHeal)}".ToKebabCase());
        }

        public static class Slave
        {
            public static EffectId RepairApplyBehaviourStructure => new EffectId($"{nameof(Slave)}{nameof(RepairApplyBehaviourStructure)}".ToKebabCase());
            public static EffectId RepairApplyBehaviourSelf => new EffectId($"{nameof(Slave)}{nameof(RepairApplyBehaviourSelf)}".ToKebabCase());
            public static EffectId ManualLabourApplyBehaviourHut => new EffectId($"{nameof(Slave)}{nameof(ManualLabourApplyBehaviourHut)}".ToKebabCase());
            public static EffectId ManualLabourApplyBehaviourSelf => new EffectId($"{nameof(Slave)}{nameof(ManualLabourApplyBehaviourSelf)}".ToKebabCase());
            public static EffectId ManualLabourModifyPlayer => new EffectId($"{nameof(Slave)}{nameof(ManualLabourModifyPlayer)}".ToKebabCase());
        }

        public static class Quickdraw
        {
            public static EffectId DoubleshotApplyBehaviour => new EffectId($"{nameof(Quickdraw)}{nameof(DoubleshotApplyBehaviour)}".ToKebabCase());
            public static EffectId CrippleApplyBehaviour => new EffectId($"{nameof(Quickdraw)}{nameof(CrippleApplyBehaviour)}".ToKebabCase());
        }

        public static class Gorger
        {
            public static EffectId FanaticSuicideApplyBehaviourBuff => new EffectId($"{nameof(Gorger)}{nameof(FanaticSuicideApplyBehaviourBuff)}".ToKebabCase());
            public static EffectId FanaticSuicideDestroy => new EffectId($"{nameof(Gorger)}{nameof(FanaticSuicideDestroy)}".ToKebabCase());
            public static EffectId FanaticSuicideSearch => new EffectId($"{nameof(Gorger)}{nameof(FanaticSuicideSearch)}".ToKebabCase());
            public static EffectId FanaticSuicideDamage => new EffectId($"{nameof(Gorger)}{nameof(FanaticSuicideDamage)}".ToKebabCase());
        }

        public static class Camou
        {
            public static EffectId SilentAssassinOnHitDamage => new EffectId($"{nameof(Camou)}{nameof(SilentAssassinOnHitDamage)}".ToKebabCase());
            public static EffectId SilentAssassinOnHitSilence => new EffectId($"{nameof(Camou)}{nameof(SilentAssassinOnHitSilence)}".ToKebabCase());
            public static EffectId SilentAssassinSearchFriendly => new EffectId($"{nameof(Camou)}{nameof(SilentAssassinSearchFriendly)}".ToKebabCase());
            public static EffectId SilentAssassinSearchEnemy => new EffectId($"{nameof(Camou)}{nameof(SilentAssassinSearchEnemy)}".ToKebabCase());
            public static EffectId ClimbTeleport => new EffectId($"{nameof(Camou)}{nameof(ClimbTeleport)}".ToKebabCase());
            public static EffectId ClimbApplyBehaviour => new EffectId($"{nameof(Camou)}{nameof(ClimbApplyBehaviour)}".ToKebabCase());
        }

        public static class Shaman
        {
            public static EffectId WondrousGooCreateEntity => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooCreateEntity)}".ToKebabCase());
            public static EffectId WondrousGooSearch => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooSearch)}".ToKebabCase());
            public static EffectId WondrousGooApplyBehaviour => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooApplyBehaviour)}".ToKebabCase());
            public static EffectId WondrousGooDestroy => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooDestroy)}".ToKebabCase());
            public static EffectId WondrousGooDamage => new EffectId($"{nameof(Shaman)}{nameof(WondrousGooDamage)}".ToKebabCase());
        }

        public static class Pyre
        {
            public static EffectId CargoCreateEntity => new EffectId($"{nameof(Pyre)}{nameof(CargoCreateEntity)}".ToKebabCase());
            public static EffectId WallOfFlamesCreateEntity => new EffectId($"{nameof(Pyre)}{nameof(WallOfFlamesCreateEntity)}".ToKebabCase());
            public static EffectId WallOfFlamesDestroy => new EffectId($"{nameof(Pyre)}{nameof(WallOfFlamesDestroy)}".ToKebabCase());
            public static EffectId WallOfFlamesDamage => new EffectId($"{nameof(Pyre)}{nameof(WallOfFlamesDamage)}".ToKebabCase());
            public static EffectId PhantomMenaceApplyBehaviour => new EffectId($"{nameof(Pyre)}{nameof(PhantomMenaceApplyBehaviour)}".ToKebabCase());
        }

        public static class BigBadBull
        {
            public static EffectId UnleashTheRageSearch => new EffectId($"{nameof(BigBadBull)}{nameof(UnleashTheRageSearch)}".ToKebabCase());
            public static EffectId UnleashTheRageDamage => new EffectId($"{nameof(BigBadBull)}{nameof(UnleashTheRageDamage)}".ToKebabCase());
            public static EffectId UnleashTheRageForce => new EffectId($"{nameof(BigBadBull)}{nameof(UnleashTheRageForce)}".ToKebabCase());
            public static EffectId UnleashTheRageForceDamage => new EffectId($"{nameof(BigBadBull)}{nameof(UnleashTheRageForceDamage)}".ToKebabCase());
        }

        public static class Mummy
        {
            public static EffectId SpawnRoachCreateEntity => new EffectId($"{nameof(Mummy)}{nameof(SpawnRoachCreateEntity)}".ToKebabCase());
            public static EffectId LeapOfHungerModifyAbility => new EffectId($"{nameof(Mummy)}{nameof(LeapOfHungerModifyAbility)}".ToKebabCase());
        }

        public static class Roach
        {
            public static EffectId DegradingCarapaceApplyBehaviour => new EffectId($"{nameof(Roach)}{nameof(DegradingCarapaceApplyBehaviour)}".ToKebabCase());
            public static EffectId DegradingCarapacePeriodicApplyBehaviour => new EffectId($"{nameof(Roach)}{nameof(DegradingCarapacePeriodicApplyBehaviour)}".ToKebabCase());
            public static EffectId DegradingCarapaceSelfDamage => new EffectId($"{nameof(Roach)}{nameof(DegradingCarapaceSelfDamage)}".ToKebabCase());
            public static EffectId CorrosiveSpitDamage => new EffectId($"{nameof(Roach)}{nameof(CorrosiveSpitDamage)}".ToKebabCase());
        }

        public static class Parasite
        {
            public static EffectId ParalysingGraspApplyTetherBehaviour => new EffectId($"{nameof(Parasite)}{nameof(ParalysingGraspApplyTetherBehaviour)}".ToKebabCase());
            public static EffectId ParalysingGraspApplyAttackBehaviour => new EffectId($"{nameof(Parasite)}{nameof(ParalysingGraspApplyAttackBehaviour)}".ToKebabCase());
            public static EffectId ParalysingGraspApplySelfBehaviour => new EffectId($"{nameof(Parasite)}{nameof(ParalysingGraspApplySelfBehaviour)}".ToKebabCase());
        }

        public static class Horrior
        {
            public static EffectId ExpertFormationSearch => new EffectId($"{nameof(Horrior)}{nameof(ExpertFormationSearch)}".ToKebabCase());
            public static EffectId ExpertFormationApplyBehaviour => new EffectId($"{nameof(Horrior)}{nameof(ExpertFormationApplyBehaviour)}".ToKebabCase());
            public static EffectId MountApplyBehaviour => new EffectId($"{nameof(Horrior)}{nameof(MountApplyBehaviour)}".ToKebabCase());
            public static EffectId MountCreateEntity => new EffectId($"{nameof(Horrior)}{nameof(MountCreateEntity)}".ToKebabCase());
            public static EffectId MountDestroy => new EffectId($"{nameof(Horrior)}{nameof(MountDestroy)}".ToKebabCase());
        }

        public static class Marksman
        {
            public static EffectId CriticalMarkApplyBehaviour => new EffectId($"{nameof(Marksman)}{nameof(CriticalMarkApplyBehaviour)}".ToKebabCase());
            public static EffectId CriticalMarkDamage => new EffectId($"{nameof(Marksman)}{nameof(CriticalMarkDamage)}".ToKebabCase());
        }

        public static class Surfer
        {
            public static EffectId DismountApplyBehaviour => new EffectId($"{nameof(Surfer)}{nameof(DismountApplyBehaviour)}".ToKebabCase());
            public static EffectId DismountCreateEntity => new EffectId($"{nameof(Surfer)}{nameof(DismountCreateEntity)}".ToKebabCase());
        }
        
        public static class Mortar
        {
            public static EffectId DeadlyAmmunitionApplyBehaviour => new EffectId($"{nameof(Mortar)}{nameof(DeadlyAmmunitionApplyBehaviour)}".ToKebabCase());
            public static EffectId DeadlyAmmunitionSearch => new EffectId($"{nameof(Mortar)}{nameof(DeadlyAmmunitionSearch)}".ToKebabCase());
            public static EffectId DeadlyAmmunitionDamage => new EffectId($"{nameof(Mortar)}{nameof(DeadlyAmmunitionDamage)}".ToKebabCase());
            public static EffectId ReloadApplyBehaviour => new EffectId($"{nameof(Mortar)}{nameof(ReloadApplyBehaviour)}".ToKebabCase());
            public static EffectId ReloadReload => new EffectId($"{nameof(Mortar)}{nameof(ReloadReload)}".ToKebabCase());
            public static EffectId PiercingBlastApplyBehaviour => new EffectId($"{nameof(Mortar)}{nameof(PiercingBlastApplyBehaviour)}".ToKebabCase());
        }

        public static class Hawk
        {
            public static EffectId TacticalGogglesApplyBehaviour => new EffectId($"{nameof(Hawk)}{nameof(TacticalGogglesApplyBehaviour)}".ToKebabCase());
            public static EffectId LeadershipApplyBehaviour => new EffectId($"{nameof(Hawk)}{nameof(LeadershipApplyBehaviour)}".ToKebabCase());
            public static EffectId HealthKitApplyBehaviour => new EffectId($"{nameof(Hawk)}{nameof(HealthKitApplyBehaviour)}".ToKebabCase());
            public static EffectId HealthKitSearch => new EffectId($"{nameof(Hawk)}{nameof(HealthKitSearch)}".ToKebabCase());
            public static EffectId HealthKitHealApplyBehaviour => new EffectId($"{nameof(Hawk)}{nameof(HealthKitHealApplyBehaviour)}".ToKebabCase());
        }

        public static class Engineer
        {
            public static EffectId OperateApplyBehaviour => new EffectId($"{nameof(Engineer)}{nameof(OperateApplyBehaviour)}".ToKebabCase());
            public static EffectId OperateModifyCounter => new EffectId($"{nameof(Engineer)}{nameof(OperateModifyCounter)}".ToKebabCase());
            public static EffectId OperateDestroy => new EffectId($"{nameof(Engineer)}{nameof(OperateDestroy)}".ToKebabCase());
            public static EffectId RepairStructureApplyBehaviour => new EffectId($"{nameof(Engineer)}{nameof(RepairStructureApplyBehaviour)}".ToKebabCase());
            public static EffectId RepairMachineApplyBehaviour => new EffectId($"{nameof(Engineer)}{nameof(RepairMachineApplyBehaviour)}".ToKebabCase());
            public static EffectId RepairHorriorApplyBehaviour => new EffectId($"{nameof(Engineer)}{nameof(RepairHorriorApplyBehaviour)}".ToKebabCase());
            public static EffectId RepairApplyBehaviourSelf => new EffectId($"{nameof(Engineer)}{nameof(RepairApplyBehaviourSelf)}".ToKebabCase());
        }
        
        public static class Cannon
        {
            public static EffectId MachineApplyBehaviour => new EffectId($"{nameof(Cannon)}{nameof(MachineApplyBehaviour)}".ToKebabCase());
            public static EffectId MachineRemoveBehaviour => new EffectId($"{nameof(Cannon)}{nameof(MachineRemoveBehaviour)}".ToKebabCase());
            public static EffectId HeatUpCreateEntity => new EffectId($"{nameof(Cannon)}{nameof(HeatUpCreateEntity)}".ToKebabCase());
            public static EffectId HeatUpApplyWaitBehaviour => new EffectId($"{nameof(Cannon)}{nameof(HeatUpApplyWaitBehaviour)}".ToKebabCase());
            public static EffectId HeatUpSearch => new EffectId($"{nameof(Cannon)}{nameof(HeatUpSearch)}".ToKebabCase());
            public static EffectId HeatUpDamage => new EffectId($"{nameof(Cannon)}{nameof(HeatUpDamage)}".ToKebabCase());
            public static EffectId HeatUpDestroy => new EffectId($"{nameof(Cannon)}{nameof(HeatUpDestroy)}".ToKebabCase());
            public static EffectId HeatUpRemoveBehaviour => new EffectId($"{nameof(Cannon)}{nameof(HeatUpRemoveBehaviour)}".ToKebabCase());
        }

        public static class Ballista
        {
            public static EffectId MachineApplyBehaviour => new EffectId($"{nameof(Ballista)}{nameof(MachineApplyBehaviour)}".ToKebabCase());
            public static EffectId MachineRemoveBehaviour => new EffectId($"{nameof(Ballista)}{nameof(MachineRemoveBehaviour)}".ToKebabCase());
            public static EffectId AimDamage => new EffectId($"{nameof(Ballista)}{nameof(AimDamage)}".ToKebabCase());
            public static EffectId AimApplyBehaviour => new EffectId($"{nameof(Ballista)}{nameof(AimApplyBehaviour)}".ToKebabCase());
            public static EffectId AimSearch => new EffectId($"{nameof(Ballista)}{nameof(AimSearch)}".ToKebabCase());
        }
        
        public static class Radar
        {
            public static EffectId MachineApplyBehaviour => new EffectId($"{nameof(Radar)}{nameof(MachineApplyBehaviour)}".ToKebabCase());
            public static EffectId MachineRemoveBehaviour => new EffectId($"{nameof(Radar)}{nameof(MachineRemoveBehaviour)}".ToKebabCase());
            public static EffectId ResonatingSweepCreateEntity => new EffectId($"{nameof(Radar)}{nameof(ResonatingSweepCreateEntity)}".ToKebabCase());
            public static EffectId ResonatingSweepDestroy => new EffectId($"{nameof(Radar)}{nameof(ResonatingSweepDestroy)}".ToKebabCase());
            public static EffectId RadioLocationApplyBehaviour => new EffectId($"{nameof(Radar)}{nameof(RadioLocationApplyBehaviour)}".ToKebabCase());
            public static EffectId RadioLocationSearchDestroy => new EffectId($"{nameof(Radar)}{nameof(RadioLocationSearchDestroy)}".ToKebabCase());
            public static EffectId RadioLocationDestroy => new EffectId($"{nameof(Radar)}{nameof(RadioLocationDestroy)}".ToKebabCase());
            public static EffectId RadioLocationSearchCreate => new EffectId($"{nameof(Radar)}{nameof(RadioLocationSearchCreate)}".ToKebabCase());
            public static EffectId RadioLocationCreateEntity => new EffectId($"{nameof(Radar)}{nameof(RadioLocationCreateEntity)}".ToKebabCase());
        }

        public static class Vessel
        {
            public static EffectId MachineApplyBehaviour => new EffectId($"{nameof(Vessel)}{nameof(MachineApplyBehaviour)}".ToKebabCase());
            public static EffectId MachineRemoveBehaviour => new EffectId($"{nameof(Vessel)}{nameof(MachineRemoveBehaviour)}".ToKebabCase());
            public static EffectId AbsorbentFieldSearch => new EffectId($"{nameof(Vessel)}{nameof(AbsorbentFieldSearch)}".ToKebabCase());
            public static EffectId AbsorbentFieldApplyBehaviour => new EffectId($"{nameof(Vessel)}{nameof(AbsorbentFieldApplyBehaviour)}".ToKebabCase());
            public static EffectId FortifyCreateEntity => new EffectId($"{nameof(Vessel)}{nameof(FortifyCreateEntity)}".ToKebabCase());
            public static EffectId FortifyDestroy => new EffectId($"{nameof(Vessel)}{nameof(FortifyDestroy)}".ToKebabCase());
            public static EffectId FortifySearch => new EffectId($"{nameof(Vessel)}{nameof(FortifySearch)}".ToKebabCase());
            public static EffectId FortifyApplyBehaviour => new EffectId($"{nameof(Vessel)}{nameof(FortifyApplyBehaviour)}".ToKebabCase());
        }

        public static class Omen
        {
            public static EffectId RenditionPlacementApplyBehaviour => new EffectId($"{nameof(Omen)}{nameof(RenditionPlacementApplyBehaviour)}".ToKebabCase());
            public static EffectId RenditionPlacementExecuteAbility => new EffectId($"{nameof(Omen)}{nameof(RenditionPlacementExecuteAbility)}".ToKebabCase());
            public static EffectId RenditionPlacementCreateEntity => new EffectId($"{nameof(Omen)}{nameof(RenditionPlacementCreateEntity)}".ToKebabCase());
            public static EffectId RenditionDestroy => new EffectId($"{nameof(Omen)}{nameof(RenditionDestroy)}".ToKebabCase());
            public static EffectId RenditionSearch => new EffectId($"{nameof(Omen)}{nameof(RenditionSearch)}".ToKebabCase());
            public static EffectId RenditionDamage => new EffectId($"{nameof(Omen)}{nameof(RenditionDamage)}".ToKebabCase());
            public static EffectId RenditionApplyBehaviourSlow => new EffectId($"{nameof(Omen)}{nameof(RenditionApplyBehaviourSlow)}".ToKebabCase());
        }
    }
}
