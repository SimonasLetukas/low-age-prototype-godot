namespace low_age_data.Domain.Entities.Actors.Structures
{
    public class StructureName : EntityName
    {
        private StructureName(string value) : base($"structure-{value}")
        {
        }
    }
}
