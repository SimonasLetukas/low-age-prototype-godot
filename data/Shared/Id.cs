﻿using System.Collections.Generic;
using low_age_data.Domain.Abilities;

namespace low_age_data.Common
{
    public class Id : ValueObject<Id>
    {
        public override string ToString()
        {
            return Value;
        }
        
        protected Id(string value)
        {
            Value = value;
        }

        private string Value { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}