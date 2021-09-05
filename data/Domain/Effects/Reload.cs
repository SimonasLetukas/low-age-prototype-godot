using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Effects
{
    public class Reload : Effect
    {
        public Reload(
            EffectName name, 
            BehaviourName ammunitionToTarget,
            Location? location = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(Reload)}", validators ?? new List<Validator>())
        {
            AmmunitionToTarget = ammunitionToTarget;
            Location = location ?? Location.Inherited;
        }
        
        public BehaviourName AmmunitionToTarget { get; }
        public Location Location { get; }
    }
}