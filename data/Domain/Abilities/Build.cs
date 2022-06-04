﻿using System.Collections.Generic;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Abilities
{
    public class Build : Ability
    {
        public Build(
            AbilityName name,
            string displayName, 
            string description,
            int distance,
            IList<EntityName> selection,
            bool casterConsumesAction = false,
            bool canHelp = false,
            float? helpEfficiency = null) : base(name, $"{nameof(Ability)}.{nameof(Build)}", TurnPhase.Planning, new List<Research>(), true, displayName, description)
        {
            Distance = distance;
            Selection = selection;
            CasterConsumesAction = casterConsumesAction;
            CanHelp = canHelp;
            HelpEfficiency = helpEfficiency ?? 1;
        }

        public int Distance { get; }
        public IList<EntityName> Selection { get; }
        public bool CasterConsumesAction { get; }
        public bool CanHelp { get; }
        
        /// <summary>
        /// Factor of production each additional helper provides compared to the previous one.
        /// E.g. with 0.5 efficiency first builder provides 1 production, second 0.5, third 0.25, etc. 
        /// </summary>
        public float HelpEfficiency { get; }
    }
}
