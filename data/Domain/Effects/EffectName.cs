using low_age_data.Common;

namespace low_age_data.Domain.Effects
{
    public class EffectName : Name
    {
        private EffectName(string value) : base($"effect-{value}")
        {
        }

        public static class Shared
        {
            public static EffectName HighGroundSearch => new EffectName($"{nameof(Shared)}{nameof(HighGroundSearch)}".ToKebabCase());
            public static EffectName HighGroundApplyBehaviour => new EffectName($"{nameof(Shared)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
            public static EffectName PassiveIncomeApplyBehaviour => new EffectName($"{nameof(Shared)}{nameof(PassiveIncomeApplyBehaviour)}".ToKebabCase());
            public static EffectName ScrapsIncomeApplyBehaviour => new EffectName($"{nameof(Shared)}{nameof(ScrapsIncomeApplyBehaviour)}".ToKebabCase());
            public static EffectName CelestiumIncomeApplyBehaviour => new EffectName($"{nameof(Shared)}{nameof(CelestiumIncomeApplyBehaviour)}".ToKebabCase());

            public static class Revelators
            {
                public static EffectName NoPopulationSpaceSearch => new EffectName($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceSearch)}".ToKebabCase());
                public static EffectName NoPopulationSpaceApplyBehaviour => new EffectName($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceApplyBehaviour)}".ToKebabCase());
            }

