﻿using low_age_data.Common;

namespace low_age_data.Domain.Effects
{
    public class EffectName : Name
    {
        private EffectName(string value) : base($"effect-{value}")
        {
        }

        public static class Leader
        {
            public static EffectName AllForOneApplyBehaviour => new EffectName($"{nameof(Leader)}{nameof(AllForOneApplyBehaviour)}".ToKebabCase());
            public static EffectName AllForOnePlayerLoses => new EffectName($"{nameof(Leader)}{nameof(AllForOnePlayerLoses)}".ToKebabCase());
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
    }
}
