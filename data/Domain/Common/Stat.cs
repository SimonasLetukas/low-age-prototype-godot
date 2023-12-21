namespace low_age_data.Domain.Common
{
    public abstract class Stat
    {
        protected Stat(int maxAmount, bool hasCurrent)
        {
            MaxAmount = maxAmount;
            HasCurrent = hasCurrent;
        }

        public int MaxAmount { get; }
        public bool HasCurrent { get; }
    }
}
