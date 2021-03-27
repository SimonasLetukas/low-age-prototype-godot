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
    }
}
