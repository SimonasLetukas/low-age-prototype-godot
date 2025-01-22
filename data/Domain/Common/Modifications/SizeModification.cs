namespace LowAgeData.Domain.Common.Modifications
{
    public class SizeModification : Modification
    {
        public SizeModification(
            Change change, 
            float amount,
            bool keepCentered = true) : base(change, amount)
        {
            KeepCentered = keepCentered;
        }

        public bool KeepCentered { get; }
    }
}
