using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Factions : ValueObject<Factions>
    {
        public override string ToString()
        {
            return $"{nameof(Factions)}.{Value}";
        }

        public static Factions Revelators => new(FactionsEnum.Revelators);
        public static Factions UEE => new(FactionsEnum.UEE);

        private Factions(FactionsEnum @enum)
        {
            Value = @enum;
        }

        private FactionsEnum Value { get; }

        private enum FactionsEnum
        {
            UEE = 0,
            Revelators = 1
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
