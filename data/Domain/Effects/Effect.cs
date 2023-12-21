using System.Collections.Generic;
using low_age_data.Domain.Logic;
using Newtonsoft.Json;

namespace low_age_data.Domain.Effects
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
