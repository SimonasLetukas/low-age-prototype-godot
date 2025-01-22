using LowAgeData.Domain.Behaviours;

namespace LowAgeData.Domain.Common.Modifications
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