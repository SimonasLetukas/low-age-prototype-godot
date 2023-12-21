namespace low_age_data.Domain.Common.Modifications
{
    public class StatCopyModification : Modification
    {
        public StatCopyModification(
            Change change,
            Location copyFrom,
            float additionalAmount,
            StatType statType) : base(change, additionalAmount)
        {
            CopyFrom = copyFrom;
            StatType = statType;
        }

        public Location CopyFrom { get; }
        public StatType StatType { get; }
    }
}
