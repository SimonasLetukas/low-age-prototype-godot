using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Durations
{
    public class EndsAt : ValueObject<EndsAt>
    {
        public override string ToString()
        {
            return $"{nameof(EndsAt)}.{Value}";
        }

        public static EndsAt Death => new EndsAt(Durations.Death);

        public static class StartOf
        {
            public static class Next
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfNextPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.StartOfNextActionPhase);
                public static EndsAt Action => new EndsAt(Durations.StartOfNextAction);
            }

            public static class Second
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfSecondPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.StartOfSecondActionPhase);
                public static EndsAt Action => new EndsAt(Durations.StartOfSecondAction);
            }

            public static class Third
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfThirdPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.StartOfThirdActionPhase);
                public static EndsAt Action => new EndsAt(Durations.StartOfThirdAction);
            }

            public static class Fourth
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfFourthPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.StartOfFourthActionPhase);
                public static EndsAt Action => new EndsAt(Durations.StartOfFourthAction);
            }
        }

        public static class EndOf
        {
            public static class This
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfThisPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfThisActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfThisAction);
            }

            public static class Next
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfNextPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfNextActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfNextAction);
            }

            public static class Second
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfSecondPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfSecondActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfSecondAction);
            }

            public static class Third
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfThirdPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfThirdActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfThirdAction);
            }

            public static class Fourth
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfFourthPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfFourthActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfFourthAction);
            }
        }

        private EndsAt(Durations @enum)
        {
            Value = @enum;
        }

        private Durations Value { get; }

        private enum Durations
        {
            Death,
            StartOfNextPlanning,
            StartOfNextActionPhase,
            StartOfNextAction,
            StartOfSecondPlanning,
            StartOfSecondActionPhase,
            StartOfSecondAction,
            StartOfThirdPlanning,
            StartOfThirdActionPhase,
            StartOfThirdAction,
            StartOfFourthPlanning,
            StartOfFourthActionPhase,
            StartOfFourthAction,
            EndOfThisPlanning,
            EndOfThisActionPhase,
            EndOfThisAction,
            EndOfNextPlanning,
            EndOfNextActionPhase,
            EndOfNextAction,
            EndOfSecondPlanning,
            EndOfSecondActionPhase,
            EndOfSecondAction,
            EndOfThirdPlanning,
            EndOfThirdActionPhase,
            EndOfThirdAction,
            EndOfFourthPlanning,
            EndOfFourthActionPhase,
            EndOfFourthAction
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
