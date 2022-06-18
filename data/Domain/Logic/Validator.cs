using System.Collections.Generic;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// Used in logic operations to validate if <b>any</b> items from the list of <see cref="Conditions"/> are true.
    /// </summary>
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
        
        /// <summary>
        /// <see cref="Validator"/> succeeds if <b>any</b> of the <see cref="Conditions"/> are true.
        /// </summary>
        public IList<Condition> Conditions { get; }
        
        /// <summary>
        /// Filters for the <see cref="Conditions"/> to apply (empty list allows <b>any</b>).
        /// </summary>
        public IList<Flag> FilterFlags { get; } 
    }
}
