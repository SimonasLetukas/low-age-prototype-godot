using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Flags
{
    public class Flag : ValueObject<Flag>
    {
        public override string ToString()
        {
            return $"{nameof(Flag)}.{Value}";
        }

        public static class Effect
        {
            public static class ModifyPlayer
            {
                public static Flag GameLost => new Flag(Flags.EffectModifyPlayerGameLost);
            }

            public static class Search
            {
                public static Flag AppliedOnEnter => new Flag(Flags.EffectSearchAppliedOnEnter);
                public static Flag AppliedOnAction => new Flag(Flags.EffectSearchAppliedOnAction);
                public static Flag RemovedOnExit => new Flag(Flags.EffectSearchRemovedOnExit);
            }
        }

        public static class Filter
        {
            public static Flag Self => new Flag(Flags.FilterSelf);
            public static Flag Ally => new Flag(Flags.FilterAlly);
            public static Flag Enemy => new Flag(Flags.FilterEnemy);
            public static Flag Unit => new Flag(Flags.FilterUnit);
        }

        private Flag(Flags @enum)
        {
            Value = @enum;
        }

        private Flags Value { get; }

        private enum Flags
        {
            EffectModifyPlayerGameLost,
            EffectSearchAppliedOnEnter,
            EffectSearchAppliedOnAction,
            EffectSearchRemovedOnExit,
            FilterSelf,
            FilterAlly,
            FilterEnemy,
            FilterUnit
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
