using LowAgeData.Domain.Resources;

namespace LowAgeData.Domain.Common.Modifications
{
    public class ResourceModification : Modification
    {
        public ResourceModification(
            float amount,
            ResourceId resource) : base(Change.AddCurrent, amount)
        {
            Resource = resource;
        }

        public ResourceId Resource { get; }
    }
}
