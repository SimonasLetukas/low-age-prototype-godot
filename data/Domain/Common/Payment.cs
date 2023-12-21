using low_age_data.Domain.Resources;

namespace low_age_data.Domain.Common
{
    public class Payment
    {
        public Payment(ResourceId resource, int amount = 0)
        {
            Resource = resource;
            Amount = amount;
        }

        public ResourceId Resource { get; }
        public int Amount { get; }
    }
}
