namespace low_age_data.Domain.Shared.Modifications
{
    public class StatCopyModification : Modification
    {
        public StatCopyModification(
            Change change,
            Location copyFrom,
            float additionalAmount,
            Stats stat) : base($"{nameof(Modification)}.{nameof(StatModification)}", change, additionalAmount)
        {
            CopyFrom = copyFrom;
            Stat = stat;
        }

        public Location CopyFrom { get; }
        public Stats Stat { get; }
    }
}
