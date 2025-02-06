using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Logic;
using LowAgeCommon.Extensions;

public class ValidationHandler
{
    public static ValidationHandler Validate(IList<Validator> validators) => new ValidationHandler(validators);
    
    private readonly IList<Validator> _validators;
    private IList<Tiles.TileInstance> _tileSource = new List<Tiles.TileInstance>();

    private ValidationHandler(IList<Validator> validators)
    {
        _validators = validators;
    }

    public ValidationHandler With(IList<Tiles.TileInstance> tileSource)
    {
        _tileSource = tileSource;
        return this;
    }

    public bool Handle() => _validators.All(Handle);

    private bool Handle(Validator validator) => validator.Conditions.Any(Handle);

    private bool Handle(Condition condition)
    {
        return condition switch
        {
            MaskCondition maskCondition => true, // TODO
            TileCondition tileCondition => Handle(tileCondition),
            _ => Handle(condition.ConditionFlag)
        };
    }

    private bool Handle(TileCondition tileCondition)
    {
        var counter = _tileSource.Count(tile => tile.Blueprint.Equals(tileCondition.ConditionedTile));
        
        if (tileCondition.ConditionFlag.Equals(ConditionFlag.Exists))
            return counter >= tileCondition.AmountOfTilesRequired;

        if (tileCondition.ConditionFlag.Equals(ConditionFlag.DoesNotExist))
            return counter < tileCondition.AmountOfTilesRequired;

        return false;
    }

    private bool Handle(ConditionFlag conditionFlag)
    {
        return conditionFlag switch
        {
            _ when conditionFlag.Equals(ConditionFlag.TargetIsLowGround) => _tileSource.All(t => 
                t.Point.IsHighGround is false),
            
            _ when conditionFlag.Equals(ConditionFlag.TargetIsUnoccupied) => _tileSource.All(x => 
                x.Occupants.IsEmpty()),
            
            _ => false
        };
    }
}