namespace low_age_data.Domain.Shared.Modifications
{
    public class StatCopyModification : Modification
    {
        public StatCopyModification(
            Change change,
            Location copyFrom,
            float additionalAmount,
            StatType statType) : base($"{nameof(Modification)}.{nameof(StatModification)}", change, additionalAmount)
        {
            CopyFrom = copyFrom;
            StatType = statType;
        }

        public Location CopyFrom { get; }
        public StatType StatType { get; }
    }
}
