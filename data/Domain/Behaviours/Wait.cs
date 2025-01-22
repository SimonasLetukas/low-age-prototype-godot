using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;

namespace LowAgeData.Domain.Behaviours
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
