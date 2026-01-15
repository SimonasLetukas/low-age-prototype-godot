using System.Collections.Generic;
using Godot;
using LowAgeData.Domain.Common;
using ResearchAbility = LowAgeData.Domain.Abilities.Research;

public partial class ResearchNode : ActiveAbilityNode<
        ResearchNode.ActivationRequest, 
        ResearchNode.PreProcessingResult, 
        ResearchNode.Focus>,
    INodeFromBlueprint<ResearchAbility>
{
    private const string ScenePath = @"res://app/shared/game/abilities/ResearchNode.tscn"; // TODO
    private static ResearchNode Instance() => (ResearchNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static ResearchNode InstantiateAsChild(ResearchAbility blueprint, Node parentNode, ActorNode owner)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.SetBlueprint(blueprint);
        ability.OwnerActor = owner;
        return ability;
    }
    
    private ResearchAbility Blueprint { get; set; } = null!;
    
    public void SetBlueprint(ResearchAbility blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }

    protected override ValidationResult ValidateActivation(ActivationRequest request)
    {
        throw new System.NotImplementedException();
    }
    
    protected override void CancelActivation(ActivationRequest request)
    {
        throw new System.NotImplementedException();
    }

    protected override Focus CreateFocus(ActivationRequest activationRequest, PreProcessingResult? preProcessingResult)
    {
        throw new System.NotImplementedException();
    }

    protected override PreProcessingResult CreatePreProcessingResult(ActivationRequest request, AbilityReservationResult reservation)
    {
        throw new System.NotImplementedException();
    }

    protected override bool TryExecutePrePayment(Focus focus)
    {
        throw new System.NotImplementedException();
    }

    protected override bool TryExecutePostPayment(Focus focus)
    {
        throw new System.NotImplementedException();
    }
    
    protected override void Complete(Focus focus)
    {
        throw new System.NotImplementedException();
    }
    
    public class ActivationRequest : IConsumableAbilityActivationRequest
    {
        public required bool UseConsumableResources { get; init; }
    }

    public class PreProcessingResult : IActiveAbilityActivationPreProcessingResult
    {
        public AbilityReservationResult? Reservation { get; init; }
    }

    public class Focus : IActiveAbilityFocus
    {
        public bool Requeued { get; set; }
        public required AbilityReservationResult Reservation { get; set; }
        
        public IConsumableAbilityActivationRequest ToActivationRequest()
        {
            throw new System.NotImplementedException();
        }

        public IConsumableAbilityActivationRequest ToActivationRequestForRequeue()
        {
            throw new System.NotImplementedException();
        }

        private bool Equals(Focus other) => throw new System.NotImplementedException();

        public bool Equals(IActiveAbilityFocus? other) => Equals((object?)other);

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Focus)obj);
        }

        public override int GetHashCode() => throw new System.NotImplementedException();
    }
}