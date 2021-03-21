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
        }

        
        public static class Filter
        {
            public static Flag Self => new Flag(Flags.FilterSelf);
            public static Flag Ally => new Flag(Flags.FilterAlly);
            public static Flag Enemy => new Flag(Flags.FilterEnemy);
        }

        private Flag(Flags @enum)
        {
            Value = @enum;
        }

        private Flags Value { get; }

        private enum Flags
        {
            EffectModifyPlayerGameLost,
            FilterSelf,
            FilterAlly,
            FilterEnemy
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
