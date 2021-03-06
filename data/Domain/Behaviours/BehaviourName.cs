﻿using low_age_data.Common;

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
    }
}
