using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Researches;

namespace LowAgeData.Domain.Logic;

/// <summary>
/// <see cref="Condition"/> to target a specific <see cref="Abilities.Research"/>ed item.
/// </summary>
public class ResearchCondition : Condition
{
    public ResearchCondition(
        ConditionFlag conditionFlag,
        ResearchId conditionedResearchId,
        Location researchOwner) : base(conditionFlag)
    {
        ConditionedResearchId = conditionedResearchId;
        ResearchOwner = researchOwner;
    }
    
    /// <summary>
    /// Used to check for a specific <see cref="ResearchId"/>.
    /// </summary>
    public ResearchId ConditionedResearchId { get; }
    
    /// <summary>
    /// Controls the condition target: checks the <see cref="Entity"/>'s player's current <see cref="Abilities.Research"/>.
    /// </summary>
    public Location ResearchOwner { get; }
}