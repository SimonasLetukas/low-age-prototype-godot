namespace low_age_data.Domain.Common.Modifications
{
    public abstract class Modification
    {
        protected Modification(Change change, float amount)
        {
            Change = change;
            Amount = amount;
        }

        public Change Change { get; }
        public float Amount { get; }
    }
}