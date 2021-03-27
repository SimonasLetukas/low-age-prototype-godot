namespace low_age_data.Domain.Shared.Modifications
{
    public class ResourceModification : Modification
    {
        public ResourceModification(
            Change change, 
            float amount,
            Resources resource) : base($"{nameof(Modification)}.{nameof(ResourceModification)}", change, amount)
        {
            Resource = resource;
        }

        public Resources Resource { get; }
    }
}
