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
    private Effects? _effectHistorySource;

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

    public ValidationHandler With(Effects effectHistorySource)
    {
        _effectHistorySource = effectHistorySource;
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
        ValidationResult result = ValidationResult.Invalid("Condition failed.");
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
            BehaviourCondition behaviourCondition => Handle(behaviourCondition),
            MaskCondition maskCondition => ValidationResult.Valid, // TODO
            ResourceCondition resourceCondition => Handle(resourceCondition),
            TargetedAbilityCondition targetedAbilityCondition => Handle(targetedAbilityCondition),
            TileCondition tileCondition => Handle(tileCondition),
            _ => Handle(condition.ConditionFlag)
        };
    }

    private ValidationResult Handle(BehaviourCondition behaviourCondition)
    {
        if (_targetSource.Count == 0)
            return ValidationResult.Invalid("Behaviour condition failed: no found targets.");

        var behaviours = _targetSource
            .Where(target => target is EntityNode)
            .Cast<EntityNode>()
            .SelectMany(e => e.Behaviours.GetAll())
            .ToHashSet();
        var conditionedBehaviourId = behaviourCondition.ConditionedBehaviour;
        var counter = behaviours.Count(b => b.BlueprintId.Equals(conditionedBehaviourId));
        var data = Data.Instance.Blueprint.Behaviours.First(b => b.Id.Equals(conditionedBehaviourId));
        
        if (behaviourCondition.ConditionFlag.Equals(ConditionFlag.Exists))
            return counter > 0
                ? ValidationResult.Valid 
                : ValidationResult.Invalid($"Could not find {data.DisplayName} which must exist.");

        if (behaviourCondition.ConditionFlag.Equals(ConditionFlag.DoesNotExist))
            return counter <= 0
                ? ValidationResult.Valid 
                : ValidationResult.Invalid($"Found {data.DisplayName} which cannot exist.");
        
        return ValidationResult.Invalid("Behaviour condition failed.");
    }

    private ValidationResult Handle(ResourceCondition resourceCondition)
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(ValidationHandler), $"{nameof(Handle)}.{nameof(ResourceCondition)}", 
                string.Empty);
        
        if (_playerSource is null)
            return ValidationResult.Invalid("Resource condition failed: player not found.");

        var counter = GlobalRegistry.Instance.GetCurrentPlayerStockpile(_playerSource)
            .FirstOrDefault(r => r.Resource.Equals(resourceCondition.ConditionedResource))?.Amount;

        if (counter is null)
            return ValidationResult.Invalid("Resource condition failed: required resource was not found.");
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ValidationHandler), $"{nameof(Handle)}.{nameof(ResourceCondition)}", 
                $"Found {resourceCondition.ConditionedResource} resources: {counter}. Required: " +
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

    private ValidationResult Handle(TargetedAbilityCondition targetedAbilityCondition)
    {
        if (_targetSource.Count == 0)
            return ValidationResult.Invalid("Targeted ability condition failed: no found targets.");

        var abilities = _targetSource
            .SelectMany(e => e.TargetedBy)
            .ToHashSet();
        var targetedByAbilityId = targetedAbilityCondition.TargetedBy;
        var counter = abilities.Count(b => b.Id.Equals(targetedByAbilityId));
        var data = Data.Instance.Blueprint.Abilities.First(b => b.Id.Equals(targetedByAbilityId));
        
        if (targetedAbilityCondition.ConditionFlag.Equals(ConditionFlag.Exists))
            return counter > 0
                ? ValidationResult.Valid 
                : ValidationResult.Invalid($"Could not find {data.DisplayName} ability which must exist.");

        if (targetedAbilityCondition.ConditionFlag.Equals(ConditionFlag.DoesNotExist))
            return counter <= 0
                ? ValidationResult.Valid 
                : ValidationResult.Invalid($"Found {data.DisplayName} ability which cannot exist.");
        
        return ValidationResult.Invalid("Targeted ability condition failed.");
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
            
            _ when conditionFlag.Equals(ConditionFlag.TargetIsCompleted)
                => _targetSource.All(TargetIsCompleted)
                    ? ValidationResult.Valid 
                    : ValidationResult.Invalid("Target is not yet completed."),
            
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
    
    private static bool TargetIsCompleted(ITargetable target) => target switch
    {
        EntityNode entity => entity.IsCompleted(),
        _ => false
    };
    
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