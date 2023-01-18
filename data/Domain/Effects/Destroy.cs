using low_age_data.Domain.Logic;
using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities;
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
        
        /// /// <summary>
        /// If true, no <see cref="Behaviour"/> of the destroyed <see cref="Entity"/> can trigger their on-death
        /// functionalities (e.g. <see cref="Buff"/> would not execute <see cref="Buff.FinalEffects"/>). False by
        /// default.
        /// </summary>
        public bool BlocksBehaviours { get; }
    }
}
