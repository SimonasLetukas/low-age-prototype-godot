using System.Collections.Generic;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
{
    public class Reload : Effect
    {
        public Reload(
            EffectId id, 
            BehaviourId ammunitionToTarget,
            Location? location = null,
            IList<Validator>? validators = null) : base(id, validators ?? new List<Validator>())
        {
            AmmunitionToTarget = ammunitionToTarget;
            Location = location ?? Location.Inherited;
        }
        
        public BehaviourId AmmunitionToTarget { get; }
        public Location Location { get; }
    }
}