using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared
{
    public class Selection<T> where T : Id
    {
        public Selection(
            T name, 
            string? description = null,
            IList<Payment>? cost = null, 
            IList<ResearchId>? researchNeeded = null, 
            bool? grayOutIfAlreadyExists = null)
        {
            Name = name;
            Description = description ?? string.Empty;
            Cost = cost ?? new List<Payment>();
            ResearchNeeded = researchNeeded ?? new List<ResearchId>();
            GrayOutIfAlreadyExists = grayOutIfAlreadyExists ?? false;
        }
        
        public T Name { get; }
        public string Description { get; }
        public IList<Payment> Cost { get; }
        
        /// <summary>
        /// List of <see cref="ResearchId"/> that all need to be true for this <see cref="Selection{T}"/> item to
        /// be available (not grayed out).
        /// </summary>
        public IList<ResearchId> ResearchNeeded { get; }

        /// <summary>
        /// If true, the selection is grayed out if the <see cref="Name"/> already exists in the game:
        /// <see cref="EntityId"/> is on the map, <see cref="ResearchId"/> has been researched, etc. 
        /// </summary>
        public bool GrayOutIfAlreadyExists { get; }
    }
}