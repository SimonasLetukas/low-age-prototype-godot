using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Shape;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors.Structures;

namespace LowAgeData.Domain.Abilities
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
            float? helpEfficiency = null, 
            IList<ResearchId>? researchNeeded = null)
            : base(
                id,
                TurnPhase.Planning,
                researchNeeded ?? new List<ResearchId>(),
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
        /// <para>
        /// If true, to continue paying for the <see cref="Buildable"/> entity, the caster of this <see cref="Build"/>
        /// <see cref="Ability"/> has to spend its action, effectively meaning that only one <see cref="Entity"/> can
        /// be worked on at the same time.
        /// </para>
        /// <para>
        /// False by default.
        /// </para>
        /// </summary>
        public bool CasterConsumesAction { get; }
        
        /// <summary>
        /// <para>
        /// If true, multiple <see cref="Entity"/>ies can cast their <see cref="Build"/> on the same placed
        /// <see cref="Entity"/> (if they have that <see cref="Entity"/> as part of their <see cref="Selection"/>),
        /// providing additional amount of production according to <see cref="HelpEfficiency"/>.
        /// </para>
        /// <para>
        /// False by default.
        /// </para>
        /// </summary>
        public bool CanHelp { get; }

        /// <summary>
        /// <para>
        /// If <see cref="CanHelp"/> is true, this describes the factor of production each additional helper provides
        /// compared to the previous one.
        /// </para>
        /// <para>
        /// E.g. with 0.5 efficiency first builder provides 100% production, second 50%, third 25%, etc.
        /// </para>
        /// </summary>
        public float HelpEfficiency { get; }
    }
}