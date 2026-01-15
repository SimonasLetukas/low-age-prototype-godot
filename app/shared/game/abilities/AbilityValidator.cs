using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;

public sealed class AbilityValidator
{
    public interface IAbilityValidatorItem
    {
        ValidationResult Validate();
    }

    public static AbilityValidator With(IEnumerable<IAbilityValidatorItem> validators) => new(validators);

    private readonly IEnumerable<IAbilityValidatorItem> _validators;

    private AbilityValidator(IEnumerable<IAbilityValidatorItem> validators)
    {
        _validators = validators;
    }

    public ValidationResult Validate()
    {
        foreach (var validator in _validators)
        {
            var result = validator.Validate();
            if (result.IsValid is false)
                return result;
        }
        
        return ValidationResult.Valid;
    }

    public sealed class BuildableEntityCanBePlaced : IAbilityValidatorItem
    {
        public required EntityNode? Entity { get; init; }
        public required bool AlreadyPlaced { get; init; }

        public ValidationResult Validate()
        {
            if (Entity is null)
                return ValidationResult.Invalid("There is nothing to build!");
            
            if (AlreadyPlaced)
                return ValidationResult.Valid;

            return Entity.CanBePlaced
                ? ValidationResult.Valid
                : ValidationResult.Invalid("Cannot be placed here!");
        }
    }

    public sealed class CorrectTurnPhase : IAbilityValidatorItem
    {
        public required TurnPhase CurrentTurnPhase { get; init; }
        public required TurnPhase RequiredTurnPhase { get; init; }

        public ValidationResult Validate() => CurrentTurnPhase.Equals(RequiredTurnPhase)
            ? ValidationResult.Valid 
            : ValidationResult.Invalid("Ability cannot be activated in the current turn phase.");
    }

    public sealed class ActorHasEnoughAction : IAbilityValidatorItem
    {
        public required ActorNode Actor { get; init; }
        public required bool ActionNeeded { get; init; }
        
        public ValidationResult Validate()
        {
            if (ActionNeeded is false)
                return ValidationResult.Valid;
            
            return Actor.ActionEconomy.CanUseAbilityAction 
                ? ValidationResult.Valid 
                : ValidationResult.Invalid("Ability action is not available.");
        }
    }

    public sealed class TargetWithinArea : IAbilityValidatorItem
    {
        public required IEnumerable<Vector2Int> AvailablePositions { get; init; }
        public required IEnumerable<Vector2Int> TargetPositions { get; init; }
        
        public ValidationResult Validate() => AvailablePositions.Any(availablePosition => 
            TargetPositions.Any(targetPosition => targetPosition.Equals(availablePosition))) 
            ? ValidationResult.Valid 
            : ValidationResult.Invalid("Target must be within the allowed range.");
    }
    
    public sealed class HelpApplicableAndAllowed : IAbilityValidatorItem
    {
        public required EntityNode EntityToBuild { get; init; }
        public required Guid HelpingAbilityInstanceId { get; init; }
        public required bool HelpingAllowed { get; init; }

        public ValidationResult Validate()
        {
            var helpers = EntityToBuild.CreationProgress.Helpers.Keys;
            
            if (helpers.IsEmpty())
                return ValidationResult.Valid; // We are starting, not helping
            
            if (helpers.Any(a => a.InstanceId.Equals(HelpingAbilityInstanceId)))
                return ValidationResult.Invalid("Already working on this.");
            
            if (HelpingAllowed is false)
                return ValidationResult.Invalid("Helping to work on this is not allowed.");
            
            return ValidationResult.Valid;
        }
    }
}