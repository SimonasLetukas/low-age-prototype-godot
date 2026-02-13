using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Logic;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Effects
{
    public class Effect
    {
        protected Effect(EffectId id, Location target, IList<Validator> validators)
        {
            Id = id;
            Target = target;
            Validators = validators;
        }

        [JsonProperty(Order = -2)]
        public EffectId Id { get; }
        
        /// <summary>
        /// Specifies what <see cref="Location"/> this <see cref="Effect"/> should target. <see cref="Ability"/>
        /// relies on this, among other things.  
        /// </summary>
        public Location Target { get; }
        
        /// <summary>
        /// Must all return true for the effect to be executed
        /// </summary>
        public IList<Validator> Validators { get; }
    }
}
