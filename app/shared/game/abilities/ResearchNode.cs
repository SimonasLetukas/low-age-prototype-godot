using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Researches;
using Newtonsoft.Json;
using ResearchAbility = LowAgeData.Domain.Abilities.Research;

public partial class ResearchNode : ActiveAbilityNode<
        ResearchNode.ActivationRequest, 
        ResearchNode.PreProcessingResult, 
        ResearchNode.Focus>,
    INodeFromBlueprint<ResearchAbility>, IAbilityHasSelection
{
    private const string ScenePath = @"res://app/shared/game/abilities/ResearchNode.tscn";
    private static ResearchNode Instance() => (ResearchNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static ResearchNode InstantiateAsChild(ResearchAbility blueprint, Node parentNode, ActorNode owner)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.OwnerActor = owner;
        ability.SetBlueprint(blueprint);
        return ability;
    }
    
    public IList<Selection<ResearchId>> Selection { get; private set; } = null!;
    
    private ResearchAbility Blueprint { get; set; } = null!;
    
    public void SetBlueprint(ResearchAbility blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        Selection = Blueprint.Selection;
    }

    public string GetResearchProgressText()
    {
        if (IsActivated is false)
            return string.Empty;

        var focus = FocusQueue.FirstOrDefault();
        if (focus is null)
            return string.Empty;

        var cost = GetCostForSelectedResearch(focus.ResearchId);
        var playerStockpile = Registry.GetCurrentPlayerStockpile(OwnerActor.Player);
        var remainingUpdateCount = Registry.SimulateProductionLength(
            cost,
            playerStockpile,
            focus.NonConsumableResourcesPaidSoFar,
            1f);
        var remainingUpdateText = remainingUpdateCount == int.MaxValue ? "too many" : remainingUpdateCount.ToString();
        
        var actualPlayerIncome = Registry.GetActualPlayerIncome(OwnerActor.Player, 1f, 1);
        var nonConsumableIncome = Registry.GetNonConsumableResources(actualPlayerIncome);
        var nonConsumableCost = Registry.GetNonConsumableResources(cost);
        var filteredIncome = nonConsumableIncome
            .Where(i => nonConsumableCost.Any(c => c.Resource.Equals(i.Resource)))
            .ToList();
        
        var appliedIncomeText = Registry.StringifyResources(filteredIncome);
        var remainingCostText = Registry.StringifyResources(Registry.SubtractResources(
            nonConsumableCost, 
            focus.NonConsumableResourcesPaidSoFar, 
            true));

        var research = Registry.GetResearchById(focus.ResearchId);
        
        var text = $"Currently researching {research.DisplayName} which will finish in {remainingUpdateText} " +
                   $"turns (with a combined production of {appliedIncomeText}, which is used to cover the " +
                   $"remaining {remainingCostText} production).\n\n" +
                   $"Research effect: {research.Description}";

        return text;
    }
    
    public IList<Payment> GetSelectableItemCost(Id selectableItemId)
    {
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        return item.Cost;
    }

    public string GetSelectableItemText(Id selectableItemId)
    {
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        var researchItem = Registry.GetResearchById(item.Name);

        if (Registry.GetResearchByPlayer(OwnerActor.Player).Contains(researchItem.Id))
        {
            var message = $"Already researched: {researchItem.DisplayName} \n\n" +
                          $"{researchItem.Description}";
        
            return message.WrapToLines(Constants.MaxTooltipCharCount);
        }

        var cost = item.Cost.Aggregate(string.Empty, (current, payment) 
            => current + $"{payment.Amount} " +
               $"{Data.Instance.Blueprint.Resources.First(x => x.Id.Equals(payment.Resource)).DisplayName}, ");
        cost = cost.Remove(cost.Length - 2);

        var researchNeeded = string.Empty;
        if (item.ResearchNeeded.Any())
        {
            researchNeeded += "\nResearch needed to unlock: ";
            researchNeeded = item.ResearchNeeded.Aggregate(researchNeeded, (current, researchId) 
                => current + $"{Registry.GetResearchById(researchId).DisplayName}, ");
            researchNeeded = researchNeeded.Remove(researchNeeded.Length - 2);
        }

        var stockpile = Registry.GetCurrentPlayerStockpile(OwnerActor.Player);
        var turnsNeeded = Registry.SimulateProductionLength(item.Cost, stockpile, [], 1);
        
        var turns = turnsNeeded == int.MaxValue
            ? "too long"
            : $"{turnsNeeded} turn(s) to produce";
        var finish = $"With the current income it will take {turns}.";

        var finalMessage = $"{researchItem.DisplayName} \n" +
                           $"{researchNeeded}" +
                           $"\nCost: {cost}." + 
                           $"\n\n{finish}\n\n" + 
                           $"{researchItem.Description}";
        
        return finalMessage.WrapToLines(Constants.MaxTooltipCharCount);
    }

    public bool IsSelectableItemDisabled(Id selectableItemId)
    {
        var researchEnabled = Config.Instance.ResearchEnabled;
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        
        var alreadyExists = item.GrayOutIfAlreadyExists 
                            && Registry.GetResearchByPlayer(OwnerActor.Player).Contains(item.Name);
        
        return (item.ResearchNeeded.Any() && researchEnabled) || alreadyExists;
    }
    
    protected override void CancelActivation(ActivationRequest request)
    {
        var focus = FocusQueue.FirstOrDefault();
        
        if (Log.VerboseDebugEnabled)
            Log.Info(nameof(ResearchNeeded), nameof(CancelActivation), 
                $"Found focus '{JsonConvert.SerializeObject(focus)}'.");
        
        if (focus is not null)
        {
            if (focus.NonConsumableResourcesPaidSoFar.IsEmpty()) 
                RefundResources(ConsumableCost);
            
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
            new AbilityValidator.HasEnoughConsumableResources
            {
                UseConsumableResources = request.UseConsumableResources,
                ConsumableCost = GetCostForSelectedResearch(request.ResearchId),
                Player = OwnerActor.Player
            }
        ])
        .Validate();

    protected override IList<Payment> ReserveResources(ActivationRequest request)
    {
        if (request.UseConsumableResources is false)
            return [];

        var cost = GetCostForSelectedResearch(request.ResearchId);
        var consumableCost = Registry.GetConsumableResources(cost);
        EventBus.Instance.RaiseResourcePaymentRequested(OwnerActor.Player, consumableCost, false);
        return consumableCost;
    }

    protected override PreProcessingResult CreatePreProcessingResult(ActivationRequest request,
        AbilityReservationResult reservation) => new()
    {
        Reservation = reservation
    };

    protected override Focus CreateFocus(ActivationRequest activationRequest, PreProcessingResult? preProcessingResult)
        => new()
        {
            Requeued = false,
            Reservation = preProcessingResult.Reservation,
            ResearchId = activationRequest.ResearchId
        };

    protected override bool ExecutePostPaymentAndDetermineIfPaymentCompleted(Focus focus)
    {
        var nonConsumableCost = Registry.GetNonConsumableResources(Selection
            .First(x => x.Name.Equals(focus.ResearchId))
            .Cost);
        
        ExecuteNonConsumablePayment(focus, nonConsumableCost);
        
        return Registry.IsPaymentComplete(nonConsumableCost, focus.NonConsumableResourcesPaidSoFar);
    }
    
    protected override void ExecuteFocus(Focus focus) 
        => EventBus.Instance.RaiseResearchGained(OwnerActor.Player, focus.ResearchId);

    private IList<Payment> GetCostForSelectedResearch(ResearchId researchId) => Selection
        .FirstOrDefault(s => s.Name.Equals(researchId))?.Cost ?? [];
    
    private IList<Payment> GetNonConsumableStockpile() => Registry.GetNonConsumableResources(
        Registry.GetCurrentPlayerStockpile(OwnerActor.Player));
    
    public class ActivationRequest : IConsumableAbilityActivationRequest
    {
        public bool IsRequeued { get; init; }
        public required bool UseConsumableResources { get; init; }
        public required ResearchId ResearchId { get; init; }
    }

    public class PreProcessingResult : IActiveAbilityActivationPreProcessingResult
    {
        public AbilityReservationResult? Reservation { get; init; } = null!;
    }

    public class Focus : IActiveAbilityFocus
    {
        public bool Requeued { get; set; }
        public required AbilityReservationResult Reservation { get; set; }
        public IList<Payment> NonConsumableResourcesPaidSoFar { get; set; } = [];
        public required ResearchId ResearchId { get; init; }
        
        public IConsumableAbilityActivationRequest ToActivationRequest() => new ActivationRequest
        {
            IsRequeued = false,
            UseConsumableResources = Reservation.ReservedConsumableResources.Any(),
            ResearchId = ResearchId
        };

        public IConsumableAbilityActivationRequest ToActivationRequestForRequeue() => new ActivationRequest
        {
            IsRequeued = true,
            UseConsumableResources = false,
            ResearchId = ResearchId
        };

        private bool Equals(Focus other) => Equals(ResearchId, other.ResearchId);

        public bool Equals(IActiveAbilityFocus? other) => Equals((object?)other);

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Focus)obj);
        }

        public override int GetHashCode() => ResearchId.GetHashCode();
    }
}