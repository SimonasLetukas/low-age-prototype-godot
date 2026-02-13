using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Factions;
using LowAgeData.Domain.Resources;
using Newtonsoft.Json;
using Resource = LowAgeData.Domain.Resources.Resource;

/// <summary>
/// <para>
/// Exposes current resources for each player via <see cref="GlobalRegistry"/> or directly. Also exposes validation
/// helpers to check if the current resources are enough for payment.
/// </para>
/// <para>
/// Aggregates all registrations of <see cref="IncomeNode"/> and updates the current resources on each turn change.
/// </para>
/// </summary>
public partial class Resources : Node2D
{
    private static bool DebugEnabled => false;
    
    private readonly Dictionary<IncomeProvider, IReadOnlyDictionary<ResourceId, int>> _currentPaymentByIncomeProvider = new();
    private readonly Dictionary<Player, IReadOnlyDictionary<ResourceId, int>> _resourcesStockpiledByPlayer = new();
    private Dictionary<ResourceId, Resource> _resourceBlueprints = [];
    
    public override void _Ready()
    {
        base._Ready();

        GlobalRegistry.Instance.ProvideStringifyResources(StringifyResources);
        GlobalRegistry.Instance.ProvideGetNonConsumableResources(GetNonConsumableResources);
        GlobalRegistry.Instance.ProvideGetCurrentPlayerStockpile(GetCurrentPlayerStockpile);
        GlobalRegistry.Instance.ProvideGetMaximumPlayerIncome(GetMaximumPlayerIncome);
        GlobalRegistry.Instance.ProvideGetActualPlayerIncome(GetActualPlayerIncome);
        GlobalRegistry.Instance.ProvideGetResourcesStoredAs(GetResourcesStoredAs);
        GlobalRegistry.Instance.ProvideSimulateProductionLength(SimulateProductionLength);
        GlobalRegistry.Instance.ProvideSimulatePayment(SimulatePayment);
        GlobalRegistry.Instance.ProvideIsPaymentComplete(IsPaymentComplete);
        GlobalRegistry.Instance.ProvideCanSubtractResources(CanSubtractResources);
        GlobalRegistry.Instance.ProvideSubtractResources(SubtractResources);

        PopulatePlayerResources();

        EventBus.Instance.IncomeProviderRegistered += OnIncomeProviderRegistered;
        EventBus.Instance.IncomeProviderUnregistered += OnIncomeProviderUnregistered;
        EventBus.Instance.PaymentRequested += OnPaymentRequested;
        EventBus.Instance.PhaseStarted += OnPhaseStarted;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.IncomeProviderRegistered -= OnIncomeProviderRegistered;
        EventBus.Instance.IncomeProviderUnregistered -= OnIncomeProviderUnregistered;
        EventBus.Instance.PaymentRequested -= OnPaymentRequested;
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        
        base._ExitTree();
    }

    public string StringifyResources(IList<Payment> resources)
    {
        if (resources.Count == 0)
            return "0 resources";
        
        var result = string.Empty;
        for (var i = 0; i < resources.Count; i++)
        {
            var resource = _resourceBlueprints[resources[i].Resource];
            result += $"{resources[i].Amount} {resource.DisplayName}";
            if (i < resources.Count - 1)
                result += ", ";
        }

        return result;
    }

    public IList<Payment> GetNonConsumableResources(IList<Payment> resources) => resources
        .Select(resource => new { resource, resourceBlueprint = _resourceBlueprints[resource.Resource] })
        .Where(r => r.resourceBlueprint.IsConsumable is false)
        .Select(r => r.resource)
        .ToList();

    public IList<Payment> GetCurrentPlayerStockpile(Player player) => ResourceCalculator.ToList(
        _resourcesStockpiledByPlayer.GetValueOrDefault(player, new Dictionary<ResourceId, int>()));

    public IList<Payment> GetMaximumPlayerIncome(Player player)
    {
        var playerIncomeProviders = _currentPaymentByIncomeProvider.Keys
            .Where(p => p.Player.Equals(player));
        var result = new Dictionary<ResourceId, int>();
        foreach (var playerIncomeProvider in playerIncomeProviders)
        {
            foreach (var resourceGained in playerIncomeProvider.ResourcesGained)
            {
                result[resourceGained.Resource] = 
                    result.GetValueOrDefault(resourceGained.Resource) + resourceGained.Amount;
            }
        }
        
        return ResourceCalculator.ToList(result);
    }

    public IList<Payment> GetActualPlayerIncome(Player player, float efficiencyFactor, int helpers)
    {
        var playerPaymentByIncomeProvider = _currentPaymentByIncomeProvider
            .Where(p => p.Key.Player.Equals(player))
            .ToDictionary(p => p.Key, p => p.Value);
        
        var contributingIncomeProviders = ResourceCalculator
            .GetContributingIncomeProviders(playerPaymentByIncomeProvider);
        
        var summedResources = ResourceCalculator.GetSummedIncomeProviderResources(
            contributingIncomeProviders.Values, efficiencyFactor, helpers);

        return ResourceCalculator.ToList(summedResources);
    }

