using System;

public interface IActiveAbilityNode : IAbilityNode
{
    event Action<IActiveAbilityNode> ActivationsCancelled;
    event Action<IActiveAbilityFocus> FocusRemoved;
    
    bool IsActivated { get; }
    
    void CancelActivations();
    void CancelActivation(IAbilityActivationRequest request);
}