using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Behaviours.Buffs
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
