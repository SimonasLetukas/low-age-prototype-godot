namespace low_age_data.Domain.Behaviours
{
    public class Destroy : Behaviour
    {
        public Destroy(
            BehaviourName name,
            string displayName, 
            string description,
            bool blocksOtherBehaviours = false) : base(name, $"{nameof(Behaviour)}.{nameof(Destroy)}", displayName, description)
        {
            BlocksOtherBehaviours = blocksOtherBehaviours;
        }

        public bool BlocksOtherBehaviours { get; } // If true, no other behaviours are executed on actor
    }
}
