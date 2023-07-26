using Newtonsoft.Json;

namespace low_age_data.Domain.Shared.Modifications
{
    public abstract class Modification
    {
        protected Modification(string type, Change change, float amount)
        {
            Type = type;
            Change = change;
            Amount = amount;
        }

        [JsonProperty(Order = -2)]
        public string Type { get; }
        public Change Change { get; }
        public float Amount { get; }
    }
}