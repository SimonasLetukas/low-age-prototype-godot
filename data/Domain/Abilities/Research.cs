using System.Collections.Generic;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Abilities
{
    /// <summary>
    /// Used for opening a selection of <see cref="ResearchName"/> to be researched by paying the <see cref="Cost"/>.
    /// </summary>
    public class Research : Ability
    {
        public Research(
            AbilityName name,
            string displayName,
            string description,
            IList<Selection<ResearchName>> selectionOfResearchToBeUnlocked,
            IList<ResearchName>? researchNeeded = null)
            : base(
                name,
                $"{nameof(Ability)}.{nameof(Research)}",
                TurnPhase.Planning,
                researchNeeded ?? new List<ResearchName>(),
                true,
                displayName,
                description)
        {
            SelectionOfResearchToBeUnlocked = selectionOfResearchToBeUnlocked;
        }

        public IList<Selection<ResearchName>> SelectionOfResearchToBeUnlocked { get; }
    }
}