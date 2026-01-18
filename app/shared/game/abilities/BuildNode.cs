using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Shape;
using LowAgeData.Domain.Entities;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Resources;

public partial class BuildNode : ActiveAbilityNode<
        BuildNode.ActivationRequest, 
        BuildNode.PreProcessingResult, 
        BuildNode.Focus>, 
    INodeFromBlueprint<Build>, IAbilityHasSelection, IAbilityHasTargetArea
{
    private const string ScenePath = @"res://app/shared/game/abilities/BuildNode.tscn";
    private static BuildNode Instance() => (BuildNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static BuildNode InstantiateAsChild(Build blueprint, Node parentNode, ActorNode owner)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.SetBlueprint(blueprint);
        ability.OwnerActor = owner;
        return ability;
    }

    public IList<Selection<EntityId>> Selection { get; private set; } = null!;
    
    private Build Blueprint { get; set; } = null!;
    private IShape PlacementArea { get; set; } = null!;
    
    public void SetBlueprint(Build blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        PlacementArea = Blueprint.PlacementArea;
        Selection = Blueprint.Selection;
        CasterConsumesAction = blueprint.CasterConsumesAction;
    }

    public bool WholeMapIsTargeted() => PlacementArea is LowAgeData.Domain.Common.Shape.Map 
                                        && Blueprint.UseWalkableTilesAsPlacementArea is false;

    public IEnumerable<Vector2Int> GetTargetPositions(EntityNode caster)
    {
        if (Blueprint.UseWalkableTilesAsPlacementArea && caster is StructureNode structure)
        {
            return structure.WalkablePositions;
        }

        var mapSize = GlobalRegistry.Instance.MapSize;
        return PlacementArea.ToPositions(caster, mapSize);
    }

    public IEnumerable<Vector2> GetGlobalPositionsOfFocusedTargets()
    {
        var globalPositions = new List<Vector2>();
        foreach (var focus in FocusQueue)
        {
            var entityToBuild = GlobalRegistry.Instance.GetEntityById(focus.EntityToBuildId);
            if (entityToBuild is null)
                continue;
            
            globalPositions.Add(entityToBuild.GlobalPosition);
        }
        
        return globalPositions;
    }

    public int GetSelectableItemCost(Id selectableItemId)
    {
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        var celestium = item.Cost.FirstOrDefault(i => i.Resource.Equals(ResourceId.Celestium));
        return celestium?.Amount ?? 0;
    }

    public string GetSelectableItemText(Id selectableItemId)
    {
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        var entity = Data.Instance.GetEntityBlueprintById(item.Name);

        var cost = item.Cost.Aggregate(string.Empty, (current, payment) 
            => current + $"{payment.Amount} " +
               $"{Data.Instance.Blueprint.Resources.First(x => x.Id.Equals(payment.Resource)).DisplayName}, ");
        cost = cost.Remove(cost.Length - 2);

        var research = string.Empty;
        if (item.ResearchNeeded.Any())
        {
            research += "\nResearch needed to unlock: ";
            research = item.ResearchNeeded.Aggregate(research, (current, researchId) 
                => current + $"{researchId}, ");
            research = research.Remove(research.Length - 2);
        }

        return $"Build {entity.DisplayName} \n" +
               $"{research}" +
               $"\nCost: {cost.WrapToLines(Constants.MaxTooltipCharCount)} \n\n" + // TODO describe how long it will
                                                                                   // take to build this with the
                                                                                   // current income
               $"{entity.Description.WrapToLines(Constants.MaxTooltipCharCount)}";
    }

    public bool IsSelectableItemDisabled(Id selectableItemId)
    {
        var researchEnabled = Config.Instance.ResearchEnabled;
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        return (item.ResearchNeeded.Any() && researchEnabled) || item.GrayOutIfAlreadyExists;
    }
    
    protected override void CancelActivation(ActivationRequest request)
    {
        var buildableEntity = request.EntityToBuild;
        
        if (buildableEntity is null)
            return;

        var creationProgress = buildableEntity.CreationProgress;
        creationProgress?.Helpers.Remove(this);
        
        if (buildableEntity.IsCandidate() && creationProgress is not null && creationProgress.Helpers.IsEmpty())
            buildableEntity.Destroy(); // No one is working on this anymore
        
        // TODO Refund
        // Can be generic (if split into "CanRefund"): Refund consumable resources (if no helpers left AND no payment progress made)

        RefundAction();

        var focus = FocusQueue.FirstOrDefault(f => f.EntityToBuildId.Equals(buildableEntity.InstanceId));
        
        if (focus is not null)
            FocusQueue.Remove(focus);

        if (FocusQueue.IsEmpty())
            OwnerActor.WorkingOn = [];
    }
    
    protected override ValidationResult ValidateActivation(ActivationRequest request) => AbilityValidator.With([
            // TODO missing validations: research, consumable resources
            new AbilityValidator.BuildableEntityCanBePlaced
            {
                Entity = request.EntityToBuild,
                AlreadyPlaced = request.EntityAlreadyPlaced
            },
            new AbilityValidator.CorrectTurnPhase
            {
                CurrentTurnPhase = GlobalRegistry.Instance.GetCurrentPhase(),
                RequiredTurnPhase = Blueprint.TurnPhase
            },
            new AbilityValidator.ActorHasEnoughAction
            {
                Actor = OwnerActor,
                ActionNeeded = CasterConsumesAction
            },
            new AbilityValidator.TargetWithinArea
            {
                AvailablePositions = GetTargetPositions(OwnerActor),
                TargetPositions = request.EntityToBuild!.EntityOccupyingPositions
            },
            new AbilityValidator.HelpApplicableAndAllowed
            {
                EntityToBuild = request.EntityToBuild,
                HelpingAbilityInstanceId = InstanceId,
                HelpingAllowed = Blueprint.CanHelp
            }
        ])
        .Validate();

    protected override AbilityReservationResult HandleReservation(ActivationRequest request)
    {
        request.EntityToBuild!.CreationProgress!.Helpers[this] = Blueprint.HelpEfficiency;
        
        return base.HandleReservation(request);
    }

    protected override PreProcessingResult CreatePreProcessingResult(ActivationRequest request,
        AbilityReservationResult reservation) => new()
    {
        Reservation = reservation
    };

    protected override Focus CreateFocus(ActivationRequest activationRequest, 
        PreProcessingResult preProcessingResult) => new()
    {
        Requeued = false,
        Reservation = preProcessingResult.Reservation,
        EntityToBuildId = activationRequest.EntityToBuild!.InstanceId
    };

    protected override bool TryExecutePrePayment(Focus focus)
    {
        var entityToBuild = GlobalRegistry.Instance.GetEntityById(focus.EntityToBuildId);

        if (entityToBuild is null)
            return false;

        if (entityToBuild.IsCompleted())
        { 
            // TODO what if there are other conditions? Might need to be tested: destroyed, null (???)
            CancelActivation(focus.ToActivationRequest());
            return false;
        }

        SpendActionAndConsumableResources(focus);
        return true;
    }

    protected override bool TryExecutePostPayment(Focus focus)
    {
        var entity = GlobalRegistry.Instance.GetEntityById(focus.EntityToBuildId);

        if (entity?.CreationProgress is null)
            return true; // For some reason entity or creation progress was not found,
                         // so it is best to try and complete the ability
            
        if (focus.Reservation.IsReservedFor(Players.Instance.Current) is false) 
            entity.CreationProgress.Helpers[this] = Blueprint.HelpEfficiency;
        
        entity.CreationProgress.UpdateProgress(50); // TODO pass in resources

        var completed = entity.IsCompleted();
        
        GD.Print($"Building completed: '{completed}'");
        
        return completed;
    }
    
    protected override void Complete(Focus focus)
    {
        var entity = GlobalRegistry.Instance.GetEntityById(focus.EntityToBuildId);
        
        if (entity is null)
            return; // Something is fishy, best to move on
        
        entity.CreationProgress?.Helpers.Clear(); // It's OK if CreationProgress doesn't even exist at this point

        FocusQueue.Remove(focus);
        
        if (FocusQueue.IsEmpty())
            OwnerActor.WorkingOn = [];
    }
    
    public class ActivationRequest : IConsumableAbilityActivationRequest
    {
        public required bool UseConsumableResources { get; init; }
        public required bool EntityAlreadyPlaced { get; init; }
        public required EntityNode? EntityToBuild { get; init; }
    }

    public class PreProcessingResult : IActiveAbilityActivationPreProcessingResult
    {
        public AbilityReservationResult Reservation { get; init; }
    }

    public class Focus : IActiveAbilityFocus
    {
        public bool Requeued { get; set; }
        public required AbilityReservationResult Reservation { get; set; }
        
        public required Guid EntityToBuildId { get; init; }
        //public required IList<Payment> Cost { get; init; }
        //public required IList<Payment> PaymentPaid { get; init; }

        public IConsumableAbilityActivationRequest ToActivationRequest() => new ActivationRequest
        {
            UseConsumableResources = false, // TODO focus.Reservation.ReservedConsumableResources,
            EntityAlreadyPlaced = true,
            EntityToBuild = GlobalRegistry.Instance.GetEntityById(EntityToBuildId)
        };

        public IConsumableAbilityActivationRequest ToActivationRequestForRequeue() => new ActivationRequest
        {
            UseConsumableResources = false,
            EntityAlreadyPlaced = true,
            EntityToBuild = GlobalRegistry.Instance.GetEntityById(EntityToBuildId)
        };

        private bool Equals(Focus other) => EntityToBuildId.Equals(other.EntityToBuildId);

        public bool Equals(IActiveAbilityFocus? other) => Equals((object?)other);

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Focus)obj);
        }

        public override int GetHashCode() => EntityToBuildId.GetHashCode();
    }
}
