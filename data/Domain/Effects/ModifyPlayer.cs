using System.Collections.Generic;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Effects
{
    public class ModifyPlayer : Effect
    {
        public ModifyPlayer(
            EffectName name,
            IList<Flag> playerFilterFlags,
            IList<Flag> modifyFlags) : base(name, $"{nameof(Effect)}.{nameof(ModifyPlayer)}")
        {
            PlayerFilterFlags = playerFilterFlags;
            ModifyFlags = modifyFlags;
        }

        public IList<Flag> PlayerFilterFlags { get; }
        public IList<Flag> ModifyFlags { get; }
    }
}
