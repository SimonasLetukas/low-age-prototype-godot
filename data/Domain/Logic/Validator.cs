using System.Collections.Generic;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Logic
{
    public class Validator
    {
        public Validator(IList<Condition> conditions, IList<Flag>? filterFlags = null)
        {
            Conditions = conditions;
            Type = $"{nameof(Validator)}";
            FilterFlags = filterFlags ?? new List<Flag>();
        }

        protected Validator(
            IList<Condition> conditions,
            string type)
        {
            Type = type;
            Conditions = conditions;
            FilterFlags = new List<Flag>();
        }

        public string Type { get; }
        public IList<Condition> Conditions { get; }
        
        /// <summary>
        /// Filters for the conditions to apply (empty list allows any).
        /// </summary>
        public IList<Flag> FilterFlags { get; } 
    }
}
