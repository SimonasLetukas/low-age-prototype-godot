﻿using System.Collections.Generic;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Abilities
{
    /// <summary>
    /// Used for opening a selection of <see cref="ResearchId"/> to be researched by paying the <see cref="Payment"/>.
    /// </summary>
    public class Research : Ability
    {
        public Research(
            AbilityId id,
            string displayName,
            string description,
            string sprite,
            IList<Selection<ResearchId>> selectionOfResearchToBeUnlocked,
            IList<ResearchId>? researchNeeded = null)
            : base(
                id,
                $"{nameof(Ability)}.{nameof(Research)}",
                TurnPhase.Planning,
                researchNeeded ?? new List<ResearchId>(),
                true,
                displayName,
                description,
                sprite)
        {
            SelectionOfResearchToBeUnlocked = selectionOfResearchToBeUnlocked;
        }

        public IList<Selection<ResearchId>> SelectionOfResearchToBeUnlocked { get; }
    }
}