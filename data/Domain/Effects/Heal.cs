using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects;

public class Heal : Effect
{
    public Heal(
        EffectId id,
        StatType statType,
        Amount amount,
        Location? target = null,
        IList<IFilterItem>? filters = null,
        IList<Validator>? validators = null) 
        : base(
            id, 
            target ?? Location.Inherited, 
            validators ?? new List<Validator>())
    {
        StatType = statType;
        Amount = amount;
        Filters = filters ?? new List<IFilterItem>();
    }
    
    public StatType StatType { get; }
    public Amount Amount { get; }
    public IList<IFilterItem> Filters { get; }
}