using low_age_data.Common;

namespace low_age_data.Domain.Behaviours
{
    public class BehaviourName : Name
    {
        private BehaviourName(string value) : base($"behaviour-{value}")
        {
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
            public static BehaviourName PowerDependencyBuff => new BehaviourName($"{nameof(Radar)}{nameof(PowerDependencyBuff)}".ToKebabCase());
            public static BehaviourName PowerDependencyBuffDisable => new BehaviourName($"{nameof(Radar)}{nameof(PowerDependencyBuffDisable)}".ToKebabCase());
        }
    }
}
