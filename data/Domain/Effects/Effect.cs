using System.Collections.Generic;
using LowAgeData.Domain.Logic;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Effects
{
    public class Effect
    {
        protected Effect(EffectId id, IList<Validator> validators)
        {
            Id = id;
            Validators = validators;
        }

        [JsonProperty(Order = -2)]
        public EffectId Id { get; }
        
        /// <summary>
        /// Must all return true for the effect to be executed
        /// </summary>
        public IList<Validator> Validators { get; }
    }
}
