using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Resources;

namespace LowAgeData.Domain.Logic;

public class ResourceCondition : Condition
{
    public ResourceCondition(
        ConditionFlag conditionFlag,
        ResourceId conditionedResource,
        int? amountOfResourcesRequired = null) : base(conditionFlag)
    {
        ConditionedResource = conditionedResource;
        AmountOfResourcesRequired = amountOfResourcesRequired ?? 1;
    }
        
    /// <summary>
    /// Specify the <see cref="Resource"/> to be targeted by this <see cref="ResourceCondition"/>.
    /// </summary>
    public ResourceId ConditionedResource { get; }
        
    /// <summary>
    /// How many <see cref="Resource"/>s should be found for this condition to return true.
    /// <see cref="ConditionFlag"/> <see cref="ConditionFlag.Exists"/> checks for equal or more (>=) amount of
    /// <see cref="Resource"/>s than <see cref="AmountOfResourcesRequired"/>, while <see cref="ConditionFlag"/>
    /// <see cref="ConditionFlag.DoesNotExist"/> checks for less (&lt;) amount of <see cref="Resource"/>s than
    /// <see cref="AmountOfResourcesRequired"/>.
    /// </summary>
    public int AmountOfResourcesRequired { get; }
}