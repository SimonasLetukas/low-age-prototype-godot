using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared
{
    public class Selection<T> where T : Name
    {
        public Selection(
            T name, 
            IList<Cost>? cost, 
            IList<ResearchName>? researchNeeded = null, 
            bool? grayOutIfAlreadyExists = null)
        {
            Name = name;
            Cost = cost ?? new List<Cost>();
            ResearchNeeded = researchNeeded ?? new List<ResearchName>();
            GrayOutIfAlreadyExists = grayOutIfAlreadyExists ?? false;
        }
        
        public T Name { get; }
        public IList<Cost> Cost { get; }
        
        /// <summary>
        /// List of <see cref="ResearchName"/> that all need to be true for this <see cref="Selection{T}"/> item to
        /// be available (not grayed out).
        /// </summary>
        public IList<ResearchName> ResearchNeeded { get; }

        /// <summary>
        /// If true, the selection is grayed out if the <see cref="Name"/> already exists in the game:
        /// <see cref="EntityName"/> is on the map, <see cref="ResearchName"/> has been researched, etc. 
        /// </summary>
        public bool GrayOutIfAlreadyExists { get; }
    }
}