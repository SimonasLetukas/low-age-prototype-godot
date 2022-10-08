using System.Collections.Generic;
using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared
{
    public class Selection
    {
        public Selection(
            EntityName entityName,
            IList<Resource>? cost = null)
        {
            EntityName = entityName;
            Cost = cost ?? new List<Resource>();
        }
        
        public EntityName EntityName { get; }
        public IList<Resource> Cost { get; }
    }
}