    public IList<Payment> GetResourcesStoredAs(ResourceId resource, IList<Payment> stockpile) =>
        ResourceCalculator.ToList(ResourceCalculator.GetResourcesStoredAs(resource, 
            ResourceCalculator.ToDictionary(stockpile), _resourceBlueprints));
    
    public int SimulateProductionLength(IList<Payment> cost, IList<Payment> stockpile, IList<Payment> paidSoFar, 
        float efficiencyFactor)
    {
        var nonConsumableCost = GetNonConsumableResources(cost);
        var turnsNeeded = ResourceCalculator.GetSimulatedProductionLength(
            ResourceCalculator.ToDictionary(nonConsumableCost),
            ResourceCalculator.ToDictionary(stockpile), 
            ResourceCalculator.ToDictionary(paidSoFar),
            efficiencyFactor);

        return turnsNeeded;
    }

    public static (IList<Payment> ResourcesSpent, IList<Payment> UpdatedPayment) SimulatePayment(
        IList<Payment> cost, IList<Payment> stockpile, IList<Payment> paidSoFar, float efficiencyFactor)
    {
        if (DebugEnabled)
            GD.Print($"Received request to simulate payment: cost '{JsonConvert.SerializeObject(cost)}', " +
                     $"stockpile '{JsonConvert.SerializeObject(stockpile)}', paid so far " +
                     $"'{JsonConvert.SerializeObject(paidSoFar)}', efficiency factor '{efficiencyFactor}'");
        
        var (resourcesSpent, updatedPayment) = ResourceCalculator
            .SimulatePayment(
                ResourceCalculator.ToDictionary(cost), 
                ResourceCalculator.ToDictionary(stockpile), 
                ResourceCalculator.ToDictionary(paidSoFar),
                efficiencyFactor);

        return (ResourceCalculator.ToList(resourcesSpent), 
            ResourceCalculator.ToList(updatedPayment));
    }

    public static bool IsPaymentComplete(IList<Payment> cost, IList<Payment> paidSoFar) => ResourceCalculator
        .IsPaymentComplete(
            ResourceCalculator.ToDictionary(cost),
            ResourceCalculator.ToDictionary(paidSoFar));

    public bool CanSubtractResources(IList<Payment> from, IList<Payment> amount) => ResourceCalculator
        .CanSubtractResources(
            ResourceCalculator.ToDictionary(from),
            ResourceCalculator.ToDictionary(amount),
            _resourceBlueprints,
            false);
    
    public IList<Payment> SubtractResources(IList<Payment> from, IList<Payment> amount, bool subtractNonConsumable) => ResourceCalculator
        .ToList(ResourceCalculator.SubtractResources(
            ResourceCalculator.ToDictionary(from), 
            ResourceCalculator.ToDictionary(amount), 
            _resourceBlueprints,
            false,
            subtractNonConsumable));

    private void PopulatePlayerResources()
    {
        var blueprint = Data.Instance.Blueprint;
        _resourceBlueprints = blueprint.Resources.ToDictionary(resource => resource.Id, resource => resource);
        
        foreach (var player in Players.Instance.GetAll())
        {
            var faction = player.Faction;
            var factionBlueprint = blueprint.Factions.First(f => f.Id.Equals(faction));
            var factionResources = factionBlueprint.AvailableResources;
            var startingResources = new Dictionary<ResourceId, int>();
            
            foreach (var factionResource in factionResources)
            {
                var amount = GetStartingResourceAmount(factionBlueprint, factionResource);
                startingResources[factionResource] = amount;
            }
            
            _resourcesStockpiledByPlayer[player] = startingResources;
        }
    }

    private static int GetStartingResourceAmount(Faction factionBlueprint, ResourceId factionResource)
    {
        var startingResource = factionBlueprint.BonusStartingResources
            .FirstOrDefault(r => r.Resource.Equals(factionResource));

        return startingResource?.Amount ?? 0;
    }

    private IReadOnlyDictionary<ResourceId, int> GetUpdatedStockpileAfterPayingForIncome(
        IEnumerable<IncomeProvider> providers, 
        IReadOnlyDictionary<ResourceId, int> stockpile)
    {
        foreach (var provider in providers)
        {
            var cost = ResourceCalculator.ToDictionary(provider.Cost);
            var paidSoFar = _currentPaymentByIncomeProvider[provider];

            if (ResourceCalculator.IsPaymentComplete(cost, paidSoFar))
                continue;

            var (resourcesSpent, updatedPayment) = ResourceCalculator
                .SimulatePayment(cost, stockpile, paidSoFar, 1);

            if (ResourceCalculator.TrySubtractResources(stockpile, resourcesSpent, _resourceBlueprints, 
                    false, out var resultingStockpile) is false)
                continue; // Try to pay for the income provider next time

            stockpile = resultingStockpile;
            _currentPaymentByIncomeProvider[provider] = updatedPayment;
            EventBus.Instance.RaiseIncomeProviderPaymentUpdated(provider, ResourceCalculator.ToList(updatedPayment));
        }

        return stockpile;
    }
    
