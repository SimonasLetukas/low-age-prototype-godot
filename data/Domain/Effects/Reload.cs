using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Effects
{
    public class Reload : Effect
    {
        public Reload(
            EffectId id, 
            BehaviourId ammunitionToTarget,
            Location? location = null,
            IList<Validator>? validators = null) : base(id, $"{nameof(Effect)}.{nameof(Reload)}", validators ?? new List<Validator>())
        {
            AmmunitionToTarget = ammunitionToTarget;
            Location = location ?? Location.Inherited;
        }
        
        public BehaviourId AmmunitionToTarget { get; }
        public Location Location { get; }
    }
}