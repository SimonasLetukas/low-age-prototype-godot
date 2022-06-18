using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class Teleport : Effect
    {
        public Teleport(
            EffectName name,
            BehaviourName? waitBefore = null, 
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(Teleport)}", validators ?? new List<Validator>())
        {
            WaitBefore = waitBefore;
        }

        /// <summary>
        /// Puts <see cref="Wait"/> behaviour on the teleporting <see cref="Actor"/> and executes teleport afterwards.
        /// Place to which teleport will happen is considered occupied while actor is waiting.
        /// </summary>
        public BehaviourName? WaitBefore { get; }
    }
}
