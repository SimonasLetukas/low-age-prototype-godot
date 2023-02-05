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
            public static BehaviourName HighGroundBuff => new($"{nameof(Shared)}{nameof(HighGroundBuff)}".ToKebabCase());
            public static BehaviourName PassiveIncomeIncome => new($"{nameof(Shared)}{nameof(PassiveIncomeIncome)}".ToKebabCase());
            public static BehaviourName ScrapsIncomeIncome => new($"{nameof(Shared)}{nameof(ScrapsIncomeIncome)}".ToKebabCase());
            public static BehaviourName CelestiumIncomeIncome => new($"{nameof(Shared)}{nameof(CelestiumIncomeIncome)}".ToKebabCase());

            public static class Revelators
            {
                public static BehaviourName BuildingBuildable => new($"{nameof(Shared)}{nameof(Revelators)}{nameof(BuildingBuildable)}".ToKebabCase());
                public static BehaviourName NoPopulationSpaceInterceptDamage => new($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceInterceptDamage)}".ToKebabCase());
            }
            
            public static class Uee
            {
                public static BehaviourName BuildingBuildable => new($"{nameof(Shared)}{nameof(Uee)}{nameof(BuildingBuildable)}".ToKebabCase());
                public static BehaviourName PowerGeneratorBuff => new($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorBuff)}".ToKebabCase());
                public static BehaviourName PowerDependencyBuff => new($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuff)}".ToKebabCase());
                public static BehaviourName PowerDependencyBuffDisable => new($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuffDisable)}".ToKebabCase());
                public static BehaviourName PowerDependencyBuffInactive => new($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuffInactive)}".ToKebabCase());
                public static BehaviourName PositiveFaithBuff => new($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithBuff)}".ToKebabCase());
            }
        }
        
        public static class Citadel
        {
            public static BehaviourName ExecutiveStashIncome => new($"{nameof(Citadel)}{nameof(ExecutiveStashIncome)}".ToKebabCase());
            public static BehaviourName AscendableAscendable => new($"{nameof(Citadel)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourName HighGroundHighGround => new($"{nameof(Citadel)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }

        public static class Hut
        {
            public static BehaviourName BuildingBuildable => new($"{nameof(Hut)}{nameof(BuildingBuildable)}".ToKebabCase());
        }
        
        public static class Obelisk
        {
            public static BehaviourName BuildingBuildable => new($"{nameof(Obelisk)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourName CelestiumDischargeBuffLong => new($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffLong)}".ToKebabCase());
            public static BehaviourName CelestiumDischargeBuffShort => new($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffShort)}".ToKebabCase());
            public static BehaviourName CelestiumDischargeBuffNegative => new($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffNegative)}".ToKebabCase());
        }
        
        public static class Shack
        {
            public static BehaviourName AccommodationIncome => new($"{nameof(Shack)}{nameof(AccommodationIncome)}".ToKebabCase());
        }
        
        public static class Smith
        {
            public static BehaviourName MeleeWeaponProductionIncome => new($"{nameof(Smith)}{nameof(MeleeWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Fletcher
        {
            public static BehaviourName RangedWeaponProductionIncome => new($"{nameof(Fletcher)}{nameof(RangedWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Alchemy
        {
            public static BehaviourName SpecialWeaponProductionIncome => new($"{nameof(Alchemy)}{nameof(SpecialWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Depot
        {
            public static BehaviourName WeaponStorageIncome => new($"{nameof(Depot)}{nameof(WeaponStorageIncome)}".ToKebabCase());
        }
        
        public static class Workshop
        {
        }
        
        public static class Outpost
        {
            public static BehaviourName AscendableAscendable => new($"{nameof(Outpost)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourName HighGroundHighGround => new($"{nameof(Outpost)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }
        
        public static class Barricade
        {
            public static BehaviourName ProtectiveShieldBuff => new($"{nameof(Barricade)}{nameof(ProtectiveShieldBuff)}".ToKebabCase());
            public static BehaviourName DecomposeBuff => new($"{nameof(Barricade)}{nameof(DecomposeBuff)}".ToKebabCase());
        }
        
        public static class BatteryCore
        {
            public static BehaviourName PowerGridMaskProvider => new($"{nameof(BatteryCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourName FusionCoreUpgradeBuff => new($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeBuff)}".ToKebabCase());
        }
        
        public static class FusionCore
        {
            public static BehaviourName PowerGridMaskProvider => new($"{nameof(FusionCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourName CelestiumCoreUpgradeBuff => new($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeBuff)}".ToKebabCase());
        }
        
        public static class CelestiumCore
        {
            public static BehaviourName PowerGridMaskProvider => new($"{nameof(CelestiumCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
        }
        
        public static class Collector
        {
            public static BehaviourName BuildingBuildable => new($"{nameof(Collector)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourName DirectTransitSystemInactiveBuff => new($"{nameof(Collector)}{nameof(DirectTransitSystemInactiveBuff)}".ToKebabCase());
            public static BehaviourName DirectTransitSystemActiveIncome => new($"{nameof(Collector)}{nameof(DirectTransitSystemActiveIncome)}".ToKebabCase());
        }
        
        public static class Extractor
        {
            public static BehaviourName BuildingBuildable => new($"{nameof(Extractor)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourName ReinforcedInfrastructureInactiveBuff => new($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureInactiveBuff)}".ToKebabCase());
            public static BehaviourName ReinforcedInfrastructureActiveBuff => new($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureActiveBuff)}".ToKebabCase());
        }

        public static class PowerPole
        {
            public static BehaviourName PowerGridMaskProvider => new($"{nameof(PowerPole)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourName ExcessDistributionBuff => new($"{nameof(PowerPole)}{nameof(ExcessDistributionBuff)}".ToKebabCase());
            public static BehaviourName PowerGridImprovedMaskProvider => new($"{nameof(PowerPole)}{nameof(PowerGridImprovedMaskProvider)}".ToKebabCase());
        }
        
        public static class Temple
        {
            public static BehaviourName KeepingTheFaithBuff => new($"{nameof(Temple)}{nameof(KeepingTheFaithBuff)}".ToKebabCase());
            public static BehaviourName KeepingTheFaithIncome => new($"{nameof(Temple)}{nameof(KeepingTheFaithIncome)}".ToKebabCase());
        }
        
        public static class Wall
        {
            public static BehaviourName BuildingBuildable => new($"{nameof(Wall)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourName HighGroundHighGround => new($"{nameof(Wall)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }

        public static class Stairs
        {
            public static BehaviourName AscendableAscendable => new($"{nameof(Stairs)}{nameof(AscendableAscendable)}".ToKebabCase());
        }

        public static class Gate
        {
            public static BehaviourName HighGroundHighGround => new($"{nameof(Gate)}{nameof(HighGroundHighGround)}".ToKebabCase());
            public static BehaviourName AscendableAscendable => new($"{nameof(Gate)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourName EntranceMovementBlock => new($"{nameof(Gate)}{nameof(EntranceMovementBlock)}".ToKebabCase());
        }

        public static class Leader
        {
            public static BehaviourName AllForOneBuff => new($"{nameof(Leader)}{nameof(AllForOneBuff)}".ToKebabCase());
            public static BehaviourName MenacingPresenceBuff => new($"{nameof(Leader)}{nameof(MenacingPresenceBuff)}".ToKebabCase());
            public static BehaviourName OneForAllObeliskBuff => new($"{nameof(Leader)}{nameof(OneForAllObeliskBuff)}".ToKebabCase());
            public static BehaviourName OneForAllHealBuff => new($"{nameof(Leader)}{nameof(OneForAllHealBuff)}".ToKebabCase());
        }

        public static class Slave
        {
            public static BehaviourName RepairStructureBuff => new($"{nameof(Slave)}{nameof(RepairStructureBuff)}".ToKebabCase());
            public static BehaviourName RepairWait => new($"{nameof(Slave)}{nameof(RepairWait)}".ToKebabCase());
            public static BehaviourName ManualLabourBuff => new($"{nameof(Slave)}{nameof(ManualLabourBuff)}".ToKebabCase());
            public static BehaviourName ManualLabourWait => new($"{nameof(Slave)}{nameof(ManualLabourWait)}".ToKebabCase());
        }

        public static class Quickdraw
        {
            public static BehaviourName DoubleshotExtraAttack => new($"{nameof(Quickdraw)}{nameof(DoubleshotExtraAttack)}".ToKebabCase());
            public static BehaviourName CrippleBuff => new($"{nameof(Quickdraw)}{nameof(CrippleBuff)}".ToKebabCase());
        }

        public static class Gorger
        {
            public static BehaviourName FanaticSuicideBuff => new($"{nameof(Gorger)}{nameof(FanaticSuicideBuff)}".ToKebabCase());
        }

        public static class Camou
        {
            public static BehaviourName SilentAssassinBuff => new($"{nameof(Camou)}{nameof(SilentAssassinBuff)}".ToKebabCase());
            public static BehaviourName ClimbWait => new($"{nameof(Camou)}{nameof(ClimbWait)}".ToKebabCase());
            public static BehaviourName ClimbBuff => new($"{nameof(Camou)}{nameof(ClimbBuff)}".ToKebabCase());
        }

        public static class Shaman
        {
            public static BehaviourName WondrousGooFeatureWait => new($"{nameof(Shaman)}{nameof(WondrousGooFeatureWait)}".ToKebabCase());
            public static BehaviourName WondrousGooFeatureBuff => new($"{nameof(Shaman)}{nameof(WondrousGooFeatureBuff)}".ToKebabCase());
            public static BehaviourName WondrousGooBuff => new($"{nameof(Shaman)}{nameof(WondrousGooBuff)}".ToKebabCase());
        }

        public static class Pyre
        {
            public static BehaviourName CargoTether => new($"{nameof(Pyre)}{nameof(CargoTether)}".ToKebabCase());
            public static BehaviourName CargoWallOfFlamesBuff => new($"{nameof(Pyre)}{nameof(CargoWallOfFlamesBuff)}".ToKebabCase());
            public static BehaviourName WallOfFlamesBuff => new($"{nameof(Pyre)}{nameof(WallOfFlamesBuff)}".ToKebabCase());
            public static BehaviourName PhantomMenaceBuff => new($"{nameof(Pyre)}{nameof(PhantomMenaceBuff)}".ToKebabCase());
        }

        public static class Roach
        {
            public static BehaviourName DegradingCarapaceBuff => new($"{nameof(Roach)}{nameof(DegradingCarapaceBuff)}".ToKebabCase());
            public static BehaviourName DegradingCarapacePeriodicDamageBuff => new($"{nameof(Roach)}{nameof(DegradingCarapacePeriodicDamageBuff)}".ToKebabCase());
        }

        public static class Parasite
        {
            public static BehaviourName ParalysingGraspTether => new($"{nameof(Parasite)}{nameof(ParalysingGraspTether)}".ToKebabCase());
            public static BehaviourName ParalysingGraspBuff => new($"{nameof(Parasite)}{nameof(ParalysingGraspBuff)}".ToKebabCase());
            public static BehaviourName ParalysingGraspSelfBuff => new($"{nameof(Parasite)}{nameof(ParalysingGraspSelfBuff)}".ToKebabCase());
        }

        public static class Horrior
        {
            public static BehaviourName ExpertFormationBuff => new($"{nameof(Horrior)}{nameof(ExpertFormationBuff)}".ToKebabCase());
            public static BehaviourName MountWait => new($"{nameof(Horrior)}{nameof(MountWait)}".ToKebabCase());
            public static BehaviourName MountBuff => new($"{nameof(Horrior)}{nameof(MountBuff)}".ToKebabCase());
        }

        public static class Marksman
        {
            public static BehaviourName CriticalMarkBuff => new($"{nameof(Marksman)}{nameof(CriticalMarkBuff)}".ToKebabCase());
        }
        
        public static class Surfer
        {
            public static BehaviourName DismountBuff => new($"{nameof(Surfer)}{nameof(DismountBuff)}".ToKebabCase());
        }

        public static class Mortar
        {
            public static BehaviourName DeadlyAmmunitionAmmunition => new($"{nameof(Mortar)}{nameof(DeadlyAmmunitionAmmunition)}".ToKebabCase());
            public static BehaviourName ReloadWait => new($"{nameof(Mortar)}{nameof(ReloadWait)}".ToKebabCase());
            public static BehaviourName ReloadBuff => new($"{nameof(Mortar)}{nameof(ReloadBuff)}".ToKebabCase());
            public static BehaviourName PiercingBlastBuff => new($"{nameof(Mortar)}{nameof(PiercingBlastBuff)}".ToKebabCase());
        }

        public static class Hawk
        {
            public static BehaviourName TacticalGogglesBuff => new($"{nameof(Hawk)}{nameof(TacticalGogglesBuff)}".ToKebabCase());
            public static BehaviourName LeadershipBuff => new($"{nameof(Hawk)}{nameof(LeadershipBuff)}".ToKebabCase());
            public static BehaviourName HealthKitBuff => new($"{nameof(Hawk)}{nameof(HealthKitBuff)}".ToKebabCase());
            public static BehaviourName HealthKitHealBuff => new($"{nameof(Hawk)}{nameof(HealthKitHealBuff)}".ToKebabCase());
        }

        public static class Engineer
        {
            public static BehaviourName OperateBuff => new($"{nameof(Engineer)}{nameof(OperateBuff)}".ToKebabCase());
            public static BehaviourName RepairStructureOrMachineBuff => new($"{nameof(Engineer)}{nameof(RepairStructureOrMachineBuff)}".ToKebabCase());
            public static BehaviourName RepairHorriorBuff => new($"{nameof(Engineer)}{nameof(RepairHorriorBuff)}".ToKebabCase());
            public static BehaviourName RepairWait => new($"{nameof(Engineer)}{nameof(RepairWait)}".ToKebabCase());
        }

        public static class Cannon
        {
            public static BehaviourName AssemblingBuildable => new($"{nameof(Cannon)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourName MachineCounter => new($"{nameof(Cannon)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourName MachineBuff => new($"{nameof(Cannon)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourName HeatUpDangerZoneBuff => new($"{nameof(Cannon)}{nameof(HeatUpDangerZoneBuff)}".ToKebabCase());
            public static BehaviourName HeatUpWait => new($"{nameof(Cannon)}{nameof(HeatUpWait)}".ToKebabCase());
        }

        public static class Ballista
        {
            public static BehaviourName AssemblingBuildable => new($"{nameof(Ballista)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourName MachineCounter => new($"{nameof(Ballista)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourName MachineBuff => new($"{nameof(Ballista)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourName AimBuff => new($"{nameof(Ballista)}{nameof(AimBuff)}".ToKebabCase());
        }

        public static class Radar
        {
            public static BehaviourName AssemblingBuildable => new($"{nameof(Radar)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourName MachineCounter => new($"{nameof(Radar)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourName MachineBuff => new($"{nameof(Radar)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourName ResonatingSweepBuff => new($"{nameof(Radar)}{nameof(ResonatingSweepBuff)}".ToKebabCase());
            public static BehaviourName RadioLocationBuff => new($"{nameof(Radar)}{nameof(RadioLocationBuff)}".ToKebabCase());
            public static BehaviourName RadioLocationFeatureBuff => new($"{nameof(Radar)}{nameof(RadioLocationFeatureBuff)}".ToKebabCase());
        }

        public static class Vessel
        {
            public static BehaviourName MachineCounter => new($"{nameof(Vessel)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourName MachineBuff => new($"{nameof(Vessel)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourName AbsorbentFieldInterceptDamage => new($"{nameof(Vessel)}{nameof(AbsorbentFieldInterceptDamage)}".ToKebabCase());
            public static BehaviourName FortifyDestroyBuff => new($"{nameof(Vessel)}{nameof(FortifyDestroyBuff)}".ToKebabCase());
            public static BehaviourName FortifyBuff => new($"{nameof(Vessel)}{nameof(FortifyBuff)}".ToKebabCase());
        }

        public static class Omen
        {
            public static BehaviourName RenditionPlacementBuff => new($"{nameof(Omen)}{nameof(RenditionPlacementBuff)}".ToKebabCase());
            public static BehaviourName RenditionInterceptDamage => new($"{nameof(Omen)}{nameof(RenditionInterceptDamage)}".ToKebabCase());
            public static BehaviourName RenditionBuffTimer => new($"{nameof(Omen)}{nameof(RenditionBuffTimer)}".ToKebabCase());
            public static BehaviourName RenditionBuffDeath => new($"{nameof(Omen)}{nameof(RenditionBuffDeath)}".ToKebabCase());
            public static BehaviourName RenditionBuffSlow => new($"{nameof(Omen)}{nameof(RenditionBuffSlow)}".ToKebabCase());
        }
    }
}
