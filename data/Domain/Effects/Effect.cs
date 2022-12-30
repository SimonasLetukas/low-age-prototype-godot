using System.Collections.Generic;
using low_age_data.Domain.Logic;
using Newtonsoft.Json;

namespace low_age_data.Domain.Effects
{
    public class Effect
    {
        protected Effect(EffectName name, string type, IList<Validator> validators)
        {
            Name = name;
            Type = type;
            Validators = validators;
        }

        [JsonProperty(Order = -3)]
        public EffectName Name { get; }
        [JsonProperty(Order = -2)]
        public string Type { get; }
        
        /// <summary>
        /// Must all return true for the effect to be executed
        /// </summary>
        public IList<Validator> Validators { get; }
    }
}
