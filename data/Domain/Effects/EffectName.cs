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

        public static class Pyre
        {
            public static EffectName CargoCreateEntity => new EffectName($"{nameof(Pyre)}{nameof(CargoCreateEntity)}".ToKebabCase());
            public static EffectName WallOfFlamesCreateEntity => new EffectName($"{nameof(Pyre)}{nameof(WallOfFlamesCreateEntity)}".ToKebabCase());
            public static EffectName WallOfFlamesDestroy => new EffectName($"{nameof(Pyre)}{nameof(WallOfFlamesDestroy)}".ToKebabCase());
            public static EffectName WallOfFlamesDamage => new EffectName($"{nameof(Pyre)}{nameof(WallOfFlamesDamage)}".ToKebabCase());
            public static EffectName PhantomMenaceApplyBehaviour => new EffectName($"{nameof(Pyre)}{nameof(PhantomMenaceApplyBehaviour)}".ToKebabCase());
        }

        public static class BigBadBull
        {
            public static EffectName UnleashTheRageSearch => new EffectName($"{nameof(BigBadBull)}{nameof(UnleashTheRageSearch)}".ToKebabCase());
            public static EffectName UnleashTheRageDamage => new EffectName($"{nameof(BigBadBull)}{nameof(UnleashTheRageDamage)}".ToKebabCase());
            public static EffectName UnleashTheRageForce => new EffectName($"{nameof(BigBadBull)}{nameof(UnleashTheRageForce)}".ToKebabCase());
            public static EffectName UnleashTheRageForceDamage => new EffectName($"{nameof(BigBadBull)}{nameof(UnleashTheRageForceDamage)}".ToKebabCase());
        }

        public static class Mummy
        {
            public static EffectName SpawnRoachCreateEntity => new EffectName($"{nameof(Mummy)}{nameof(SpawnRoachCreateEntity)}".ToKebabCase());
            public static EffectName LeapOfHungerModifyAbility => new EffectName($"{nameof(Mummy)}{nameof(LeapOfHungerModifyAbility)}".ToKebabCase());
        }

        public static class Roach
        {
            public static EffectName DegradingCarapaceApplyBehaviour => new EffectName($"{nameof(Roach)}{nameof(DegradingCarapaceApplyBehaviour)}".ToKebabCase());
            public static EffectName DegradingCarapacePeriodicApplyBehaviour => new EffectName($"{nameof(Roach)}{nameof(DegradingCarapacePeriodicApplyBehaviour)}".ToKebabCase());
            public static EffectName DegradingCarapaceSelfDamage => new EffectName($"{nameof(Roach)}{nameof(DegradingCarapaceSelfDamage)}".ToKebabCase());
            public static EffectName CorrosiveSpitDamage => new EffectName($"{nameof(Roach)}{nameof(CorrosiveSpitDamage)}".ToKebabCase());
        }

        public static class Parasite
        {
            public static EffectName ParalysingGraspApplyTetherBehaviour => new EffectName($"{nameof(Parasite)}{nameof(ParalysingGraspApplyTetherBehaviour)}".ToKebabCase());
            public static EffectName ParalysingGraspApplyAttackBehaviour => new EffectName($"{nameof(Parasite)}{nameof(ParalysingGraspApplyAttackBehaviour)}".ToKebabCase());
            public static EffectName ParalysingGraspApplySelfBehaviour => new EffectName($"{nameof(Parasite)}{nameof(ParalysingGraspApplySelfBehaviour)}".ToKebabCase());
        }

        public static class Horrior
        {
            public static EffectName ExpertFormationSearch => new EffectName($"{nameof(Horrior)}{nameof(ExpertFormationSearch)}".ToKebabCase());
            public static EffectName ExpertFormationApplyBehaviour => new EffectName($"{nameof(Horrior)}{nameof(ExpertFormationApplyBehaviour)}".ToKebabCase());
            public static EffectName MountApplyBehaviour => new EffectName($"{nameof(Horrior)}{nameof(MountApplyBehaviour)}".ToKebabCase());
            public static EffectName MountCreateEntity => new EffectName($"{nameof(Horrior)}{nameof(MountCreateEntity)}".ToKebabCase());
            public static EffectName MountDestroy => new EffectName($"{nameof(Horrior)}{nameof(MountDestroy)}".ToKebabCase());
        }

        public static class Marksman
        {
            public static EffectName CriticalMarkApplyBehaviour => new EffectName($"{nameof(Marksman)}{nameof(CriticalMarkApplyBehaviour)}".ToKebabCase());
            public static EffectName CriticalMarkDamage => new EffectName($"{nameof(Marksman)}{nameof(CriticalMarkDamage)}".ToKebabCase());
        }

        public static class Surfer
        {
            public static EffectName DismountApplyBehaviour => new EffectName($"{nameof(Surfer)}{nameof(DismountApplyBehaviour)}".ToKebabCase());
            public static EffectName DismountCreateEntity => new EffectName($"{nameof(Surfer)}{nameof(DismountCreateEntity)}".ToKebabCase());
        }
        
        public static class Mortar
        {
            public static EffectName DeadlyAmmunitionApplyBehaviour => new EffectName($"{nameof(Mortar)}{nameof(DeadlyAmmunitionApplyBehaviour)}".ToKebabCase());
            public static EffectName DeadlyAmmunitionSearch => new EffectName($"{nameof(Mortar)}{nameof(DeadlyAmmunitionSearch)}".ToKebabCase());
            public static EffectName DeadlyAmmunitionDamage => new EffectName($"{nameof(Mortar)}{nameof(DeadlyAmmunitionDamage)}".ToKebabCase());
            public static EffectName ReloadApplyBehaviour => new EffectName($"{nameof(Mortar)}{nameof(ReloadApplyBehaviour)}".ToKebabCase());
            public static EffectName ReloadReload => new EffectName($"{nameof(Mortar)}{nameof(ReloadReload)}".ToKebabCase());
            public static EffectName PiercingBlastApplyBehaviour => new EffectName($"{nameof(Mortar)}{nameof(PiercingBlastApplyBehaviour)}".ToKebabCase());
        }

        public static class Hawk
        {
            public static EffectName TacticalGogglesApplyBehaviour => new EffectName($"{nameof(Hawk)}{nameof(TacticalGogglesApplyBehaviour)}".ToKebabCase());
            public static EffectName LeadershipApplyBehaviour => new EffectName($"{nameof(Hawk)}{nameof(LeadershipApplyBehaviour)}".ToKebabCase());
            public static EffectName HealthKitApplyBehaviour => new EffectName($"{nameof(Hawk)}{nameof(HealthKitApplyBehaviour)}".ToKebabCase());
            public static EffectName HealthKitSearch => new EffectName($"{nameof(Hawk)}{nameof(HealthKitSearch)}".ToKebabCase());
            public static EffectName HealthKitHealApplyBehaviour => new EffectName($"{nameof(Hawk)}{nameof(HealthKitHealApplyBehaviour)}".ToKebabCase());
        }
    }
}
