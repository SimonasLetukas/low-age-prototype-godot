using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    public class Wait : Behaviour
    {
        public Wait(
            BehaviourId id,
            string displayName, 
            string description,
            EndsAt endsAt,
            BehaviourId? nextBehaviour = null) 
            : base(
                id, 
                $"{nameof(Behaviour)}.{nameof(Wait)}", 
                displayName, 
                description, 
                endsAt,
                Alignment.Neutral)
        {
            NextBehaviour = nextBehaviour;
        }

        public BehaviourId? NextBehaviour { get; }
    }
}
