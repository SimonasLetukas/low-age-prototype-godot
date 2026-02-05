using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Factions;
using LowAgeData.Domain.Resources;
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
    private readonly Dictionary<IncomeProvider, IReadOnlyDictionary<ResourceId, int>> _currentPaymentByIncomeProvider = new();
    private readonly Dictionary<Player, IReadOnlyDictionary<ResourceId, int>> _resourcesStockpiledByPlayer = new();
    private Dictionary<ResourceId, Resource> _resourceBlueprints = [];
    
    public override void _Ready()
    {
        base._Ready();
        
        GlobalRegistry.Instance.ProvideGetCurrentPlayerStockpile(GetCurrentPlayerStockpile);
        GlobalRegistry.Instance.ProvideGetCurrentPlayerIncome(GetCurrentPlayerIncome);
        GlobalRegistry.Instance.ProvideGetResourcesStoredAs(GetResourcesStoredAs);
        GlobalRegistry.Instance.ProvideSimulatePayment(SimulatePayment);
        GlobalRegistry.Instance.ProvideIsPaymentComplete(IsPaymentComplete);
        GlobalRegistry.Instance.ProvideCanSubtractResources(CanSubtractResources);

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

    public IList<Payment> GetCurrentPlayerStockpile(Player player) => ResourceCalculator.ToList(
        _resourcesStockpiledByPlayer.GetValueOrDefault(player, new Dictionary<ResourceId, int>()));

    public IList<Payment> GetCurrentPlayerIncome(Player player)
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

    public IList<Payment> GetResourcesStoredAs(ResourceId resource, IList<Payment> stockpile) =>
        ResourceCalculator.ToList(ResourceCalculator.GetResourcesStoredAs(resource, 
            ResourceCalculator.ToDictionary(stockpile), _resourceBlueprints));

    public static (IList<Payment> ResourcesSpent, IList<Payment> UpdatedPayment) SimulatePayment(
        IList<Payment> cost, IList<Payment> stockpile, IList<Payment> paidSoFar)
    {
        var (resourcesSpent, updatedPayment) = ResourceCalculator
            .SimulatePayment(
                ResourceCalculator.ToDictionary(cost), 
                ResourceCalculator.ToDictionary(stockpile), 
                ResourceCalculator.ToDictionary(paidSoFar));

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
            _resourceBlueprints);

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
                .SimulatePayment(cost, stockpile, paidSoFar);

            if (ResourceCalculator.TrySubtractResources(stockpile, resourcesSpent, _resourceBlueprints, 
                    out var resultingStockpile) is false)
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

    private void OnPhaseStarted(int turn, TurnPhase phase)
    {
        if (phase.Equals(TurnPhase.Planning) is false)
            return;
        
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

            var currentStockpile = ResourceCalculator.ToList(_resourcesStockpiledByPlayer[player]);
            EventBus.Instance.RaisePlayerResourcesUpdated(player, currentStockpile);
        }
    }

    private void OnPaymentRequested(Player player, IList<Payment> resourcesSpent)
    {
        var stockpile = _resourcesStockpiledByPlayer[player];
        if (ResourceCalculator.TrySubtractResources(stockpile, 
                ResourceCalculator.ToDictionary(resourcesSpent), _resourceBlueprints, 
                out var resultingStockpile) is false)
            return;

        _resourcesStockpiledByPlayer[player] = resultingStockpile;
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