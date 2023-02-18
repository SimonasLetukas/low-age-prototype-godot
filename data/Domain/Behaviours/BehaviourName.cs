using low_age_data.Common;

namespace low_age_data.Domain.Behaviours
{
    public class BehaviourName : Name
    {
        private BehaviourName(string value) : base($"behaviour-{value}")
        {
        }

        public static class Shared
        {
            public static BehaviourName HighGroundBuff => new BehaviourName($"{nameof(Shared)}{nameof(HighGroundBuff)}".ToKebabCase());
            public static BehaviourName PassiveIncomeIncome => new BehaviourName($"{nameof(Shared)}{nameof(PassiveIncomeIncome)}".ToKebabCase());
            public static BehaviourName ScrapsIncomeIncome => new BehaviourName($"{nameof(Shared)}{nameof(ScrapsIncomeIncome)}".ToKebabCase());
            public static BehaviourName CelestiumIncomeIncome => new BehaviourName($"{nameof(Shared)}{nameof(CelestiumIncomeIncome)}".ToKebabCase());

            public static class Revelators
            {
                public static BehaviourName BuildingBuildable => new BehaviourName($"{nameof(Shared)}{nameof(Revelators)}{nameof(BuildingBuildable)}".ToKebabCase());
                public static BehaviourName NoPopulationSpaceInterceptDamage => new BehaviourName($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceInterceptDamage)}".ToKebabCase());
            }
            
