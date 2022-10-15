using System.Collections.Generic;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Resources;

namespace low_age_data.Domain.Factions
{
    public class Faction
    {
        public Faction(
            FactionName name, 
            string displayName, 
            string description,
            IList<ResourceName> availableResources,
            IList<EntityName> startingEntities)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
            AvailableResources = availableResources;
            StartingEntities = startingEntities;
        }

        public FactionName Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
        
        /// <summary>
        /// List of <see cref="Resource"/>s to be kept track of for this <see cref="Faction"/>.
        /// </summary>
        public IList<ResourceName> AvailableResources { get; }
        
        /// <summary>
        /// List of <see cref="Entity"/> to be spawned on game start, in sequential order (could be relevant if
        /// <see cref="Unit"/> has to be spawned on top of a <see cref="Structure"/>). The starting
        /// <see cref="Entity"/> is placed on the starting map location, using the <see cref="Structure.CenterPoint"/>
        /// property, if available, otherwise keeping top-left discrete center position. The starting
        /// <see cref="Entity"/> is also rotated to face as many enemies as possible. 
        /// </summary>
        public IList<EntityName> StartingEntities { get; }
    }
}