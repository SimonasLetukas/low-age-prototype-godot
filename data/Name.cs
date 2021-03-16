using System.Collections.Generic;
using Newtonsoft.Json;

namespace low_age_data
{
    [JsonConverter(typeof(ToStringJsonConverter))]
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
