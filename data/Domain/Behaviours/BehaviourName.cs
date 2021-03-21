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
            //public static BehaviourName MenacingPresence => new BehaviourName($"{nameof(Leader)}{nameof(MenacingPresence)}".ToKebabCase());
            //public static BehaviourName OneForAll => new BehaviourName($"{nameof(Leader)}{nameof(OneForAll)}".ToKebabCase());
        }
    }
}
