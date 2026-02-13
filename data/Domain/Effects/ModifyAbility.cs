using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
{
    public class ModifyAbility : Effect
    {
        public ModifyAbility(
            EffectId id,
            AbilityId abilityToModify,
            AbilityId modifiedAbility,
            IList<Validator>? validators = null) 
            : base(
                id, 
                Location.Inherited, 
                validators ?? new List<Validator>())
        {
            AbilityToModify = abilityToModify;
            ModifiedAbility = modifiedAbility;
        }

        public AbilityId AbilityToModify { get; }
        public AbilityId ModifiedAbility { get; }
    }
}
