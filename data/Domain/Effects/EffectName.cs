using low_age_data.Common;

namespace low_age_data.Domain.Effects
{
    public class EffectName : Name
    {
        protected EffectName(string value) : base($"effect-{value}")
        {
        }

        public static class Leader
        {
            public static EffectName AllForOneApplyBehaviour => new EffectName($"{nameof(Leader)}{nameof(AllForOneApplyBehaviour)}".ToKebabCase());
            public static EffectName AllForOnePlayerLoses => new EffectName($"{nameof(Leader)}{nameof(AllForOnePlayerLoses)}".ToKebabCase());
            public static EffectName MenacingPresenceSearch => new EffectName($"{nameof(Leader)}{nameof(MenacingPresenceSearch)}".ToKebabCase());
            public static EffectName MenacingPresenceApplyBehaviour => new EffectName($"{nameof(Leader)}{nameof(MenacingPresenceApplyBehaviour)}".ToKebabCase());
            //public static BehaviourName OneForAll => new BehaviourName($"{nameof(Leader)}{nameof(OneForAll)}".ToKebabCase());
        }
    }
}
