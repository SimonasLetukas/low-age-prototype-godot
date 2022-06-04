using low_age_data.Domain.Logic;
using System.Collections.Generic;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Effects
{
    public class Destroy : Effect
    {
        public Destroy(
            EffectName name, 
            Location? target = null,
            IList<Validator>? validators = null,
            bool blocksBehaviours = false) : base(name, $"{nameof(Effect)}.{nameof(Destroy)}", validators ?? new List<Validator>())
        {
            Target = target ?? Location.Self;
            BlocksBehaviours = blocksBehaviours;
        }

        public Location Target { get; }
        
        /// <summary>
        /// If true, no behaviours that have "final" effects or modifications are executed on actor.
        /// </summary>
        public bool BlocksBehaviours { get; }
    }
}
