using System.Collections.Generic;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Collections
{
    public static class Effects
    {
        public static List<Effect> Get()
        {
            return new List<Effect>
            {
                new ModifyPlayer(
                    EffectName.Leader.AllForOnePlayerLoses,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    },
                    new List<Flag>
                    {
                        Flag.Effect.ModifyPlayer.GameLost
                    })
            };
        }
    }
}
