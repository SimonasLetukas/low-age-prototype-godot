using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common.Flags;

namespace LowAgeData.Domain.Logic;

/// <summary>
/// Used to validate the abilities that have targeted the affected target.
/// </summary>
public class TargetedAbilityCondition(
    ConditionFlag conditionFlag, 
    AbilityId targetedBy) : Condition(conditionFlag)
{
    /// <summary>
    /// Used to check for a specific <see cref="AbilityId"/> which is targeting the target.
    /// </summary>
    public AbilityId TargetedBy { get; } = targetedBy;
}