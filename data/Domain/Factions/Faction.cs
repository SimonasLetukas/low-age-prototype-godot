using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeData.Domain.Resources;

namespace LowAgeData.Domain.Factions
{
    public class Faction
    {
        public Faction(
            FactionId id, 
            string displayName, 
            string description,
            IList<ResourceId> availableResources,
            IList<EntityId> startingEntities,
            IList<Payment>? bonusStartingResources = null)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            AvailableResources = availableResources;
            StartingEntities = startingEntities;
            BonusStartingResources = bonusStartingResources ?? new List<Payment>();
        }

        public FactionId Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        
        /// <summary>
        /// List of <see cref="Resource"/>s to be kept track of for this <see cref="Faction"/>.
        /// </summary>
        public IList<ResourceId> AvailableResources { get; }
        
        /// <summary>
        /// List of <see cref="Entity"/> to be spawned on game start, in sequential order (could be relevant if
        /// <see cref="Unit"/> has to be spawned on top of a <see cref="Structure"/>). The starting
        /// <see cref="Entity"/> is placed on the starting map location, using the <see cref="Structure.CenterPoint"/>
        /// property, if available, otherwise keeping top-left discrete center position. The starting
        /// <see cref="Entity"/> is also rotated to face as many enemies as possible. 
        /// </summary>
        public IList<EntityId> StartingEntities { get; }
        
        /// <summary>
        /// <see cref="Resource"/> added on game start to the default starting <see cref="Resource"/>s.
        /// Limits are ignored.
        /// </summary>
        public IList<Payment> BonusStartingResources { get; }
    }
}