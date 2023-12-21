using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Durations;

namespace low_age_data.Domain.Behaviours
{
    public class Wait : Behaviour
    {
        public Wait(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            EndsAt endsAt,
            BehaviourId? nextBehaviour = null) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                endsAt,
                Alignment.Neutral)
        {
            NextBehaviour = nextBehaviour;
        }

        public BehaviourId? NextBehaviour { get; }
    }
}
