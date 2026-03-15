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
    private IList<ITargetable> _targetSource = [];

    private ValidationHandler(IList<Validator> validators)
    {
        _validators = validators;
    }
    
    public ValidationHandler With(Player playerSource)
    {
        _playerSource = playerSource;
        return this;
    }

    public ValidationHandler With(IEnumerable<ITargetable> targetSource)
    {
        _targetSource = targetSource.ToList();
        return this;
    }

    public ValidationResult Handle()
    {
        foreach (var validator in _validators) // ALL
        {
            var result = Handle(validator);
            if (result.IsValid is false)
                return result;
        }
        
        return ValidationResult.Valid;
    }

    private ValidationResult Handle(Validator validator)
    {
        ValidationResult result = default;
        foreach (var condition in validator.Conditions) // ANY
        {
            result = Handle(condition);
            if (result.IsValid)
                return result;
        }
        
        return result;
    }

    private ValidationResult Handle(Condition condition)
    {
        return condition switch
        {
            MaskCondition maskCondition => ValidationResult.Valid, // TODO
            ResourceCondition resourceCondition => Handle(resourceCondition),
            TileCondition tileCondition => Handle(tileCondition),
            _ => Handle(condition.ConditionFlag)
        };
    }

    private ValidationResult Handle(ResourceCondition resourceCondition)
    {
        Console.WriteLine($"Resource validation triggered");
        
        if (_playerSource is null)
            return ValidationResult.Invalid("Resource condition failed: player not found.");

        var counter = GlobalRegistry.Instance.GetCurrentPlayerStockpile(_playerSource)
            .FirstOrDefault(r => r.Resource.Equals(resourceCondition.ConditionedResource))?.Amount;

        if (counter is null)
            return ValidationResult.Invalid("Resource condition failed: required resource was not found.");
        
        Console.WriteLine($"Found {resourceCondition.ConditionedResource} resources: {counter}. Required: " +
                          $"{resourceCondition.AmountOfResourcesRequired}. Exists: " +
                          $"{counter >= resourceCondition.AmountOfResourcesRequired}. DoesNotExist: " +
                          $"{counter < resourceCondition.AmountOfResourcesRequired}");
        
        if (resourceCondition.ConditionFlag.Equals(ConditionFlag.Exists))
            return counter >= resourceCondition.AmountOfResourcesRequired 
                ? ValidationResult.Valid 
                : ValidationResult.Invalid("Not enough required resources found.");

        if (resourceCondition.ConditionFlag.Equals(ConditionFlag.DoesNotExist))
            return counter < resourceCondition.AmountOfResourcesRequired 
                ? ValidationResult.Valid 
                : ValidationResult.Invalid("Found too many forbidden resources.");

        return ValidationResult.Invalid("Resource condition failed.");
    }

    private ValidationResult Handle(TileCondition tileCondition)
    {
        var counter = _targetSource.Count(target => target is Tiles.TileInstance tile
                                                    && tile.Blueprint.Equals(tileCondition.ConditionedTile));
        
        if (tileCondition.ConditionFlag.Equals(ConditionFlag.Exists))
            return counter >= tileCondition.AmountOfTilesRequired 
                ? ValidationResult.Valid 
                : ValidationResult.Invalid("Not enough required tiles found.");

        if (tileCondition.ConditionFlag.Equals(ConditionFlag.DoesNotExist))
            return counter < tileCondition.AmountOfTilesRequired 
                ? ValidationResult.Valid 
                : ValidationResult.Invalid("Found too many forbidden tiles.");

        return ValidationResult.Invalid("Tile condition failed.");
    }

    private ValidationResult Handle(ConditionFlag conditionFlag)
    {
        return conditionFlag switch
        {
            _ when _targetSource.Any() is false => ValidationResult.Invalid("No valid targets."),
            
            _ when conditionFlag.Equals(ConditionFlag.TargetDoesNotHaveFullHealth) 
                => _targetSource.All(TargetDoesNotHaveFullHealth) 
                    ? ValidationResult.Valid 
                    : ValidationResult.Invalid("Target has full health."),
            
            _ when conditionFlag.Equals(ConditionFlag.TargetIsLowGround) 
                => _targetSource.All(TargetIsLowGround) 
                    ? ValidationResult.Valid 
                    : ValidationResult.Invalid("Target was not low ground."),
            
            _ when conditionFlag.Equals(ConditionFlag.TargetIsUnoccupied) 
                => _targetSource.All(TargetIsUnoccupied) 
                    ? ValidationResult.Valid 
                    : ValidationResult.Invalid("Target was not unoccupied."),
            
            _ => ValidationResult.Invalid("Unknown condition flag.")
        };
    }
    
    private static bool TargetDoesNotHaveFullHealth(ITargetable target) => target switch
    {
        Tiles.TileInstance => false,
        ActorNode actor => (int?)actor.Health?.CurrentAmount < actor.Health?.MaxAmount,
        _ => false
    };

    private static bool TargetIsLowGround(ITargetable target) => target switch
    {
        Tiles.TileInstance tile => tile.Point.IsHighGround is false,
        UnitNode unit => unit.IsOnHighGround is false,
        EntityNode => true,
        _ => false
    };

    private static bool TargetIsUnoccupied(ITargetable target) => target switch
    {
        Tiles.TileInstance tile => tile.IsOccupied() is false,
        EntityNode => true,
        _ => false
    };
}