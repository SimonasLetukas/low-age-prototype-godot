using System;
using System.Collections.Generic;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;

public interface IAbilityNode
{
    Guid InstanceId { get; set; }
    AbilityId Id { get; }
    string DisplayName { get; }
    string Description { get; }
    TurnPhase TurnPhase { get; }
    IList<ResearchId> ResearchNeeded { get; }
    ActorNode OwnerActor { get; }
    EndsAtNode RemainingCooldown { get; } 
    bool HasButton { get; }

    public ValidationResult Activate(IAbilityActivationRequest request);
    void OnExecutionRequested(IAbilityFocus focus);
}