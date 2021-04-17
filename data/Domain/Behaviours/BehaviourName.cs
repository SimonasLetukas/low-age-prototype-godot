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
            public static BehaviourName FanaticSuicideDestroy => new BehaviourName($"{nameof(Gorger)}{nameof(FanaticSuicideDestroy)}".ToKebabCase());
        }

        public static class Camou
        {
            public static BehaviourName SilentAssassinBuff => new BehaviourName($"{nameof(Camou)}{nameof(SilentAssassinBuff)}".ToKebabCase());
            public static BehaviourName ClimbWait => new BehaviourName($"{nameof(Camou)}{nameof(ClimbWait)}".ToKebabCase());
            public static BehaviourName ClimbBuff => new BehaviourName($"{nameof(Camou)}{nameof(ClimbBuff)}".ToKebabCase());
        }
    }
}
