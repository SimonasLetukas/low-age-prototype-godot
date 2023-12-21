using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    /// <summary>
    /// To be used when <see cref="ResearchId"/> should be activated separate from <see cref="Research"/>
    /// <see cref="Ability"/> or if some <see cref="ResearchId"/> should be removed.
    /// </summary>
    public class ModifyResearch : Effect
    {
        public ModifyResearch(
            EffectId id,
            IList<ResearchId>? researchToAdd = null,
            IList<ResearchId>? researchToRemove = null,
            IList<Validator>? validators = null)
            : base(
                id, 
                validators ?? new List<Validator>())
        {
            ResearchToAdd = researchToAdd ?? new List<ResearchId>();
            ResearchToRemove = researchToRemove ?? new List<ResearchId>();
        }
        
        public IList<ResearchId> ResearchToAdd { get; }
        public IList<ResearchId> ResearchToRemove { get; }
    }
}