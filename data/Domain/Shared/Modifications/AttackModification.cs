using System.Collections.Generic;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Shared.Modifications
{
    public class AttackModification : Modification
    {
        public AttackModification(
            Change change, 
            float amount,
            Attacks attackType,
            AttackAttribute attribute,
            IList<Flag>? modificationFlags = null) : base($"{nameof(Modification)}.{nameof(AttackModification)}", change, amount)
        {
            AttackType = attackType;
            Attribute = attribute;
            ModificationFlags = modificationFlags ?? new List<Flag>();
        }
        
        public AttackModification(
            Attacks attackType,
            IList<Flag> modificationFlags) : base($"{nameof(Modification)}.{nameof(AttackModification)}", Change.AddCurrent, 0)
        {
            AttackType = attackType;
            Attribute = AttackAttribute.MaxAmount;
            ModificationFlags = modificationFlags;
        }

        public Attacks AttackType { get; }
        public AttackAttribute Attribute { get; }
        public IList<Flag> ModificationFlags { get; }
    }
}
