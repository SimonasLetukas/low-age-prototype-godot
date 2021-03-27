namespace low_age_data.Domain.Shared.Modifications
{
    public class StatModification : Modification
    {
        public StatModification(
            Change change, 
            float amount,
            Stats stat) : base($"{nameof(Modification)}.{nameof(StatModification)}", change, amount)
        {
            Stat = stat;
        }

        public Stats Stat { get; }
    }
}
