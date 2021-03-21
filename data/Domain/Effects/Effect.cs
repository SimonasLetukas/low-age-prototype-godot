using System;
using Newtonsoft.Json;

namespace low_age_data.Domain.Effects
{
    public class Effect
    {
        protected Effect(EffectName name, string type)
        {
            Name = name;
            Type = type;
        }

        [JsonProperty(Order = -3)]
        public EffectName Name { get; }
        [JsonProperty(Order = -2)]
        public string Type { get; }
    }
}
