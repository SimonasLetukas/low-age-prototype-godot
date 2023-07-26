namespace low_age_data.Domain.Shared.Modifications
{
    public class StatModification : Modification
    {
        public StatModification(
            Change change, 
            float amount,
            StatType statType) : base($"{nameof(Modification)}.{nameof(StatModification)}", change, amount)
        {
            StatType = statType;
        }

        public StatType StatType { get; }
    }
}
