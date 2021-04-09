using System.Collections.Generic;
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
            IList<StructureName> selection,
            bool casterConsumesAction = false,
            bool canHelp = false) : base(name, $"{nameof(Ability)}.{nameof(Build)}", TurnPhase.Planning, new List<Research>(), true, displayName, description)
        {
            Distance = distance;
            Selection = selection;
            CasterConsumesAction = casterConsumesAction;
            CanHelp = canHelp;
        }

        public int Distance { get; }
        public IList<StructureName> Selection { get; }
        public bool CasterConsumesAction { get; }
        public bool CanHelp { get; }
    }
}
