using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Common.Flags;
using low_age_data.Domain.Logic;

public partial class ValidationHandler
{
    public static ValidationHandler Validate(IList<Validator> validators) => new ValidationHandler(validators);
    
    private IList<Validator> _validators;
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
        switch (condition)
        {
            case MaskCondition maskCondition:
                return true; // TODO
            case TileCondition tileCondition:
                return Handle(tileCondition);
                break;
            default:
                return Handle(condition.ConditionFlag);
                break;
        }
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
        switch (conditionFlag)
        {
            case var _ when conditionFlag.Equals(ConditionFlag.TargetIsLowGround):
                return _tileSource.All(t => t.Occupants.IsEmpty() 
                                            || (t.IsTarget && t.Occupants.All(o => 
                                                o is StructureNode structure 
                                                && structure.WalkablePositions.Contains(t.Position))));
            case var _ when conditionFlag.Equals(ConditionFlag.TargetIsUnoccupied):
                return _tileSource.All(x => x.Occupants.IsEmpty());
            
            default:
                return false;
        }
    }
}