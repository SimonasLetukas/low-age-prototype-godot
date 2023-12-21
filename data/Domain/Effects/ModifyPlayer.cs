using System.Collections.Generic;
using low_age_data.Domain.Common.Filters;
using low_age_data.Domain.Common.Flags;
using low_age_data.Domain.Common.Modifications;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class ModifyPlayer : Effect
    {
        public ModifyPlayer(
            EffectId id,
            IList<IFilterItem> playerFilters,
            IList<ModifyPlayerFlag>? modifyFlags = null,
            IList<ResourceModification>? resourceModifications = null,
            IList<Validator>? validators = null) : base(id, validators ?? new List<Validator>())
        {
            PlayerFilters = playerFilters;
            ModifyFlags = modifyFlags ?? new List<ModifyPlayerFlag>();
            ResourceModifications = resourceModifications ?? new List<ResourceModification>();
        }

        public IList<IFilterItem> PlayerFilters { get; }
        public IList<ModifyPlayerFlag> ModifyFlags { get; }
        public IList<ResourceModification> ResourceModifications { get; }
    }
}