            public static class Uee
            {
                public static EffectName PowerGeneratorApplyBehaviour => new EffectName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorApplyBehaviour)}".ToKebabCase());
                public static EffectName PowerGeneratorModifyPlayer => new EffectName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorModifyPlayer)}".ToKebabCase());
                public static EffectName PowerDependencyApplyBehaviour => new EffectName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviour)}".ToKebabCase());
                public static EffectName PowerDependencyDamage => new EffectName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyDamage)}".ToKebabCase());
                public static EffectName PowerDependencyApplyBehaviourDisable => new EffectName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviourDisable)}".ToKebabCase());
                public static EffectName PowerDependencyApplyBehaviourInactive => new EffectName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyApplyBehaviourInactive)}".ToKebabCase());
                public static EffectName PositiveFaithSearch => new EffectName($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithSearch)}".ToKebabCase());
                public static EffectName PositiveFaithApplyBehaviour => new EffectName($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithApplyBehaviour)}".ToKebabCase());
            }
        }
        
        public static class Citadel
        {
            public static EffectName ExecutiveStashApplyBehaviour => new EffectName($"{nameof(Citadel)}{nameof(ExecutiveStashApplyBehaviour)}".ToKebabCase());
            public static EffectName AscendableApplyBehaviour => new EffectName($"{nameof(Citadel)}{nameof(AscendableApplyBehaviour)}".ToKebabCase());
            public static EffectName HighGroundApplyBehaviour => new EffectName($"{nameof(Citadel)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
        }

        public static class Hut
        {
        }
        
        public static class Obelisk
        {
            public static EffectName CelestiumDischargeSearchLong => new EffectName($"{nameof(Obelisk)}{nameof(CelestiumDischargeSearchLong)}".ToKebabCase());
            public static EffectName CelestiumDischargeApplyBehaviourLong => new EffectName($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourLong)}".ToKebabCase());
            public static EffectName CelestiumDischargeSearchShort => new EffectName($"{nameof(Obelisk)}{nameof(CelestiumDischargeSearchShort)}".ToKebabCase());
            public static EffectName CelestiumDischargeApplyBehaviourShort => new EffectName($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourShort)}".ToKebabCase());
            public static EffectName CelestiumDischargeApplyBehaviourNegative => new EffectName($"{nameof(Obelisk)}{nameof(CelestiumDischargeApplyBehaviourNegative)}".ToKebabCase());
        }

        public static class Shack
        {
            public static EffectName AccommodationApplyBehaviour => new EffectName($"{nameof(Shack)}{nameof(AccommodationApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Smith
        {
            public static EffectName MeleeWeaponProductionApplyBehaviour => new EffectName($"{nameof(Smith)}{nameof(MeleeWeaponProductionApplyBehaviour)}".ToKebabCase());
        }

        public static class Fletcher
        {
            public static EffectName RangedWeaponProductionApplyBehaviour => new EffectName($"{nameof(Fletcher)}{nameof(RangedWeaponProductionApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Alchemy
        {
            public static EffectName SpecialWeaponProductionApplyBehaviour => new EffectName($"{nameof(Alchemy)}{nameof(SpecialWeaponProductionApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Depot
        {
            public static EffectName WeaponStorageApplyBehaviour => new EffectName($"{nameof(Depot)}{nameof(WeaponStorageApplyBehaviour)}".ToKebabCase());
        }

        public static class Workshop
        {
        }

        public static class Outpost
        {
            public static EffectName AscendableApplyBehaviour => new EffectName($"{nameof(Outpost)}{nameof(AscendableApplyBehaviour)}".ToKebabCase());
            public static EffectName HighGroundApplyBehaviour => new EffectName($"{nameof(Outpost)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
        }

        public static class Barricade
        {
            public static EffectName ProtectiveShieldSearch => new EffectName($"{nameof(Barricade)}{nameof(ProtectiveShieldSearch)}".ToKebabCase());
            public static EffectName ProtectiveShieldApplyBehaviour => new EffectName($"{nameof(Barricade)}{nameof(ProtectiveShieldApplyBehaviour)}".ToKebabCase());
            public static EffectName CaltropsSearch => new EffectName($"{nameof(Barricade)}{nameof(CaltropsSearch)}".ToKebabCase());
            public static EffectName CaltropsDamage => new EffectName($"{nameof(Barricade)}{nameof(CaltropsDamage)}".ToKebabCase());
            public static EffectName DecomposeApplyBehaviour => new EffectName($"{nameof(Barricade)}{nameof(DecomposeApplyBehaviour)}".ToKebabCase());
            public static EffectName DecomposeRemoveBehaviour => new EffectName($"{nameof(Barricade)}{nameof(DecomposeRemoveBehaviour)}".ToKebabCase());
            public static EffectName DecomposeDamage => new EffectName($"{nameof(Barricade)}{nameof(DecomposeDamage)}".ToKebabCase());
        }
        
        public static class BatteryCore
        {
            public static EffectName PowerGridApplyBehaviour => new EffectName($"{nameof(BatteryCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase());
            public static EffectName FusionCoreUpgradeApplyBehaviour => new EffectName($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeApplyBehaviour)}".ToKebabCase());
            public static EffectName FusionCoreUpgradeCreateEntity => new EffectName($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeCreateEntity)}".ToKebabCase());
            public static EffectName FusionCoreUpgradeDestroy => new EffectName($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeDestroy)}".ToKebabCase());
            public static EffectName FusionCoreUpgradeModifyResearch => new EffectName($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeModifyResearch)}".ToKebabCase());
        }
        
        public static class FusionCore
        {
            public static EffectName PowerGridApplyBehaviour => new EffectName($"{nameof(FusionCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase());
            public static EffectName DefenceProtocolDamage => new EffectName($"{nameof(FusionCore)}{nameof(DefenceProtocolDamage)}".ToKebabCase());
            public static EffectName CelestiumCoreUpgradeApplyBehaviour => new EffectName($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeApplyBehaviour)}".ToKebabCase());
            public static EffectName CelestiumCoreUpgradeCreateEntity => new EffectName($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeCreateEntity)}".ToKebabCase());
            public static EffectName CelestiumCoreUpgradeDestroy => new EffectName($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeDestroy)}".ToKebabCase());
            public static EffectName CelestiumCoreUpgradeModifyResearch => new EffectName($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeModifyResearch)}".ToKebabCase());
        }
        
        public static class CelestiumCore
        {
            public static EffectName PowerGridApplyBehaviour => new EffectName($"{nameof(CelestiumCore)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase());
            public static EffectName DefenceProtocolDamage => new EffectName($"{nameof(CelestiumCore)}{nameof(DefenceProtocolDamage)}".ToKebabCase());
            public static EffectName HeightenedConductivityModifyResearch => new EffectName($"{nameof(CelestiumCore)}{nameof(HeightenedConductivityModifyResearch)}".ToKebabCase());
        }
        
        public static class Collector
        {
            public static EffectName DirectTransitSystemApplyBehaviourInactive => new EffectName($"{nameof(Collector)}{nameof(DirectTransitSystemApplyBehaviourInactive)}".ToKebabCase());
            public static EffectName DirectTransitSystemApplyBehaviourActive => new EffectName($"{nameof(Collector)}{nameof(DirectTransitSystemApplyBehaviourActive)}".ToKebabCase());
        }
        
        public static class Extractor
        {
            public static EffectName ReinforcedInfrastructureApplyBehaviourInactive => new EffectName($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureApplyBehaviourInactive)}".ToKebabCase());
            public static EffectName ReinforcedInfrastructureApplyBehaviourActive => new EffectName($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureApplyBehaviourActive)}".ToKebabCase());
        }
        
        public static class PowerPole
        {
            public static EffectName PowerGridApplyBehaviour => new EffectName($"{nameof(PowerPole)}{nameof(PowerGridApplyBehaviour)}".ToKebabCase());
            public static EffectName ExcessDistributionSearch => new EffectName($"{nameof(PowerPole)}{nameof(ExcessDistributionSearch)}".ToKebabCase());
            public static EffectName ExcessDistributionApplyBehaviour => new EffectName($"{nameof(PowerPole)}{nameof(ExcessDistributionApplyBehaviour)}".ToKebabCase());
            public static EffectName ImprovedPowerGridModifyAbilityPowerGrid => new EffectName($"{nameof(PowerPole)}{nameof(ImprovedPowerGridModifyAbilityPowerGrid)}".ToKebabCase());
            public static EffectName ImprovedPowerGridModifyAbilityExcessDistribution => new EffectName($"{nameof(PowerPole)}{nameof(ImprovedPowerGridModifyAbilityExcessDistribution)}".ToKebabCase());
            public static EffectName PowerGridImprovedApplyBehaviour => new EffectName($"{nameof(PowerPole)}{nameof(PowerGridImprovedApplyBehaviour)}".ToKebabCase());
            public static EffectName ExcessDistributionImprovedSearch => new EffectName($"{nameof(PowerPole)}{nameof(ExcessDistributionImprovedSearch)}".ToKebabCase());
        }
        
        public static class Temple
        {
            public static EffectName KeepingTheFaithSearch => new EffectName($"{nameof(Temple)}{nameof(KeepingTheFaithSearch)}".ToKebabCase());
            public static EffectName KeepingTheFaithApplyBehaviourBuff => new EffectName($"{nameof(Temple)}{nameof(KeepingTheFaithApplyBehaviourBuff)}".ToKebabCase());
            public static EffectName KeepingTheFaithApplyBehaviourIncome => new EffectName($"{nameof(Temple)}{nameof(KeepingTheFaithApplyBehaviourIncome)}".ToKebabCase());
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
            public static EffectName HighGroundApplyBehaviour => new EffectName($"{nameof(Wall)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Stairs
        {
            public static EffectName AscendableApplyBehaviour => new EffectName($"{nameof(Stairs)}{nameof(AscendableApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Gate
        {
            public static EffectName HighGroundApplyBehaviour => new EffectName($"{nameof(Gate)}{nameof(HighGroundApplyBehaviour)}".ToKebabCase());
            public static EffectName AscendableApplyBehaviour => new EffectName($"{nameof(Gate)}{nameof(AscendableApplyBehaviour)}".ToKebabCase());
            public static EffectName EntranceApplyBehaviour => new EffectName($"{nameof(Gate)}{nameof(EntranceApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Watchtower
        {
            public static EffectName VantagePointSearch => new EffectName($"{nameof(Watchtower)}{nameof(VantagePointSearch)}".ToKebabCase());
            public static EffectName VantagePointApplyBehaviour => new EffectName($"{nameof(Watchtower)}{nameof(VantagePointApplyBehaviour)}".ToKebabCase());
        }
        
        public static class Bastion
        {
            public static EffectName BattlementSearch => new EffectName($"{nameof(Bastion)}{nameof(BattlementSearch)}".ToKebabCase());
            public static EffectName BattlementApplyBehaviour => new EffectName($"{nameof(Bastion)}{nameof(BattlementApplyBehaviour)}".ToKebabCase());
        }

        public static class Leader
        {
            public static EffectName AllForOneApplyBehaviour => new EffectName($"{nameof(Leader)}{nameof(AllForOneApplyBehaviour)}".ToKebabCase());
            public static EffectName AllForOneModifyPlayer => new EffectName($"{nameof(Leader)}{nameof(AllForOneModifyPlayer)}".ToKebabCase());
            public static EffectName MenacingPresenceSearch => new EffectName($"{nameof(Leader)}{nameof(MenacingPresenceSearch)}".ToKebabCase());
            public static EffectName MenacingPresenceApplyBehaviour => new EffectName($"{nameof(Leader)}{nameof(MenacingPresenceApplyBehaviour)}".ToKebabCase());
            public static EffectName OneForAllApplyBehaviourObelisk => new EffectName($"{nameof(Leader)}{nameof(OneForAllApplyBehaviourObelisk)}".ToKebabCase());
            public static EffectName OneForAllSearch => new EffectName($"{nameof(Leader)}{nameof(OneForAllSearch)}".ToKebabCase());
            public static EffectName OneForAllApplyBehaviourHeal => new EffectName($"{nameof(Leader)}{nameof(OneForAllApplyBehaviourHeal)}".ToKebabCase());
        }

        public static class Slave
        {
            public static EffectName RepairApplyBehaviourStructure => new EffectName($"{nameof(Slave)}{nameof(RepairApplyBehaviourStructure)}".ToKebabCase());
            public static EffectName RepairApplyBehaviourSelf => new EffectName($"{nameof(Slave)}{nameof(RepairApplyBehaviourSelf)}".ToKebabCase());
            public static EffectName ManualLabourApplyBehaviourHut => new EffectName($"{nameof(Slave)}{nameof(ManualLabourApplyBehaviourHut)}".ToKebabCase());
            public static EffectName ManualLabourApplyBehaviourSelf => new EffectName($"{nameof(Slave)}{nameof(ManualLabourApplyBehaviourSelf)}".ToKebabCase());
            public static EffectName ManualLabourModifyPlayer => new EffectName($"{nameof(Slave)}{nameof(ManualLabourModifyPlayer)}".ToKebabCase());
        }

        public static class Quickdraw
        {
            public static EffectName DoubleshotApplyBehaviour => new EffectName($"{nameof(Quickdraw)}{nameof(DoubleshotApplyBehaviour)}".ToKebabCase());
            public static EffectName CrippleApplyBehaviour => new EffectName($"{nameof(Quickdraw)}{nameof(CrippleApplyBehaviour)}".ToKebabCase());
        }

        public static class Gorger
        {
            public static EffectName FanaticSuicideApplyBehaviourBuff => new EffectName($"{nameof(Gorger)}{nameof(FanaticSuicideApplyBehaviourBuff)}".ToKebabCase());
            public static EffectName FanaticSuicideDestroy => new EffectName($"{nameof(Gorger)}{nameof(FanaticSuicideDestroy)}".ToKebabCase());
            public static EffectName FanaticSuicideSearch => new EffectName($"{nameof(Gorger)}{nameof(FanaticSuicideSearch)}".ToKebabCase());
            public static EffectName FanaticSuicideDamage => new EffectName($"{nameof(Gorger)}{nameof(FanaticSuicideDamage)}".ToKebabCase());
        }

        public static class Camou
        {
            public static EffectName SilentAssassinOnHitDamage => new EffectName($"{nameof(Camou)}{nameof(SilentAssassinOnHitDamage)}".ToKebabCase());
            public static EffectName SilentAssassinOnHitSilence => new EffectName($"{nameof(Camou)}{nameof(SilentAssassinOnHitSilence)}".ToKebabCase());
            public static EffectName SilentAssassinSearchFriendly => new EffectName($"{nameof(Camou)}{nameof(SilentAssassinSearchFriendly)}".ToKebabCase());
            public static EffectName SilentAssassinSearchEnemy => new EffectName($"{nameof(Camou)}{nameof(SilentAssassinSearchEnemy)}".ToKebabCase());
            public static EffectName ClimbTeleport => new EffectName($"{nameof(Camou)}{nameof(ClimbTeleport)}".ToKebabCase());
            public static EffectName ClimbApplyBehaviour => new EffectName($"{nameof(Camou)}{nameof(ClimbApplyBehaviour)}".ToKebabCase());
        }

        public static class Shaman
        {
            public static EffectName WondrousGooCreateEntity => new EffectName($"{nameof(Shaman)}{nameof(WondrousGooCreateEntity)}".ToKebabCase());
            public static EffectName WondrousGooSearch => new EffectName($"{nameof(Shaman)}{nameof(WondrousGooSearch)}".ToKebabCase());
            public static EffectName WondrousGooApplyBehaviour => new EffectName($"{nameof(Shaman)}{nameof(WondrousGooApplyBehaviour)}".ToKebabCase());
            public static EffectName WondrousGooDestroy => new EffectName($"{nameof(Shaman)}{nameof(WondrousGooDestroy)}".ToKebabCase());
            public static EffectName WondrousGooDamage => new EffectName($"{nameof(Shaman)}{nameof(WondrousGooDamage)}".ToKebabCase());
        }

        public static class Pyre
        {
            public static EffectName CargoCreateEntity => new EffectName($"{nameof(Pyre)}{nameof(CargoCreateEntity)}".ToKebabCase());
            public static EffectName WallOfFlamesCreateEntity => new EffectName($"{nameof(Pyre)}{nameof(WallOfFlamesCreateEntity)}".ToKebabCase());
            public static EffectName WallOfFlamesDestroy => new EffectName($"{nameof(Pyre)}{nameof(WallOfFlamesDestroy)}".ToKebabCase());
            public static EffectName WallOfFlamesDamage => new EffectName($"{nameof(Pyre)}{nameof(WallOfFlamesDamage)}".ToKebabCase());
            public static EffectName PhantomMenaceApplyBehaviour => new EffectName($"{nameof(Pyre)}{nameof(PhantomMenaceApplyBehaviour)}".ToKebabCase());
        }

        public static class BigBadBull
        {
            public static EffectName UnleashTheRageSearch => new EffectName($"{nameof(BigBadBull)}{nameof(UnleashTheRageSearch)}".ToKebabCase());
            public static EffectName UnleashTheRageDamage => new EffectName($"{nameof(BigBadBull)}{nameof(UnleashTheRageDamage)}".ToKebabCase());
            public static EffectName UnleashTheRageForce => new EffectName($"{nameof(BigBadBull)}{nameof(UnleashTheRageForce)}".ToKebabCase());
            public static EffectName UnleashTheRageForceDamage => new EffectName($"{nameof(BigBadBull)}{nameof(UnleashTheRageForceDamage)}".ToKebabCase());
        }

        public static class Mummy
        {
            public static EffectName SpawnRoachCreateEntity => new EffectName($"{nameof(Mummy)}{nameof(SpawnRoachCreateEntity)}".ToKebabCase());
            public static EffectName LeapOfHungerModifyAbility => new EffectName($"{nameof(Mummy)}{nameof(LeapOfHungerModifyAbility)}".ToKebabCase());
        }

        public static class Roach
        {
            public static EffectName DegradingCarapaceApplyBehaviour => new EffectName($"{nameof(Roach)}{nameof(DegradingCarapaceApplyBehaviour)}".ToKebabCase());
            public static EffectName DegradingCarapacePeriodicApplyBehaviour => new EffectName($"{nameof(Roach)}{nameof(DegradingCarapacePeriodicApplyBehaviour)}".ToKebabCase());
            public static EffectName DegradingCarapaceSelfDamage => new EffectName($"{nameof(Roach)}{nameof(DegradingCarapaceSelfDamage)}".ToKebabCase());
            public static EffectName CorrosiveSpitDamage => new EffectName($"{nameof(Roach)}{nameof(CorrosiveSpitDamage)}".ToKebabCase());
        }

        public static class Parasite
        {
            public static EffectName ParalysingGraspApplyTetherBehaviour => new EffectName($"{nameof(Parasite)}{nameof(ParalysingGraspApplyTetherBehaviour)}".ToKebabCase());
            public static EffectName ParalysingGraspApplyAttackBehaviour => new EffectName($"{nameof(Parasite)}{nameof(ParalysingGraspApplyAttackBehaviour)}".ToKebabCase());
            public static EffectName ParalysingGraspApplySelfBehaviour => new EffectName($"{nameof(Parasite)}{nameof(ParalysingGraspApplySelfBehaviour)}".ToKebabCase());
        }

        public static class Horrior
        {
            public static EffectName ExpertFormationSearch => new EffectName($"{nameof(Horrior)}{nameof(ExpertFormationSearch)}".ToKebabCase());
            public static EffectName ExpertFormationApplyBehaviour => new EffectName($"{nameof(Horrior)}{nameof(ExpertFormationApplyBehaviour)}".ToKebabCase());
            public static EffectName MountApplyBehaviour => new EffectName($"{nameof(Horrior)}{nameof(MountApplyBehaviour)}".ToKebabCase());
            public static EffectName MountCreateEntity => new EffectName($"{nameof(Horrior)}{nameof(MountCreateEntity)}".ToKebabCase());
            public static EffectName MountDestroy => new EffectName($"{nameof(Horrior)}{nameof(MountDestroy)}".ToKebabCase());
        }

        public static class Marksman
        {
            public static EffectName CriticalMarkApplyBehaviour => new EffectName($"{nameof(Marksman)}{nameof(CriticalMarkApplyBehaviour)}".ToKebabCase());
            public static EffectName CriticalMarkDamage => new EffectName($"{nameof(Marksman)}{nameof(CriticalMarkDamage)}".ToKebabCase());
        }

        public static class Surfer
        {
            public static EffectName DismountApplyBehaviour => new EffectName($"{nameof(Surfer)}{nameof(DismountApplyBehaviour)}".ToKebabCase());
            public static EffectName DismountCreateEntity => new EffectName($"{nameof(Surfer)}{nameof(DismountCreateEntity)}".ToKebabCase());
        }
        
        public static class Mortar
        {
            public static EffectName DeadlyAmmunitionApplyBehaviour => new EffectName($"{nameof(Mortar)}{nameof(DeadlyAmmunitionApplyBehaviour)}".ToKebabCase());
            public static EffectName DeadlyAmmunitionSearch => new EffectName($"{nameof(Mortar)}{nameof(DeadlyAmmunitionSearch)}".ToKebabCase());
            public static EffectName DeadlyAmmunitionDamage => new EffectName($"{nameof(Mortar)}{nameof(DeadlyAmmunitionDamage)}".ToKebabCase());
            public static EffectName ReloadApplyBehaviour => new EffectName($"{nameof(Mortar)}{nameof(ReloadApplyBehaviour)}".ToKebabCase());
            public static EffectName ReloadReload => new EffectName($"{nameof(Mortar)}{nameof(ReloadReload)}".ToKebabCase());
            public static EffectName PiercingBlastApplyBehaviour => new EffectName($"{nameof(Mortar)}{nameof(PiercingBlastApplyBehaviour)}".ToKebabCase());
        }

        public static class Hawk
        {
            public static EffectName TacticalGogglesApplyBehaviour => new EffectName($"{nameof(Hawk)}{nameof(TacticalGogglesApplyBehaviour)}".ToKebabCase());
            public static EffectName LeadershipApplyBehaviour => new EffectName($"{nameof(Hawk)}{nameof(LeadershipApplyBehaviour)}".ToKebabCase());
            public static EffectName HealthKitApplyBehaviour => new EffectName($"{nameof(Hawk)}{nameof(HealthKitApplyBehaviour)}".ToKebabCase());
            public static EffectName HealthKitSearch => new EffectName($"{nameof(Hawk)}{nameof(HealthKitSearch)}".ToKebabCase());
            public static EffectName HealthKitHealApplyBehaviour => new EffectName($"{nameof(Hawk)}{nameof(HealthKitHealApplyBehaviour)}".ToKebabCase());
        }

        public static class Engineer
        {
            public static EffectName OperateApplyBehaviour => new EffectName($"{nameof(Engineer)}{nameof(OperateApplyBehaviour)}".ToKebabCase());
            public static EffectName OperateModifyCounter => new EffectName($"{nameof(Engineer)}{nameof(OperateModifyCounter)}".ToKebabCase());
            public static EffectName OperateDestroy => new EffectName($"{nameof(Engineer)}{nameof(OperateDestroy)}".ToKebabCase());
            public static EffectName RepairStructureApplyBehaviour => new EffectName($"{nameof(Engineer)}{nameof(RepairStructureApplyBehaviour)}".ToKebabCase());
            public static EffectName RepairMachineApplyBehaviour => new EffectName($"{nameof(Engineer)}{nameof(RepairMachineApplyBehaviour)}".ToKebabCase());
            public static EffectName RepairHorriorApplyBehaviour => new EffectName($"{nameof(Engineer)}{nameof(RepairHorriorApplyBehaviour)}".ToKebabCase());
            public static EffectName RepairApplyBehaviourSelf => new EffectName($"{nameof(Engineer)}{nameof(RepairApplyBehaviourSelf)}".ToKebabCase());
        }
        
        public static class Cannon
        {
            public static EffectName MachineApplyBehaviour => new EffectName($"{nameof(Cannon)}{nameof(MachineApplyBehaviour)}".ToKebabCase());
            public static EffectName MachineRemoveBehaviour => new EffectName($"{nameof(Cannon)}{nameof(MachineRemoveBehaviour)}".ToKebabCase());
            public static EffectName HeatUpCreateEntity => new EffectName($"{nameof(Cannon)}{nameof(HeatUpCreateEntity)}".ToKebabCase());
            public static EffectName HeatUpApplyWaitBehaviour => new EffectName($"{nameof(Cannon)}{nameof(HeatUpApplyWaitBehaviour)}".ToKebabCase());
            public static EffectName HeatUpSearch => new EffectName($"{nameof(Cannon)}{nameof(HeatUpSearch)}".ToKebabCase());
            public static EffectName HeatUpDamage => new EffectName($"{nameof(Cannon)}{nameof(HeatUpDamage)}".ToKebabCase());
            public static EffectName HeatUpDestroy => new EffectName($"{nameof(Cannon)}{nameof(HeatUpDestroy)}".ToKebabCase());
            public static EffectName HeatUpRemoveBehaviour => new EffectName($"{nameof(Cannon)}{nameof(HeatUpRemoveBehaviour)}".ToKebabCase());
        }

        public static class Ballista
        {
            public static EffectName MachineApplyBehaviour => new EffectName($"{nameof(Ballista)}{nameof(MachineApplyBehaviour)}".ToKebabCase());
            public static EffectName MachineRemoveBehaviour => new EffectName($"{nameof(Ballista)}{nameof(MachineRemoveBehaviour)}".ToKebabCase());
            public static EffectName AimDamage => new EffectName($"{nameof(Ballista)}{nameof(AimDamage)}".ToKebabCase());
            public static EffectName AimApplyBehaviour => new EffectName($"{nameof(Ballista)}{nameof(AimApplyBehaviour)}".ToKebabCase());
            public static EffectName AimSearch => new EffectName($"{nameof(Ballista)}{nameof(AimSearch)}".ToKebabCase());
        }
        
        public static class Radar
        {
            public static EffectName MachineApplyBehaviour => new EffectName($"{nameof(Radar)}{nameof(MachineApplyBehaviour)}".ToKebabCase());
            public static EffectName MachineRemoveBehaviour => new EffectName($"{nameof(Radar)}{nameof(MachineRemoveBehaviour)}".ToKebabCase());
            public static EffectName ResonatingSweepCreateEntity => new EffectName($"{nameof(Radar)}{nameof(ResonatingSweepCreateEntity)}".ToKebabCase());
            public static EffectName ResonatingSweepDestroy => new EffectName($"{nameof(Radar)}{nameof(ResonatingSweepDestroy)}".ToKebabCase());
            public static EffectName RadioLocationApplyBehaviour => new EffectName($"{nameof(Radar)}{nameof(RadioLocationApplyBehaviour)}".ToKebabCase());
            public static EffectName RadioLocationSearchDestroy => new EffectName($"{nameof(Radar)}{nameof(RadioLocationSearchDestroy)}".ToKebabCase());
            public static EffectName RadioLocationDestroy => new EffectName($"{nameof(Radar)}{nameof(RadioLocationDestroy)}".ToKebabCase());
            public static EffectName RadioLocationSearchCreate => new EffectName($"{nameof(Radar)}{nameof(RadioLocationSearchCreate)}".ToKebabCase());
            public static EffectName RadioLocationCreateEntity => new EffectName($"{nameof(Radar)}{nameof(RadioLocationCreateEntity)}".ToKebabCase());
        }

        public static class Vessel
        {
            public static EffectName MachineApplyBehaviour => new EffectName($"{nameof(Vessel)}{nameof(MachineApplyBehaviour)}".ToKebabCase());
            public static EffectName MachineRemoveBehaviour => new EffectName($"{nameof(Vessel)}{nameof(MachineRemoveBehaviour)}".ToKebabCase());
            public static EffectName AbsorbentFieldSearch => new EffectName($"{nameof(Vessel)}{nameof(AbsorbentFieldSearch)}".ToKebabCase());
            public static EffectName AbsorbentFieldApplyBehaviour => new EffectName($"{nameof(Vessel)}{nameof(AbsorbentFieldApplyBehaviour)}".ToKebabCase());
            public static EffectName FortifyCreateEntity => new EffectName($"{nameof(Vessel)}{nameof(FortifyCreateEntity)}".ToKebabCase());
            public static EffectName FortifyDestroy => new EffectName($"{nameof(Vessel)}{nameof(FortifyDestroy)}".ToKebabCase());
            public static EffectName FortifySearch => new EffectName($"{nameof(Vessel)}{nameof(FortifySearch)}".ToKebabCase());
            public static EffectName FortifyApplyBehaviour => new EffectName($"{nameof(Vessel)}{nameof(FortifyApplyBehaviour)}".ToKebabCase());
        }

        public static class Omen
        {
            public static EffectName RenditionPlacementApplyBehaviour => new EffectName($"{nameof(Omen)}{nameof(RenditionPlacementApplyBehaviour)}".ToKebabCase());
            public static EffectName RenditionPlacementExecuteAbility => new EffectName($"{nameof(Omen)}{nameof(RenditionPlacementExecuteAbility)}".ToKebabCase());
            public static EffectName RenditionPlacementCreateEntity => new EffectName($"{nameof(Omen)}{nameof(RenditionPlacementCreateEntity)}".ToKebabCase());
            public static EffectName RenditionDestroy => new EffectName($"{nameof(Omen)}{nameof(RenditionDestroy)}".ToKebabCase());
            public static EffectName RenditionSearch => new EffectName($"{nameof(Omen)}{nameof(RenditionSearch)}".ToKebabCase());
            public static EffectName RenditionDamage => new EffectName($"{nameof(Omen)}{nameof(RenditionDamage)}".ToKebabCase());
            public static EffectName RenditionApplyBehaviourSlow => new EffectName($"{nameof(Omen)}{nameof(RenditionApplyBehaviourSlow)}".ToKebabCase());
        }
    }
}
