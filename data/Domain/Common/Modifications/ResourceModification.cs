using low_age_data.Domain.Resources;

namespace low_age_data.Domain.Common.Modifications
{
    public class ResourceModification : Modification
    {
        public ResourceModification(
            Change change, 
            float amount,
            ResourceId resource) : base(change, amount)
        {
            Resource = resource;
        }

        public ResourceId Resource { get; }
    }
}
