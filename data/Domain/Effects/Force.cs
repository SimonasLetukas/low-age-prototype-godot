using System.Collections.Generic;
using low_age_data.Domain.Common;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    /// <summary>
    /// Moves target in a line extrapolated from source location and current location
    /// </summary>
    public class Force : Effect
    {
        public Force(
            EffectId id, 
            Location from,
            int amount,
            IList<EffectId>? onCollisionEffects = null,
            IList<Validator>? validators = null) : base(id, validators ?? new List<Validator>())
        {
            From = from;
            Amount = amount;
            OnCollisionEffects = onCollisionEffects ?? new List<EffectId>();
        }

        public Location From { get; }
        public int Amount { get; }
        public IList<EffectId> OnCollisionEffects { get; } // Executed when force is unable to be completed due to collision
    }
}
