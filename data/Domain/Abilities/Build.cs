using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Shape;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;

namespace low_age_data.Domain.Abilities
{
    /// <summary>
    /// Used for placing and working on a <see cref="Buildable"/> <see cref="Entity"/> on top of the game map. 
    /// </summary>
    public class Build : Ability
    {
        public Build(
            AbilityId id,
            string displayName,
            string description,
            string sprite,
            IShape placementArea,
            bool useWalkableTilesAsPlacementArea,
            IList<Selection<EntityId>> selection,
            bool casterConsumesAction = false,
            bool canHelp = false,
            float? helpEfficiency = null)
            : base(
                id,
                TurnPhase.Planning,
                new List<ResearchId>(),
                true,
                displayName,
                description,
                sprite)
        {
            PlacementArea = placementArea;
            UseWalkableTilesAsPlacementArea = useWalkableTilesAsPlacementArea;
            Selection = selection;
            CasterConsumesAction = casterConsumesAction;
            CanHelp = canHelp;
            HelpEfficiency = helpEfficiency ?? 1;
        }

        public IShape PlacementArea { get; }
        
        /// <summary>
        /// If true, <see cref="PlacementArea"/> value is ignored, instead <see cref="Structure.WalkableAreas"/> is
        /// used as a placement area.
        /// </summary>
        public bool UseWalkableTilesAsPlacementArea { get; }
        
        public IList<Selection<EntityId>> Selection { get; }
        
        /// <summary>
        /// If true, to continue paying for the <see cref="Buildable"/> entity, the caster of this <see cref="Build"/>
        /// <see cref="Ability"/> has to spend its action, effectively meaning that only one <see cref="Entity"/> can
        /// be worked on at the same time. 
        ///
        /// False by default.
        /// </summary>
        public bool CasterConsumesAction { get; }
        
        /// <summary>
        /// If true, multiple <see cref="Entity"/>ies can cast their <see cref="Build"/> on the same placed
        /// <see cref="Entity"/> (if they have that <see cref="Entity"/> as part of their <see cref="Selection"/>),
        /// providing additional amount of production according to <see cref="HelpEfficiency"/>.
        ///
        /// False by default.
        /// </summary>
        public bool CanHelp { get; }

        /// <summary>
        /// If <see cref="CanHelp"/> is true, this describes the factor of production each additional helper provides
        /// compared to the previous one.
        /// 
        /// E.g. with 0.5 efficiency first builder provides 100% production, second 50%, third 25%, etc. 
        /// </summary>
        public float HelpEfficiency { get; }
    }
}