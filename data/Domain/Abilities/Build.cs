using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Shape;

namespace low_age_data.Domain.Abilities
{
    public class Build : Ability
    {
        /// <summary>
        /// Used for placing and working on a <see cref="Buildable"/> <see cref="Entity"/> on top of the game map. 
        /// </summary>
        public Build(
            AbilityName name,
            string displayName,
            string description,
            Shape placementArea,
            IList<Selection<EntityName>> selection,
            bool casterConsumesAction = false,
            bool canHelp = false,
            float? helpEfficiency = null)
            : base(
                name,
                $"{nameof(Ability)}.{nameof(Build)}",
                TurnPhase.Planning,
                new List<ResearchName>(),
                true,
                displayName,
                description)
        {
            PlacementArea = placementArea;
            Selection = selection;
            CasterConsumesAction = casterConsumesAction;
            CanHelp = canHelp;
            HelpEfficiency = helpEfficiency ?? 1;
        }

        public Shape PlacementArea { get; }
        public IList<Selection<EntityName>> Selection { get; }
        public bool CasterConsumesAction { get; }
        public bool CanHelp { get; }

        /// <summary>
        /// Factor of production each additional helper provides compared to the previous one.
        /// E.g. with 0.5 efficiency first builder provides 1 production, second 0.5, third 0.25, etc. 
        /// </summary>
        public float HelpEfficiency { get; }
    }
}