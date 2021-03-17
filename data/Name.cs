using System.Collections.Generic;

namespace low_age_data
{
    public abstract class Name : ValueObject<Name>
    {
        public override string ToString()
        {
            return Value;
        }

        protected Name(string value)
        {
            Value = value;
        }

        private string Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
