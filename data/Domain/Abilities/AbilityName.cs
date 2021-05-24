using low_age_data.Common;

namespace low_age_data.Domain.Abilities
{
    public class AbilityName : Name
    {
        private AbilityName(string value) : base($"ability-{value}")
        {
        }

        public static class Leader
        {
            public static AbilityName AllForOne => new AbilityName($"{nameof(Leader)}{nameof(AllForOne)}".ToKebabCase());
            public static AbilityName MenacingPresence => new AbilityName($"{nameof(Leader)}{nameof(MenacingPresence)}".ToKebabCase());
            public static AbilityName OneForAll => new AbilityName($"{nameof(Leader)}{nameof(OneForAll)}".ToKebabCase());
        }

        public static class Slave
        {
            public static AbilityName Build => new AbilityName($"{nameof(Slave)}{nameof(Build)}".ToKebabCase());
            public static AbilityName Repair => new AbilityName($"{nameof(Slave)}{nameof(Repair)}".ToKebabCase());
            public static AbilityName ManualLabour => new AbilityName($"{nameof(Slave)}{nameof(ManualLabour)}".ToKebabCase());
        }

        public static class Quickdraw
        {
            public static AbilityName Doubleshot => new AbilityName($"{nameof(Quickdraw)}{nameof(Doubleshot)}".ToKebabCase());
            public static AbilityName Cripple => new AbilityName($"{nameof(Quickdraw)}{nameof(Cripple)}".ToKebabCase());
        }

        public static class Gorger
        {
            public static AbilityName FanaticSuicide => new AbilityName($"{nameof(Gorger)}{nameof(FanaticSuicide)}".ToKebabCase());
            public static AbilityName FanaticSuicidePassive => new AbilityName($"{nameof(Gorger)}{nameof(FanaticSuicidePassive)}".ToKebabCase());
        }

        public static class Camou
        {
            public static AbilityName SilentAssassin => new AbilityName($"{nameof(Camou)}{nameof(SilentAssassin)}".ToKebabCase());
            public static AbilityName Climb => new AbilityName($"{nameof(Camou)}{nameof(Climb)}".ToKebabCase());
            public static AbilityName ClimbPassive => new AbilityName($"{nameof(Camou)}{nameof(ClimbPassive)}".ToKebabCase());
        }

        public static class Shaman
        {
            public static AbilityName WondrousGoo => new AbilityName($"{nameof(Shaman)}{nameof(WondrousGoo)}".ToKebabCase());
        }

        public static class Pyre
        {
            public static AbilityName WallOfFlames => new AbilityName($"{nameof(Pyre)}{nameof(WallOfFlames)}".ToKebabCase());
            public static AbilityName PhantomMenace => new AbilityName($"{nameof(Pyre)}{nameof(PhantomMenace)}".ToKebabCase());
        }

        public static class BigBadBull
        {
            public static AbilityName UnleashTheRage => new AbilityName($"{nameof(BigBadBull)}{nameof(UnleashTheRage)}".ToKebabCase());
        }
    }
}
