using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities.Actors;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
{
    public class Teleport : Effect
    {
        public Teleport(
            EffectId id,
            BehaviourId? waitBefore = null, 
            IList<Validator>? validators = null) 
            : base(
                id, 
                Location.Point, 
                validators ?? new List<Validator>())
        {
            WaitBefore = waitBefore;
        }

        /// <summary>
        /// Puts <see cref="Wait"/> behaviour on the teleporting <see cref="Actor"/> and executes teleport afterwards.
        /// Place to which teleport will happen is considered occupied while actor is waiting.
        /// </summary>
        public BehaviourId? WaitBefore { get; }
    }
}
