namespace low_age_data.Domain.Shared.Modifications
{
    public class SizeModification : Modification
    {
        public SizeModification(
            Change change, 
            float amount,
            bool keepCentered = true) : base($"{nameof(Modification)}.{nameof(SizeModification)}", change, amount)
        {
            KeepCentered = keepCentered;
        }

        public bool KeepCentered { get; }
    }
}
