using System.Collections.Generic;
using low_age_data.Domain.Shared.Flags;
using low_age_data.Domain.Shared.Modifications;

namespace low_age_data.Domain.Effects
{
    public class ModifyPlayer : Effect
    {
        public ModifyPlayer(
            EffectName name,
            IList<Flag> playerFilterFlags,
            IList<Flag>? modifyFlags = null,
            IList<ResourceModification>? resourceModifications = null) : base(name, $"{nameof(Effect)}.{nameof(ModifyPlayer)}")
        {
            PlayerFilterFlags = playerFilterFlags;
            ModifyFlags = modifyFlags ?? new List<Flag>();
            ResourceModifications = resourceModifications ?? new List<ResourceModification>();
        }

        public IList<Flag> PlayerFilterFlags { get; }
        public IList<Flag> ModifyFlags { get; }
        public IList<ResourceModification> ResourceModifications { get; }
    }
}
