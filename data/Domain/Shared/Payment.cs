using low_age_data.Domain.Resources;

namespace low_age_data.Domain.Shared
{
    public class Payment
    {
        public Payment(ResourceName resource, int amount = 0)
        {
            Resource = resource;
            Amount = amount;
        }

        public ResourceName Resource { get; }
        public int Amount { get; }
    }
}
