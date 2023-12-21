using low_age_data.Domain.Behaviours;

namespace low_age_data.Domain.Common.Modifications
{
    public class DurationModification : Modification
    {
        public DurationModification(
            Change change, 
            float amount,
            BehaviourId behaviourToModify) : base(change, amount)
        {
            BehaviourToModify = behaviourToModify;
        }
        
        public BehaviourId BehaviourToModify { get; }
    }
}