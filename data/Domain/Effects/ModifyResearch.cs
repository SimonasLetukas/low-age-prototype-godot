using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Effects
{
    /// <summary>
    /// To be used when <see cref="ResearchName"/> should be activated separate from <see cref="Research"/>
    /// <see cref="Ability"/> or if some <see cref="ResearchName"/> should be removed.
    /// </summary>
    public class ModifyResearch : Effect
    {
        public ModifyResearch(
            EffectName name,
            IList<ResearchName>? researchToAdd = null,
            IList<ResearchName>? researchToRemove = null,
            IList<Validator>? validators = null)
            : base(
                name, 
                $"{nameof(Effect)}.{nameof(ModifyResearch)}", 
                validators ?? new List<Validator>())
        {
            ResearchToAdd = researchToAdd ?? new List<ResearchName>();
            ResearchToRemove = researchToRemove ?? new List<ResearchName>();
        }
        
        public IList<ResearchName> ResearchToAdd { get; }
        public IList<ResearchName> ResearchToRemove { get; }
    }
}