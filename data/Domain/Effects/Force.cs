using System.Collections.Generic;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Effects
{
    /// <summary>
    /// Moves target in a line extrapolated from source location and current location
    /// </summary>
    public class Force : Effect
    {
        public Force(
            EffectName name, 
            Location from,
            int amount,
            IList<EffectName>? onCollisionEffects = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(Force)}", validators ?? new List<Validator>())
        {
            From = from;
            Amount = amount;
            OnCollisionEffects = onCollisionEffects ?? new List<EffectName>();
        }

        public Location From { get; }
        public int Amount { get; }
        public IList<EffectName> OnCollisionEffects { get; } // Executed when force is unable to be completed due to collision
    }
}
