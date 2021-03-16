namespace low_age_data.Domain.Entities
{
    public abstract class Entity
    {
        protected Entity(EntityName name, string displayName, string description)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }

        public EntityName Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
    }
}
