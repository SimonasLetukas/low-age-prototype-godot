using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Resources
{
    public class Resource
    {
        public Resource(
            ResourceName name, 
            string displayName, 
            string description, 
            bool hasLimit, 
            bool isConsumable, 
            bool hasBank)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
            HasLimit = hasLimit;
            IsConsumable = isConsumable;
            HasBank = hasBank;
        }

        public ResourceName Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
        
        /// <summary>
        /// If true, max and current values are separate and going over the max value normally is not allowed.
        /// </summary>
        public bool HasLimit { get; }
        
        /// <summary>
        /// If true, spending this <see cref="Resource"/> as part of <see cref="Cost"/> deducts the value from the
        /// current amount. Otherwise, the payment is continuous (used for production).
        /// </summary>
        public bool IsConsumable { get; }
        
        /// <summary>
        /// If true, adding current or max amount accumulates into a bank of resources. Otherwise, only the incomes
        /// are added together to always represent the current value. 
        /// </summary>
        public bool HasBank { get; }
    }
}