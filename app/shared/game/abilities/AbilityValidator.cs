using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;

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
    
    public sealed class CooldownCompleted : IAbilityValidatorItem
    {
        public required EndsAtNode Cooldown { get; init; }

        public ValidationResult Validate() => Cooldown.HasCompleted()
            ? ValidationResult.Valid 
            : ValidationResult.Invalid($"Ability cannot be activated until cooldown " +
                                       $"completes (remaining: {Cooldown.GetText()}).");
    }
    
    public sealed class CorrectTurnPhase : IAbilityValidatorItem
    {
        public required TurnPhase CurrentTurnPhase { get; init; }
        public required TurnPhase RequiredTurnPhase { get; init; }
        public required bool IsRequeued { get; init; }

        public ValidationResult Validate() => IsRequeued || CurrentTurnPhase.Equals(RequiredTurnPhase)
            ? ValidationResult.Valid 
            : ValidationResult.Invalid("Ability cannot be activated in the current turn phase.");
    }

    public sealed class ActorHasEnoughAction : IAbilityValidatorItem
    {
        public required ActorNode Actor { get; init; }
        public required bool ActionNeeded { get; init; }
        public required bool IsRequeued { get; init; }
        
        public ValidationResult Validate()
        {
            if (IsRequeued)
                return ValidationResult.Valid;
            
            if (ActionNeeded is false)
                return ValidationResult.Valid;
            
            return Actor.ActionEconomy.CanUseAbilityAction 
                ? ValidationResult.Valid 
                : ValidationResult.Invalid("Ability action is not available.");
        }
    }
    
    public sealed class HasEnoughConsumableResources : IAbilityValidatorItem
    {
        public required bool UseConsumableResources { get; init; }
        public required IList<Payment> ConsumableCost { get; init; }
        public required Player Player { get; init; }
        
        public ValidationResult Validate()
        {
            if (UseConsumableResources is false || ConsumableCost.IsEmpty())
                return ValidationResult.Valid;

            var stockpile = GlobalRegistry.Instance.GetCurrentPlayerStockpile(Player);
            var canSubtractResources = GlobalRegistry.Instance.CanSubtractResources(stockpile, ConsumableCost);
            
            return canSubtractResources
                ? ValidationResult.Valid 
                : ValidationResult.Invalid("Not enough resources.");
        }
    }

    public sealed class BuildableEntityCanBePlaced : IAbilityValidatorItem
    {
        public required EntityNode? Entity { get; init; }
        public required bool AlreadyPlaced { get; init; }
        public required IList<Selection<EntityId>> Selection { get; init; } 

        public ValidationResult Validate()
        {
            if (Entity is null)
                return ValidationResult.Invalid("There is nothing to build!");
            
            if (Selection.Any(s => s.Name.Equals(Entity.BlueprintId)) is false)
                return ValidationResult.Invalid("Can only work on something from the available selection.");
            
            if (AlreadyPlaced)
                return ValidationResult.Valid;

            return Entity.CanBePlaced
                ? ValidationResult.Valid
                : ValidationResult.Invalid("Cannot be placed here!");
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
        public required BuildNode Helper { get; init; }
        public required bool HelpingAllowed { get; init; }
        public required IList<Payment> NonConsumableStockpile { get; init; }
        public required bool IsRequeued { get; init; }

        public ValidationResult Validate()
        {
            var creationProgress = EntityToBuild.CreationProgress;
            
            if (creationProgress is null)
                return ValidationResult.Invalid("This entity cannot be built!");
            
            var helpers = creationProgress.Helpers.Keys;
            
            if (helpers.Any(a => a.Equals(Helper)) 
                && IsRequeued is false)
                return ValidationResult.Invalid("Already working on this.");
            
            if (creationProgress.CanAddNewHelper() is false 
                && IsRequeued is false)
                return ValidationResult.Invalid("Maximum amount of workers are already working on this.");
            
            if (helpers.IsEmpty())
                return ValidationResult.Valid; // We are starting, not helping
            
            if (HelpingAllowed is false)
                return ValidationResult.Invalid("Helping to work on this is not allowed.");

            var remainingProductionLength = creationProgress.GetRemainingProductionLength(NonConsumableStockpile, 
                [Helper]);
            var enoughHelp = IsRequeued
                ? helpers.Count > 1 && remainingProductionLength <= 1
                : remainingProductionLength <= 1;
            if (enoughHelp)
                return ValidationResult.Invalid("There is enough help to finish this in 1 turn.");
            
            return ValidationResult.Valid;
        }
    }

    public sealed class EffectsValidatorsPass : IAbilityValidatorItem
    {
        public required IEnumerable<Effects> Effects { get; init; }
        
        public ValidationResult Validate()
        {
            foreach (var effect in Effects)
            {
                var result = effect.ValidateLast();
                if (result.IsValid is false)
                    return result;
            }
            
            return ValidationResult.Valid;
        }
    }
}