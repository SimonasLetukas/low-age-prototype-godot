namespace low_age_data.Domain.Shared
{
    public class Resource
    {
        public Resource(Resources type, int amount = 0)
        {
            Type = type;
            Amount = amount;
        }

        public Resources Type { get; }
        public int Amount { get; }
    }
}
