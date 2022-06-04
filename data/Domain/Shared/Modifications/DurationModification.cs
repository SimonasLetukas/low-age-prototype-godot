using low_age_data.Domain.Behaviours;

namespace low_age_data.Domain.Shared.Modifications
{
    public class DurationModification : Modification
    {
        public DurationModification(
            Change change, 
            float amount,
            BehaviourName behaviourToModify) : base($"{nameof(Modification)}.{nameof(DurationModification)}", change, amount)
        {
            BehaviourToModify = behaviourToModify;
        }
        
        public BehaviourName BehaviourToModify { get; }
    }
}