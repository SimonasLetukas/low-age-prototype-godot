using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    public class Wait : Behaviour
    {
        public Wait(
            BehaviourName name,
            string displayName, 
            string description,
            EndsAt endsAt,
            BehaviourName? nextBehaviour = null) : base(name, $"{nameof(Behaviour)}.{nameof(Wait)}", displayName, description)
        {
            EndsAt = endsAt;
            NextBehaviour = nextBehaviour;
        }

        public EndsAt EndsAt { get; }
        public BehaviourName? NextBehaviour { get; }
    }
}
