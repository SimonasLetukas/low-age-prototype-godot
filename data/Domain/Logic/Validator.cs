using System.Collections.Generic;

namespace low_age_data.Domain.Logic
{
    public class Validator
    {
        public Validator(
            IList<Condition> conditions)
        {
            Conditions = conditions;
            Type = $"{nameof(Validator)}";
        }

        protected Validator(
            IList<Condition> conditions,
            string type)
        {
            Type = type;
            Conditions = conditions;
        }

        public string Type { get; }
        public IList<Condition> Conditions { get; }
    }
}
