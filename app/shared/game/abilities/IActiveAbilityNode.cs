using System;

public interface IActiveAbilityNode
{
    event Action<IActiveAbilityFocus> Cancelled;
    
    bool IsActivated { get; }
    
    void CancelActivations();
    void CancelActivation(IAbilityActivationRequest request);
}