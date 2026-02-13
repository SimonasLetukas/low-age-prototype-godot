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
            Location? target = null,
            IList<Validator>? validators = null) 
            : base(
                id, 
                target ?? Location.Inherited,
                validators ?? new List<Validator>())
        {
            AmmunitionToTarget = ammunitionToTarget;
        }
        
        public BehaviourId AmmunitionToTarget { get; }
    }
}