            public static class Uee
            {
                public static BehaviourName BuildingBuildable => new BehaviourName($"{nameof(Shared)}{nameof(Uee)}{nameof(BuildingBuildable)}".ToKebabCase());
                public static BehaviourName PowerGeneratorBuff => new BehaviourName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorBuff)}".ToKebabCase());
                public static BehaviourName PowerDependencyBuff => new BehaviourName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuff)}".ToKebabCase());
                public static BehaviourName PowerDependencyBuffDisable => new BehaviourName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuffDisable)}".ToKebabCase());
                public static BehaviourName PowerDependencyBuffInactive => new BehaviourName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuffInactive)}".ToKebabCase());
                public static BehaviourName PositiveFaithBuff => new BehaviourName($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithBuff)}".ToKebabCase());
            }
        }
        
        public static class Citadel
        {
            public static BehaviourName ExecutiveStashIncome => new BehaviourName($"{nameof(Citadel)}{nameof(ExecutiveStashIncome)}".ToKebabCase());
            public static BehaviourName AscendableAscendable => new BehaviourName($"{nameof(Citadel)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourName HighGroundHighGround => new BehaviourName($"{nameof(Citadel)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }

        public static class Hut
        {
            public static BehaviourName BuildingBuildable => new BehaviourName($"{nameof(Hut)}{nameof(BuildingBuildable)}".ToKebabCase());
        }
        
        public static class Obelisk
        {
            public static BehaviourName BuildingBuildable => new BehaviourName($"{nameof(Obelisk)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourName CelestiumDischargeBuffLong => new BehaviourName($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffLong)}".ToKebabCase());
            public static BehaviourName CelestiumDischargeBuffShort => new BehaviourName($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffShort)}".ToKebabCase());
            public static BehaviourName CelestiumDischargeBuffNegative => new BehaviourName($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffNegative)}".ToKebabCase());
        }
        
        public static class Shack
        {
            public static BehaviourName AccommodationIncome => new BehaviourName($"{nameof(Shack)}{nameof(AccommodationIncome)}".ToKebabCase());
        }
        
        public static class Smith
        {
            public static BehaviourName MeleeWeaponProductionIncome => new BehaviourName($"{nameof(Smith)}{nameof(MeleeWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Fletcher
        {
            public static BehaviourName RangedWeaponProductionIncome => new BehaviourName($"{nameof(Fletcher)}{nameof(RangedWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Alchemy
        {
            public static BehaviourName SpecialWeaponProductionIncome => new BehaviourName($"{nameof(Alchemy)}{nameof(SpecialWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Depot
        {
            public static BehaviourName WeaponStorageIncome => new BehaviourName($"{nameof(Depot)}{nameof(WeaponStorageIncome)}".ToKebabCase());
        }
        
        public static class Workshop
        {
        }
        
        public static class Outpost
        {
            public static BehaviourName AscendableAscendable => new BehaviourName($"{nameof(Outpost)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourName HighGroundHighGround => new BehaviourName($"{nameof(Outpost)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }
        
        public static class Barricade
        {
            public static BehaviourName ProtectiveShieldBuff => new BehaviourName($"{nameof(Barricade)}{nameof(ProtectiveShieldBuff)}".ToKebabCase());
            public static BehaviourName DecomposeBuff => new BehaviourName($"{nameof(Barricade)}{nameof(DecomposeBuff)}".ToKebabCase());
        }
        
        public static class BatteryCore
        {
            public static BehaviourName PowerGridMaskProvider => new BehaviourName($"{nameof(BatteryCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourName FusionCoreUpgradeBuff => new BehaviourName($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeBuff)}".ToKebabCase());
        }
        
        public static class FusionCore
        {
            public static BehaviourName PowerGridMaskProvider => new BehaviourName($"{nameof(FusionCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourName CelestiumCoreUpgradeBuff => new BehaviourName($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeBuff)}".ToKebabCase());
        }
        
        public static class CelestiumCore
        {
            public static BehaviourName PowerGridMaskProvider => new BehaviourName($"{nameof(CelestiumCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
        }
        
        public static class Collector
        {
            public static BehaviourName BuildingBuildable => new BehaviourName($"{nameof(Collector)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourName DirectTransitSystemInactiveBuff => new BehaviourName($"{nameof(Collector)}{nameof(DirectTransitSystemInactiveBuff)}".ToKebabCase());
            public static BehaviourName DirectTransitSystemActiveIncome => new BehaviourName($"{nameof(Collector)}{nameof(DirectTransitSystemActiveIncome)}".ToKebabCase());
        }
        
        public static class Extractor
        {
            public static BehaviourName BuildingBuildable => new BehaviourName($"{nameof(Extractor)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourName ReinforcedInfrastructureInactiveBuff => new BehaviourName($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureInactiveBuff)}".ToKebabCase());
            public static BehaviourName ReinforcedInfrastructureActiveBuff => new BehaviourName($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureActiveBuff)}".ToKebabCase());
        }

        public static class PowerPole
        {
            public static BehaviourName PowerGridMaskProvider => new BehaviourName($"{nameof(PowerPole)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourName ExcessDistributionBuff => new BehaviourName($"{nameof(PowerPole)}{nameof(ExcessDistributionBuff)}".ToKebabCase());
            public static BehaviourName PowerGridImprovedMaskProvider => new BehaviourName($"{nameof(PowerPole)}{nameof(PowerGridImprovedMaskProvider)}".ToKebabCase());
        }
        
        public static class Temple
        {
            public static BehaviourName KeepingTheFaithBuff => new BehaviourName($"{nameof(Temple)}{nameof(KeepingTheFaithBuff)}".ToKebabCase());
            public static BehaviourName KeepingTheFaithIncome => new BehaviourName($"{nameof(Temple)}{nameof(KeepingTheFaithIncome)}".ToKebabCase());
        }
        
        public static class Wall
        {
            public static BehaviourName BuildingBuildable => new BehaviourName($"{nameof(Wall)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourName HighGroundHighGround => new BehaviourName($"{nameof(Wall)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }

        public static class Stairs
        {
            public static BehaviourName AscendableAscendable => new BehaviourName($"{nameof(Stairs)}{nameof(AscendableAscendable)}".ToKebabCase());
        }

        public static class Gate
        {
            public static BehaviourName HighGroundHighGround => new BehaviourName($"{nameof(Gate)}{nameof(HighGroundHighGround)}".ToKebabCase());
            public static BehaviourName AscendableAscendable => new BehaviourName($"{nameof(Gate)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourName EntranceMovementBlock => new BehaviourName($"{nameof(Gate)}{nameof(EntranceMovementBlock)}".ToKebabCase());
        }

        public static class Watchtower
        {
            public static BehaviourName VantagePointBuff => new BehaviourName($"{nameof(Watchtower)}{nameof(VantagePointBuff)}".ToKebabCase());
        }

        public static class Bastion
        {
            public static BehaviourName BattlementBuff => new BehaviourName($"{nameof(Bastion)}{nameof(BattlementBuff)}".ToKebabCase());
        }

        public static class Leader
        {
            public static BehaviourName AllForOneBuff => new BehaviourName($"{nameof(Leader)}{nameof(AllForOneBuff)}".ToKebabCase());
            public static BehaviourName MenacingPresenceBuff => new BehaviourName($"{nameof(Leader)}{nameof(MenacingPresenceBuff)}".ToKebabCase());
            public static BehaviourName OneForAllObeliskBuff => new BehaviourName($"{nameof(Leader)}{nameof(OneForAllObeliskBuff)}".ToKebabCase());
            public static BehaviourName OneForAllHealBuff => new BehaviourName($"{nameof(Leader)}{nameof(OneForAllHealBuff)}".ToKebabCase());
        }

        public static class Slave
        {
            public static BehaviourName RepairStructureBuff => new BehaviourName($"{nameof(Slave)}{nameof(RepairStructureBuff)}".ToKebabCase());
            public static BehaviourName RepairWait => new BehaviourName($"{nameof(Slave)}{nameof(RepairWait)}".ToKebabCase());
            public static BehaviourName ManualLabourBuff => new BehaviourName($"{nameof(Slave)}{nameof(ManualLabourBuff)}".ToKebabCase());
            public static BehaviourName ManualLabourWait => new BehaviourName($"{nameof(Slave)}{nameof(ManualLabourWait)}".ToKebabCase());
        }

        public static class Quickdraw
        {
            public static BehaviourName DoubleshotExtraAttack => new BehaviourName($"{nameof(Quickdraw)}{nameof(DoubleshotExtraAttack)}".ToKebabCase());
            public static BehaviourName CrippleBuff => new BehaviourName($"{nameof(Quickdraw)}{nameof(CrippleBuff)}".ToKebabCase());
        }

        public static class Gorger
        {
            public static BehaviourName FanaticSuicideBuff => new BehaviourName($"{nameof(Gorger)}{nameof(FanaticSuicideBuff)}".ToKebabCase());
        }

        public static class Camou
        {
            public static BehaviourName SilentAssassinBuff => new BehaviourName($"{nameof(Camou)}{nameof(SilentAssassinBuff)}".ToKebabCase());
            public static BehaviourName ClimbWait => new BehaviourName($"{nameof(Camou)}{nameof(ClimbWait)}".ToKebabCase());
            public static BehaviourName ClimbBuff => new BehaviourName($"{nameof(Camou)}{nameof(ClimbBuff)}".ToKebabCase());
        }

        public static class Shaman
        {
            public static BehaviourName WondrousGooFeatureWait => new BehaviourName($"{nameof(Shaman)}{nameof(WondrousGooFeatureWait)}".ToKebabCase());
            public static BehaviourName WondrousGooFeatureBuff => new BehaviourName($"{nameof(Shaman)}{nameof(WondrousGooFeatureBuff)}".ToKebabCase());
            public static BehaviourName WondrousGooBuff => new BehaviourName($"{nameof(Shaman)}{nameof(WondrousGooBuff)}".ToKebabCase());
        }

        public static class Pyre
        {
            public static BehaviourName CargoTether => new BehaviourName($"{nameof(Pyre)}{nameof(CargoTether)}".ToKebabCase());
            public static BehaviourName CargoWallOfFlamesBuff => new BehaviourName($"{nameof(Pyre)}{nameof(CargoWallOfFlamesBuff)}".ToKebabCase());
            public static BehaviourName WallOfFlamesBuff => new BehaviourName($"{nameof(Pyre)}{nameof(WallOfFlamesBuff)}".ToKebabCase());
            public static BehaviourName PhantomMenaceBuff => new BehaviourName($"{nameof(Pyre)}{nameof(PhantomMenaceBuff)}".ToKebabCase());
        }

        public static class Roach
        {
            public static BehaviourName DegradingCarapaceBuff => new BehaviourName($"{nameof(Roach)}{nameof(DegradingCarapaceBuff)}".ToKebabCase());
            public static BehaviourName DegradingCarapacePeriodicDamageBuff => new BehaviourName($"{nameof(Roach)}{nameof(DegradingCarapacePeriodicDamageBuff)}".ToKebabCase());
        }

        public static class Parasite
        {
            public static BehaviourName ParalysingGraspTether => new BehaviourName($"{nameof(Parasite)}{nameof(ParalysingGraspTether)}".ToKebabCase());
            public static BehaviourName ParalysingGraspBuff => new BehaviourName($"{nameof(Parasite)}{nameof(ParalysingGraspBuff)}".ToKebabCase());
            public static BehaviourName ParalysingGraspSelfBuff => new BehaviourName($"{nameof(Parasite)}{nameof(ParalysingGraspSelfBuff)}".ToKebabCase());
        }

        public static class Horrior
        {
            public static BehaviourName ExpertFormationBuff => new BehaviourName($"{nameof(Horrior)}{nameof(ExpertFormationBuff)}".ToKebabCase());
            public static BehaviourName MountWait => new BehaviourName($"{nameof(Horrior)}{nameof(MountWait)}".ToKebabCase());
            public static BehaviourName MountBuff => new BehaviourName($"{nameof(Horrior)}{nameof(MountBuff)}".ToKebabCase());
        }

        public static class Marksman
        {
            public static BehaviourName CriticalMarkBuff => new BehaviourName($"{nameof(Marksman)}{nameof(CriticalMarkBuff)}".ToKebabCase());
        }
        
        public static class Surfer
        {
            public static BehaviourName DismountBuff => new BehaviourName($"{nameof(Surfer)}{nameof(DismountBuff)}".ToKebabCase());
        }

        public static class Mortar
        {
            public static BehaviourName DeadlyAmmunitionAmmunition => new BehaviourName($"{nameof(Mortar)}{nameof(DeadlyAmmunitionAmmunition)}".ToKebabCase());
            public static BehaviourName ReloadWait => new BehaviourName($"{nameof(Mortar)}{nameof(ReloadWait)}".ToKebabCase());
            public static BehaviourName ReloadBuff => new BehaviourName($"{nameof(Mortar)}{nameof(ReloadBuff)}".ToKebabCase());
            public static BehaviourName PiercingBlastBuff => new BehaviourName($"{nameof(Mortar)}{nameof(PiercingBlastBuff)}".ToKebabCase());
        }

        public static class Hawk
        {
            public static BehaviourName TacticalGogglesBuff => new BehaviourName($"{nameof(Hawk)}{nameof(TacticalGogglesBuff)}".ToKebabCase());
            public static BehaviourName LeadershipBuff => new BehaviourName($"{nameof(Hawk)}{nameof(LeadershipBuff)}".ToKebabCase());
            public static BehaviourName HealthKitBuff => new BehaviourName($"{nameof(Hawk)}{nameof(HealthKitBuff)}".ToKebabCase());
            public static BehaviourName HealthKitHealBuff => new BehaviourName($"{nameof(Hawk)}{nameof(HealthKitHealBuff)}".ToKebabCase());
        }

        public static class Engineer
        {
            public static BehaviourName OperateBuff => new BehaviourName($"{nameof(Engineer)}{nameof(OperateBuff)}".ToKebabCase());
            public static BehaviourName RepairStructureOrMachineBuff => new BehaviourName($"{nameof(Engineer)}{nameof(RepairStructureOrMachineBuff)}".ToKebabCase());
            public static BehaviourName RepairHorriorBuff => new BehaviourName($"{nameof(Engineer)}{nameof(RepairHorriorBuff)}".ToKebabCase());
            public static BehaviourName RepairWait => new BehaviourName($"{nameof(Engineer)}{nameof(RepairWait)}".ToKebabCase());
        }

        public static class Cannon
        {
            public static BehaviourName AssemblingBuildable => new BehaviourName($"{nameof(Cannon)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourName MachineCounter => new BehaviourName($"{nameof(Cannon)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourName MachineBuff => new BehaviourName($"{nameof(Cannon)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourName HeatUpDangerZoneBuff => new BehaviourName($"{nameof(Cannon)}{nameof(HeatUpDangerZoneBuff)}".ToKebabCase());
            public static BehaviourName HeatUpWait => new BehaviourName($"{nameof(Cannon)}{nameof(HeatUpWait)}".ToKebabCase());
        }

        public static class Ballista
        {
            public static BehaviourName AssemblingBuildable => new BehaviourName($"{nameof(Ballista)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourName MachineCounter => new BehaviourName($"{nameof(Ballista)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourName MachineBuff => new BehaviourName($"{nameof(Ballista)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourName AimBuff => new BehaviourName($"{nameof(Ballista)}{nameof(AimBuff)}".ToKebabCase());
        }

        public static class Radar
        {
            public static BehaviourName AssemblingBuildable => new BehaviourName($"{nameof(Radar)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourName MachineCounter => new BehaviourName($"{nameof(Radar)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourName MachineBuff => new BehaviourName($"{nameof(Radar)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourName ResonatingSweepBuff => new BehaviourName($"{nameof(Radar)}{nameof(ResonatingSweepBuff)}".ToKebabCase());
            public static BehaviourName RadioLocationBuff => new BehaviourName($"{nameof(Radar)}{nameof(RadioLocationBuff)}".ToKebabCase());
            public static BehaviourName RadioLocationFeatureBuff => new BehaviourName($"{nameof(Radar)}{nameof(RadioLocationFeatureBuff)}".ToKebabCase());
        }

        public static class Vessel
        {
            public static BehaviourName MachineCounter => new BehaviourName($"{nameof(Vessel)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourName MachineBuff => new BehaviourName($"{nameof(Vessel)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourName AbsorbentFieldInterceptDamage => new BehaviourName($"{nameof(Vessel)}{nameof(AbsorbentFieldInterceptDamage)}".ToKebabCase());
            public static BehaviourName FortifyDestroyBuff => new BehaviourName($"{nameof(Vessel)}{nameof(FortifyDestroyBuff)}".ToKebabCase());
            public static BehaviourName FortifyBuff => new BehaviourName($"{nameof(Vessel)}{nameof(FortifyBuff)}".ToKebabCase());
        }

        public static class Omen
        {
            public static BehaviourName RenditionPlacementBuff => new BehaviourName($"{nameof(Omen)}{nameof(RenditionPlacementBuff)}".ToKebabCase());
            public static BehaviourName RenditionInterceptDamage => new BehaviourName($"{nameof(Omen)}{nameof(RenditionInterceptDamage)}".ToKebabCase());
            public static BehaviourName RenditionBuffTimer => new BehaviourName($"{nameof(Omen)}{nameof(RenditionBuffTimer)}".ToKebabCase());
            public static BehaviourName RenditionBuffDeath => new BehaviourName($"{nameof(Omen)}{nameof(RenditionBuffDeath)}".ToKebabCase());
            public static BehaviourName RenditionBuffSlow => new BehaviourName($"{nameof(Omen)}{nameof(RenditionBuffSlow)}".ToKebabCase());
        }
    }
}
