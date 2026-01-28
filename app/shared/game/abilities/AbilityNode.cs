using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;

/// <summary>
/// Note: since visualisation in-game is not needed, this abstract class has no node/scene. 
/// </summary>
public abstract partial class AbilityNode<
    TActivationRequest, 
    TPreProcessingResult, 
    TFocus> 
    : Node2D, INodeFromBlueprint<Ability>, IAbilityNode
    where TActivationRequest : IAbilityActivationRequest 
    where TPreProcessingResult : IAbilityActivationPreProcessingResult 
    where TFocus : IAbilityFocus
{
    public event Action<AbilityNode<TActivationRequest, TPreProcessingResult, TFocus>> Activated = delegate { };
    public event Action<AbilityNode<TActivationRequest, TPreProcessingResult, TFocus>, TFocus> ExecutionRequested = delegate { };
    public event Action<AbilityNode<TActivationRequest, TPreProcessingResult, TFocus>> CooldownEnded = delegate { };
    
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public AbilityId Id { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public TurnPhase TurnPhase { get; private set; } = null!;
    public IList<ResearchId> ResearchNeeded { get; private set; } = [];
    public ActorNode OwnerActor { get; protected set; } = null!;
    public EndsAtNode RemainingCooldown { get; private set; } = null!;
    public bool HasButton { get; private set; }

    protected IList<TFocus> FocusQueue { get; } = new List<TFocus>();
    
    private Ability Blueprint { get; set; } = null!;

    public void SetBlueprint(Ability blueprint)
    {
        Blueprint = blueprint;
        Id = Blueprint.Id;
        DisplayName = Blueprint.DisplayName;
        Description = Blueprint.Description;
        TurnPhase = Blueprint.TurnPhase;
        ResearchNeeded = Blueprint.ResearchNeeded;
        RemainingCooldown = EndsAtNode.InstantiateAsChild(Blueprint.Cooldown, this, OwnerActor);
        RemainingCooldown.Completed += OnCooldownEnded;
        HasButton = Blueprint.HasButton;
    }
    
    public override void _ExitTree()
    {
        RemainingCooldown.Completed -= OnCooldownEnded;
        base._ExitTree();
    }

    public ValidationResult Activate(IAbilityActivationRequest request)
    {
        if (request is not TActivationRequest typedRequest)
        {
            GD.PrintErr($"Invalid {nameof(IAbilityActivationRequest)} type. Expected " +
                        $"'{typeof(TActivationRequest).Name}', but got '{request.GetType().Name}'");
            return ValidationResult.Invalid("Error in ability activation!");
        }

        return Activate(typedRequest);
    }
    
    private ValidationResult Activate(TActivationRequest request)
    {
        var validationResult = ValidateActivation(request);
        if (validationResult.IsValid is false)
            return validationResult;

        var preProcessingResult = PreProcessActivation(request);

        var focus = CreateFocus(request, preProcessingResult);
        FocusQueue.Add(focus);
        
        RaiseActivated();
        return ValidationResult.Valid;
    }
    
    protected abstract ValidationResult ValidateActivation(TActivationRequest request);

    protected virtual TPreProcessingResult PreProcessActivation(TActivationRequest request) => default!;

    protected abstract TFocus CreateFocus(TActivationRequest activationRequest, 
        TPreProcessingResult preProcessingResult);

    protected void RequestExecution()
    {
        foreach (var focus in FocusQueue)
        {
            RequestExecution(focus);
        }
    }
    
    private void RequestExecution(TFocus focus) => ExecutionRequested(this, focus);

    public void OnExecutionRequested(IAbilityFocus focus)
    {
        if (focus is not TFocus typedFocus)
        {
            GD.PrintErr($"Invalid {nameof(IAbilityFocus)} type. Expected '{typeof(TFocus).Name}', but got " +
                        $"'{focus.GetType().Name}'");
            return;
        }

        OnExecutionRequested(typedFocus);
    }

    protected virtual void OnExecutionRequested(TFocus focus) => Complete(focus);

    protected abstract void Complete(TFocus focus);
    
    protected void RaiseActivated() => Activated(this);

    protected virtual void StartCooldown()
    {
        RemainingCooldown.ResetDuration();
    }
    
    protected virtual void OnCooldownEnded()
    {
        CooldownEnded(this);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IAbilityNode other) return false;
        
        return Id.Equals(other.Id) 
               && OwnerActor.InstanceId.Equals(other.OwnerActor.InstanceId);
    }
    
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() => HashCode.Combine(Id, OwnerActor.InstanceId);
}

public interface IAbilityActivationRequest
{
    
}

public interface IAbilityActivationPreProcessingResult
{
    
}

public interface IAbilityFocus
{
    
}