using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Behaviours.Buffs
{
    public class AttackModification : Modification
    {
        public AttackModification(
            Change change, 
            float amount,
            Attacks attackType,
            AttackAttribute attribute) : base($"{nameof(Modification)}.{nameof(AttackModification)}", change, amount)
        {
            AttackType = attackType;
            Attribute = attribute;
        }

        public Attacks AttackType { get; }
        public AttackAttribute Attribute { get; }
    }
}
