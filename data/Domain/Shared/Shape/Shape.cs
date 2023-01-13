using Newtonsoft.Json;

namespace low_age_data.Domain.Shared.Shape
{
    public abstract class Shape
    {
        protected Shape(string type)
        {
            Type = type;
        }
        
        [JsonProperty(Order = -2)]
        public string Type { get; }
    }
}
