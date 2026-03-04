using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Logic;

public class ValidationHandler
{
    public static ValidationHandler Validate(IList<Validator> validators) => new(validators);
    
    private readonly IList<Validator> _validators;
    private Player? _playerSource;
    private IList<Tiles.TileInstance?> _tileSource = new List<Tiles.TileInstance?>();

    private ValidationHandler(IList<Validator> validators)
    {
        _validators = validators;
    }
    
    public ValidationHandler With(Player playerSource)
    {
        _playerSource = playerSource;
        return this;
    }

    public ValidationHandler With(IList<Tiles.TileInstance?> tileSource)
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
            ResourceCondition resourceCondition => Handle(resourceCondition),
            TileCondition tileCondition => Handle(tileCondition),
            _ => Handle(condition.ConditionFlag)
        };
    }

    private bool Handle(ResourceCondition resourceCondition)
    {
        Console.WriteLine($"Resource validation triggered");
        
        if (_playerSource is null)
            return false;

        var counter = GlobalRegistry.Instance.GetCurrentPlayerStockpile(_playerSource)
            .FirstOrDefault(r => r.Resource.Equals(resourceCondition.ConditionedResource))?.Amount;

        if (counter is null)
            return false;
        
        Console.WriteLine($"Found {resourceCondition.ConditionedResource} resources: {counter}. Required: " +
                          $"{resourceCondition.AmountOfResourcesRequired}. Exists: " +
                          $"{counter >= resourceCondition.AmountOfResourcesRequired}. DoesNotExist: " +
                          $"{counter < resourceCondition.AmountOfResourcesRequired}");
        
        if (resourceCondition.ConditionFlag.Equals(ConditionFlag.Exists))
            return counter >= resourceCondition.AmountOfResourcesRequired;

        if (resourceCondition.ConditionFlag.Equals(ConditionFlag.DoesNotExist))
            return counter < resourceCondition.AmountOfResourcesRequired;

        return false;
    }

    private bool Handle(TileCondition tileCondition)
    {
        var counter = _tileSource.Count(tile => tile is not null 
                                                && tile.Blueprint.Equals(tileCondition.ConditionedTile));
        
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
                t is null || t.Point.IsHighGround is false),
            
            _ when conditionFlag.Equals(ConditionFlag.TargetIsUnoccupied) => _tileSource.All(t => 
                t is null || t.IsOccupied() is false),
            
            _ => false
        };
    }
}