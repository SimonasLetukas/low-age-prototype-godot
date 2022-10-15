using low_age_data.Domain.Resources;

namespace low_age_data.Domain.Shared
{
    public class Cost
    {
        public Cost(ResourceName type, int amount = 0)
        {
            Type = type;
            Amount = amount;
        }

        public ResourceName Type { get; }
        public int Amount { get; }
    }
}