    private IReadOnlyDictionary<ResourceId, int> GetUpdatedStockpileAfterIncome(IEnumerable<IncomeProvider> providers, 
        IReadOnlyDictionary<ResourceId, int> stockpile)
    {
        var paymentsByProviders = providers
            .ToDictionary(p => p, p => _currentPaymentByIncomeProvider[p]);
        
        var (updatedStockpile, usedIncomeProviders) = ResourceCalculator
            .AddResources(stockpile, paymentsByProviders, _resourceBlueprints);

        foreach (var usedIncomeProvider in usedIncomeProviders)
        {
            _currentPaymentByIncomeProvider[usedIncomeProvider] = new Dictionary<ResourceId, int>();
            EventBus.Instance.RaiseIncomeProviderPaymentUpdated(usedIncomeProvider, 
                ResourceCalculator.ToList(_currentPaymentByIncomeProvider[usedIncomeProvider]));
        }

        return updatedStockpile;
    }
    
    private void RaisePlayerResourcesUpdated(Player player)
    {
        var currentStockpile = ResourceCalculator.ToList(_resourcesStockpiledByPlayer[player]);
        EventBus.Instance.RaisePlayerResourcesUpdated(player, currentStockpile);
    }

    private void OnPhaseStarted(int turn, TurnPhase phase)
    {
        if (phase.Equals(TurnPhase.Planning))
            OnPlanningPhaseStarted();
        
        if (phase.Equals(TurnPhase.Action))
            OnActionPhaseStarted();
    }

    private void OnPlanningPhaseStarted()
    {
        var providersByPlayer = _currentPaymentByIncomeProvider.Keys
            .OrderBy(p => p.Player.Id) // Ensure determinism across clients
            .GroupBy(p => p.Player)
            .ToDictionary(g => g.Key, g => g
                .OrderBy(p => p.EntityId) // Ensure determinism across clients
                .ToList());

        foreach (var player in Players.Instance.GetAll())
        {
            var providers = providersByPlayer.GetValueOrDefault(player, []);
            
            _resourcesStockpiledByPlayer[player] = GetUpdatedStockpileAfterPayingForIncome(
                providers, _resourcesStockpiledByPlayer[player]);
            
            _resourcesStockpiledByPlayer[player] = GetUpdatedStockpileAfterIncome(
                providers, _resourcesStockpiledByPlayer[player]);

            RaisePlayerResourcesUpdated(player);
        }
    }

    private void OnActionPhaseStarted()
    {
        foreach (var (player, stockpile) in _resourcesStockpiledByPlayer)
        {
            foreach (var (resourceId, amount) in stockpile)
            {
                if (amount == 0)
                    continue;
                
                var resource = _resourceBlueprints[resourceId];
                if (resource.PositiveIncomeEffects.IsEmpty() && resource.NegativeIncomeEffects.IsEmpty())
                    continue;
                
                var effects = amount > 0 
                    ? resource.PositiveIncomeEffects 
                    : resource.NegativeIncomeEffects;

                var count = resource.EffectAmountMultipliedByResourceAmount 
                    ? effects.Count 
                    : 1;

                for (var i = 0; i < count; i++)
                {
                    foreach (var effectId in effects)
                    {
                        var chain = new Effects(effectId, player);
                        if (chain.ValidateLast())
                            chain.ExecuteLast();
                    }
                }
            }
        }
    }

    private void OnPaymentRequested(Player player, IList<Payment> resourcesSpent, bool isRefund)
    {
        var stockpile = _resourcesStockpiledByPlayer[player];
        if (ResourceCalculator.TrySubtractResources(stockpile, 
                ResourceCalculator.ToDictionary(resourcesSpent), _resourceBlueprints, 
                isRefund, out var resultingStockpile) is false)
            return;

        _resourcesStockpiledByPlayer[player] = resultingStockpile;
        RaisePlayerResourcesUpdated(player);
    }

    private void OnIncomeProviderRegistered(IncomeProvider provider)
    {
        _currentPaymentByIncomeProvider[provider] = new Dictionary<ResourceId, int>();
        EventBus.Instance.RaiseIncomeProviderPaymentUpdated(provider, 
            ResourceCalculator.ToList(_currentPaymentByIncomeProvider[provider]));
    }
    
    private void OnIncomeProviderUnregistered(IncomeProvider provider)
    {
        _currentPaymentByIncomeProvider.Remove(provider);
    }
}