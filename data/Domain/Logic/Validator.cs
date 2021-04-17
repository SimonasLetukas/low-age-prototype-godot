using System.Collections.Generic;

namespace low_age_data.Domain.Logic
{
    public class Validator
    {
        public Validator(
            IList<Condition> conditions,
            string? type = null)
        {
            Type = type ?? $"{nameof(Validator)}";
            Conditions = conditions;
        }

        public string Type { get; }
        public IList<Condition> Conditions { get; }
    }
}
