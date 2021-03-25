using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class TurnPhase : ValueObject<TurnPhase>
    {
        public override string ToString()
        {
            return $"{nameof(TurnPhase)}.{Value}";
        }

        public static TurnPhase Passive => new TurnPhase(TurnPhases.Passive);
        public static TurnPhase Planning => new TurnPhase(TurnPhases.Planning);
        public static TurnPhase Action => new TurnPhase(TurnPhases.Action);

        private TurnPhase(TurnPhases @enum)
        {
            Value = @enum;
        }

        private TurnPhases Value { get; }

        private enum TurnPhases
        {
            Passive = 0,
            Planning = 1,
            Action = 2
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
