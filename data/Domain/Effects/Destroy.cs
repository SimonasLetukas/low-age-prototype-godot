using LowAgeData.Domain.Logic;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;

namespace LowAgeData.Domain.Effects
{
    public class Destroy : Effect
    {
        public Destroy(
            EffectId id, 
            Location? target = null,
            IList<Validator>? validators = null,
            bool blocksBehaviours = false) 
            : base(
                id, 
                target ?? Location.Self, 
                validators ?? new List<Validator>())
        {
            BlocksBehaviours = blocksBehaviours;
        }
        
        /// /// <summary>
        /// If true, no <see cref="Behaviour"/> of the destroyed <see cref="Entity"/> can trigger their on-death
        /// functionalities (e.g. <see cref="Buff"/> would not execute <see cref="Buff.FinalEffects"/>). False by
        /// default.
        /// </summary>
        public bool BlocksBehaviours { get; }
    }
}
