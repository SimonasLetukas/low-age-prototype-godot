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
using Newtonsoft.Json;

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
        ability.OwnerActor = owner;
        ability.SetBlueprint(blueprint);
        return ability;
    }

    public IList<Selection<EntityId>> Selection { get; private set; } = null!;
    
    private Build Blueprint { get; set; } = null!;
    private IShape PlacementArea { get; set; } = null!;

    public override void _Ready()
    {
        base._Ready();

        EventBus.Instance.EntityDestroyed += OnEntityDestroyed;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.EntityDestroyed -= OnEntityDestroyed;
        
        base._ExitTree();
    }

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

        var mapSize = Registry.MapSize;
        return PlacementArea.ToPositions(caster, mapSize);
    }

    public IEnumerable<Vector2> GetGlobalPositionsOfFocusedTargets()
    {
        var globalPositions = new List<Vector2>();
        foreach (var focus in FocusQueue)
        {
            var entityToBuild = Registry.GetEntityById(focus.EntityToBuildId);
            if (entityToBuild is null)
                continue;
            
            globalPositions.Add(entityToBuild.GlobalPosition);
        }
        
        return globalPositions;
    }

    public IList<Payment> GetSelectableItemCost(Id selectableItemId)
    {
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        return item.Cost;
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
            research += "\nResearch needed: ";
            research = item.ResearchNeeded.Aggregate(research, (current, researchId) 
                => current + $"{Registry.GetResearchById(researchId).DisplayName}, ");
            research = research.Remove(research.Length - 2);
        }

        var stockpile = Registry.GetCurrentPlayerStockpile(OwnerActor.Player);
        var turnsNeeded = Registry.SimulateProductionLength(item.Cost, stockpile, [], 1);
        
        var turns = turnsNeeded == int.MaxValue
            ? "too long"
            : $"{turnsNeeded} turn(s) to produce";
        var finish = $"With the current income it will take {turns}.";

        var finalMessage = $"{entity.DisplayName} \n" +
                           $"{research}" +
                           $"\nCost: {cost}." + 
                           $"\n\n{finish}\n\n" + 
                           $"{entity.Description}";
        
        return finalMessage.WrapToLines(Constants.MaxTooltipCharCount);
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
        var nonConsumableStockpile = GetNonConsumableStockpile();
        creationProgress?.UpdateDescription(nonConsumableStockpile);

        var costOfDestroyedEntity = new List<Payment>();
        if (buildableEntity.IsCandidate() && creationProgress is not null && creationProgress.Helpers.IsEmpty())
        {
            // No one is working on this anymore
            buildableEntity.Destroy(null); 
            costOfDestroyedEntity = creationProgress.TotalCost;
        }
        
        var focus = FocusQueue.FirstOrDefault(f => f.EntityToBuildId.Equals(buildableEntity.InstanceId));

        if (focus is not null)
        {
            if (costOfDestroyedEntity.Count != 0) 
                RefundResources(costOfDestroyedEntity);
            
            RemoveFocus(focus);
        }
        
        if (FocusQueue.IsEmpty())
            RefundAction();
    }
    
    protected override ValidationResult ValidateActivation(ActivationRequest request) => AbilityValidator.With([
            new AbilityValidator.PlayerHasResearch
            {
                Player = OwnerActor.Player,
                ResearchNeeded = Blueprint.ResearchNeeded,
            },
            new AbilityValidator.CorrectTurnPhase
            {
                CurrentTurnPhase = Registry.GetCurrentPhase(),
                RequiredTurnPhase = Blueprint.TurnPhase,
                IsRequeued = request.IsRequeued
            },
            new AbilityValidator.ActorHasEnoughAction
            {
                Actor = OwnerActor,
                ActionNeeded = CasterConsumesAction,
                IsRequeued = request.IsRequeued
            },
            new AbilityValidator.BuildableEntityCanBePlaced
            {
                Entity = request.EntityToBuild,
                AlreadyPlaced = request.EntityAlreadyPlaced,
                Selection = Selection
            },
            new AbilityValidator.TargetWithinArea
            {
                AvailablePositions = GetTargetPositions(OwnerActor),
                TargetPositions = request.EntityToBuild?.EntityOccupyingPositions!
            },
            new AbilityValidator.HasEnoughConsumableResources
            {
                UseConsumableResources = request.UseConsumableResources,
                ConsumableCost = GetCostForSelectedEntity(request.EntityToBuild),
                Player = OwnerActor.Player
            },
            new AbilityValidator.HelpApplicableAndAllowed
            {
                EntityToBuild = request.EntityToBuild!,
                Helper = this,
                HelpingAllowed = Blueprint.CanHelp,
                NonConsumableStockpile = GetNonConsumableStockpile(),
                IsRequeued = request.IsRequeued
            }
        ])
        .Validate();

    protected override AbilityReservationResult HandleReservation(ActivationRequest request)
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(BuildNode), nameof(HandleReservation), 
                $"{OwnerActor} trying to reserve request ({request.EntityToBuild}, already placed " +
                $"{request.EntityAlreadyPlaced}, use consumables {request.UseConsumableResources}). Creation " +
                $"progress (completed {request.EntityToBuild?.IsCompleted()}, helpers " +
                $"{request.EntityToBuild?.CreationProgress?.Helpers.Count})");

        if (request.EntityToBuild is null || request.EntityToBuild.IsCompleted())
        {
            // This may be true for non-current player clients, in which case just try to pay whatever they paid,
            // even if one of the helpers managed to complete building in the same timing window just before this
            // execution.
            return base.HandleReservation(request);
        } 
        
        request.EntityToBuild!.CreationProgress!.Helpers[this] = Blueprint.HelpEfficiency;
        
        var nonConsumableStockpile = GetNonConsumableStockpile();
        request.EntityToBuild!.CreationProgress!.UpdateDescription(nonConsumableStockpile);
        
        return base.HandleReservation(request);
    }

    protected override IList<Payment> ReserveResources(ActivationRequest request)
    {
        if (request.UseConsumableResources is false)
            return [];

        var cost = GetCostForSelectedEntity(request.EntityToBuild);
        var consumableCost = Registry.GetConsumableResources(cost);
        EventBus.Instance.RaiseResourcePaymentRequested(OwnerActor.Player, consumableCost, false);
        return consumableCost;
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

    protected override void RequestExecution(Focus focus)
    {
        var entity = Registry.GetEntityById(focus.EntityToBuildId);
        focus.EfficiencyFactor = entity?.CreationProgress?.CalculateEfficiencyFactor([]);
        
        base.RequestExecution(focus);
    }

    protected override bool ExecutePostPaymentAndDetermineIfPaymentCompleted(Focus focus)
    {
        var entity = Registry.GetEntityById(focus.EntityToBuildId);

        if (entity?.CreationProgress is null)
            return true; // For some reason entity or creation progress was not found,
                         // so it is best to try and complete the ability
            
        if (focus.Reservation.IsReservedFor(Players.Instance.Current) is false) 
            entity.CreationProgress.Helpers[this] = Blueprint.HelpEfficiency;
        
        var nonConsumableStockpile = GetNonConsumableStockpile();
        if (Log.DebugEnabled)
            Log.Info(nameof(BuildNode), nameof(ExecutePostPaymentAndDetermineIfPaymentCompleted), 
                $"{nameof(nonConsumableStockpile)} '{JsonConvert.SerializeObject(nonConsumableStockpile)}'");
        
        entity.CreationProgress.UpdateProgress(nonConsumableStockpile, focus.EfficiencyFactor);

        var completed = entity.IsCompleted();
        
        if (Log.DebugEnabled)
            Log.Info(nameof(BuildNode), nameof(ExecutePostPaymentAndDetermineIfPaymentCompleted), 
                $"Building completed '{completed}'");
        
        return completed;
    }

    protected override bool WasAlreadyCompleted(Focus focus)
    {
        var entity = Registry.GetEntityById(focus.EntityToBuildId);

        if (entity?.CreationProgress is null || entity.IsCompleted()) // Meaning that building has completed
            return true;

        return false;
    }

    protected override void ExecuteFocus(Focus focus)
    {
        // Real execution happens during UpdateProgress, so this method is used for cleaning up.
        
        var entity = Registry.GetEntityById(focus.EntityToBuildId);
        
        if (entity is null)
        {
            // Something is fishy, best to move on
            RemoveFocus(focus);
            return; 
        }
        
        entity.CreationProgress?.Helpers.Clear(); // It's OK if CreationProgress doesn't even exist at this point
        
        if (Log.DebugEnabled)
            Log.Info(nameof(BuildNode), nameof(ExecuteFocus), 
                $"{OwnerActor} completed focus '{JsonConvert.SerializeObject(focus)}'. Focus queue " +
                $"{FocusQueue.Count}. Entity is completed {entity.IsCompleted()}");
    }

    private IList<Payment> GetCostForSelectedEntity(EntityNode? entityToBuild) => Selection
        .FirstOrDefault(s => s.Name.Equals(entityToBuild?.BlueprintId))?.Cost ?? [];

    private IList<Payment> GetNonConsumableStockpile() => Registry.GetNonConsumableResources(
        Registry.GetCurrentPlayerStockpile(OwnerActor.Player));

    private void OnEntityDestroyed(EntityNode entity, EntityNode? source, bool triggersOnDeathBehaviours)
    {
        foreach (var focus in FocusQueue.ToList())
        {
            if (focus.EntityToBuildId.Equals(entity.InstanceId) is false)
                continue;

            var request = focus.ToActivationRequest();
            CancelActivation(request);
        }
    }
    
    public class ActivationRequest : IConsumableAbilityActivationRequest
    {
        public bool IsRequeued { get; init; }
        public required bool UseConsumableResources { get; init; }
        public required bool EntityAlreadyPlaced { get; init; }
        public required EntityNode? EntityToBuild { get; init; }
    }

    public class PreProcessingResult : IActiveAbilityActivationPreProcessingResult
    {
        public AbilityReservationResult Reservation { get; init; } = null!;
    }

    public class Focus : IActiveAbilityFocus
    {
        public bool Requeued { get; set; }
        public required AbilityReservationResult Reservation { get; set; }
        public IList<Payment> NonConsumableResourcesPaidSoFar { get; set; } = []; 
        public float? EfficiencyFactor { get; set; }
        public required Guid EntityToBuildId { get; init; }

        public IConsumableAbilityActivationRequest ToActivationRequest() => new ActivationRequest
        {
            IsRequeued = false,
            UseConsumableResources = Reservation.ReservedConsumableResources.Any(),
            EntityAlreadyPlaced = true,
            EntityToBuild = GlobalRegistry.Instance.GetEntityById(EntityToBuildId)
        };

        public IConsumableAbilityActivationRequest ToActivationRequestForRequeue() => new ActivationRequest
        {
            IsRequeued = true,
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
