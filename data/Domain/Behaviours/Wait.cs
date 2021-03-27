using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    public class Wait : Behaviour
    {
        public Wait(
            BehaviourName name,
            string displayName, 
            string description,
            EndsAt endsAt) : base(name, $"{nameof(Behaviour)}.{nameof(Wait)}", displayName, description)
        {
            EndsAt = endsAt;
        }

        public EndsAt EndsAt { get; }
    }
}
