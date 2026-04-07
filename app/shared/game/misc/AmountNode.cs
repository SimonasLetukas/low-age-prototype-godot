using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Flags;

public class AmountNode
{
    public static AmountNode Build(Amount amount) => new(amount);

    private Amount Amount { get; }
    
    private AmountNode(Amount amount)
    {
        Amount = amount;
    }

    public float GetResolvedAmount(float currentAmount, Effects history, EntityNode? initiatorEntity, 
        EntityNode? targetEntity)
    {
        var flat = Amount.Flat;
        var multiplied = GetMultipliedDamage(currentAmount, history, initiatorEntity, targetEntity);
        return flat + multiplied;
    }
    
    private float GetMultipliedDamage(float currentAmount, Effects history, EntityNode? initiatorEntity, 
        EntityNode? targetEntity)
    {
        if (Amount.Multiplier is null)
            return 0;

        var target = Amount.MultiplyTarget;
        if (target.Equals(Location.Inherited))
            return currentAmount * Amount.Multiplier.Value;

        var entity = target switch
        {
            _ when target.Equals(Location.Self) => initiatorEntity ?? history.Last.InitiatorEntity,
            _ when target.Equals(Location.Entity) => targetEntity,
            _ when target.Equals(Location.Source) => history.SourceEntityOrNull ?? initiatorEntity,
            _ when target.Equals(Location.Origin) => history.OriginEntityOrNull ?? initiatorEntity,
            _ => targetEntity
        };

        var multiplierOf = Amount.MultiplierOf;
        if (entity is not ActorNode actor || multiplierOf is null)
            return 0;

        var baseValue = multiplierOf switch
        {
            _ when multiplierOf.Equals(AmountMultiplyOfFlag.Health) => actor.Health?.CurrentAmount ?? 0,
            
            _ when multiplierOf.Equals(AmountMultiplyOfFlag.Vitals)
                => actor.Health?.CurrentAmount ?? 0 + actor.Shields?.CurrentAmount ?? 0,
            
            _ when multiplierOf.Equals(AmountMultiplyOfFlag.MissingHealth)
                => actor.Health?.MaxAmount ?? 0 - actor.Health?.CurrentAmount ?? 0,
            
            _ when multiplierOf.Equals(AmountMultiplyOfFlag.MissingVitals)
                => (actor.Health?.MaxAmount ?? 0 + actor.Shields?.MaxAmount ?? 0) -
                   (actor.Health?.CurrentAmount ?? 0 + actor.Shields?.CurrentAmount ?? 0),
            
            _ => 0
        };

        return baseValue * Amount.Multiplier.Value;
    }
}