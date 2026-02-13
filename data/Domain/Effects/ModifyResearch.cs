using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
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
                Location.Inherited, 
                validators ?? new List<Validator>())
        {
            ResearchToAdd = researchToAdd ?? new List<ResearchId>();
            ResearchToRemove = researchToRemove ?? new List<ResearchId>();
        }
        
        public IList<ResearchId> ResearchToAdd { get; }
        public IList<ResearchId> ResearchToRemove { get; }
    }
}