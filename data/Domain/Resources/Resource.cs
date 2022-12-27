using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities.Actors;
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
            bool hasBank,
            bool? attachesToNewActors = null,
            ResourceName? storedAs = null)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
            HasLimit = hasLimit;
            IsConsumable = isConsumable;
            HasBank = hasBank;
            AttachesToNewActors = attachesToNewActors ?? false;
            StoredAs = storedAs ?? name;
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
        
        /// <summary>
        /// If true, spending this resource as part of <see cref="Cost"/> during <see cref="Selection"/> retains the
        /// value spent for as long as the <see cref="Actor"/> that was spawned is not destroyed (technically, each
        /// such <see cref="Actor"/> gets a negative <see cref="Income"/> for as long as it's not destroyed, so it
        /// mostly makes sense to use this property together with <see cref="HasBank"/> set to false, unless funky
        /// behaviour is required). False by default.
        /// </summary>
        public bool AttachesToNewActors { get; }
        
        /// <summary>
        /// Specify the <see cref="Resource"/> used for storing purposes, using and making its max amount shared
        /// between other <see cref="Resource"/>s with the same <see cref="StoredAs"/> value. This can be
        /// done to better control the <see cref="Income"/> of the stored resource and to group certain resources to
        /// be stored together. <see cref="StoredAs"/> is identical to the <see cref="Name"/> by default.
        /// </summary>
        public ResourceName StoredAs { get; }
    }
}