using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Common.Modifications;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
{
    public class ModifyPlayer : Effect
    {
        public ModifyPlayer(
            EffectId id,
            Location target,
            IList<ModifyPlayerFlag>? modifyFlags = null,
            IList<ResourceModification>? resourceModifications = null,
            IList<Validator>? validators = null) 
            : base(
                id,
                target, 
                validators ?? new List<Validator>())
        {
            ModifyFlags = modifyFlags ?? new List<ModifyPlayerFlag>();
            ResourceModifications = resourceModifications ?? new List<ResourceModification>();
        }

        public IList<ModifyPlayerFlag> ModifyFlags { get; }
        public IList<ResourceModification> ResourceModifications { get; }
    }
}
