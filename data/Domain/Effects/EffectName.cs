using low_age_data.Common;

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
    }
}
