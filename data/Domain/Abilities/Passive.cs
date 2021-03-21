using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Abilities
{
    public class Passive : Ability
    {
        public Passive(
            AbilityName name, 
            string displayName, 
            string description, 
            BehaviourName behaviour,
            EntityName source,
            EntityName target) : base(name, $"{nameof(Ability)}.{nameof(Passive)}", displayName, description)
        {
            Behaviour = behaviour;
            Source = source;
            Target = target;
        }

        public BehaviourName Behaviour { get; }
        public EntityName Source { get; }
        public EntityName Target { get; }
    }
}
