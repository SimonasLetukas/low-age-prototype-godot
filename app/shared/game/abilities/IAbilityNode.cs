using System;
using System.Collections.Generic;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Researches;

public interface IAbilityNode
{
    Guid InstanceId { get; set; }
    AbilityId Id { get; }
    string DisplayName { get; }
    string Description { get; }
    string? SpriteLocation { get; }
    TurnPhase TurnPhase { get; }
    IList<ResearchId> ResearchNeeded { get; }
    ActorNode OwnerActor { get; }
    EndsAtNode RemainingCooldown { get; } 
    bool HasButton { get; }
    bool Disabled { get; }

    void SetDisabled(bool disabled);
    ValidationResult Activate(IAbilityActivationRequest request);
    void OnExecutionRequested(IAbilityFocus focus);
}