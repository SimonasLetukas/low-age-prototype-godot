using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Resources;

public static class ResourceCalculator
{
    public static IReadOnlyDictionary<ResourceId, int> ToDictionary(IList<Payment> payments)
    {
        var dictionary = new Dictionary<ResourceId, int>();
        foreach (var payment in payments)
        {
            dictionary[payment.Resource] = payment.Amount;
        }

        return dictionary;
    }

    public static IList<Payment> ToList(IReadOnlyDictionary<ResourceId, int> dictionary) => dictionary
        .Select(entry => new Payment(entry.Key, entry.Value))
        .ToList();
    
    public static (IReadOnlyDictionary<ResourceId, int> ResourcesSpent, 
        IReadOnlyDictionary<ResourceId, int> UpdatedPayment) SimulatePayment(
            IReadOnlyDictionary<ResourceId, int> cost, 
            IReadOnlyDictionary<ResourceId, int> stockpile, 
            IReadOnlyDictionary<ResourceId, int> paidSoFar)
    {
        var resourcesSpent = new Dictionary<ResourceId, int>();
        var updatedPayment = paidSoFar.ToDictionary();

        foreach (var (resource, amountNeeded) in cost)
        {
            var amountPaid = paidSoFar.GetValueOrDefault(resource);
            var amountRemaining = amountNeeded - amountPaid;
            if (amountRemaining <= 0) continue;

            var amountAvailable = stockpile.GetValueOrDefault(resource);
            var amountUsed = Math.Min(amountRemaining, amountAvailable);

            resourcesSpent[resource] = amountUsed;
            updatedPayment[resource] = amountPaid + amountUsed;
        }

        return (resourcesSpent, updatedPayment);
    }

    public static bool IsPaymentComplete(IReadOnlyDictionary<ResourceId, int> cost,
        IReadOnlyDictionary<ResourceId, int> paidSoFar)
    {
        foreach (var (resource, amountNeeded) in cost)
        {
            var amountPaid = paidSoFar.GetValueOrDefault(resource);
            if (amountPaid < amountNeeded) 
                return false;
        }

        return true;
    }

    public static bool TrySubtractResources(IReadOnlyDictionary<ResourceId, int> from,
        IReadOnlyDictionary<ResourceId, int> amount, IReadOnlyDictionary<ResourceId, Resource> resourceBlueprints,
        out IReadOnlyDictionary<ResourceId, int> result)
    {
        result = new Dictionary<ResourceId, int>();
        if (CanSubtractResources(from, amount, resourceBlueprints) is false)
            return false;
            
        result = SubtractResources(from, amount, resourceBlueprints);
        return true;
    }

    public static bool CanSubtractResources(IReadOnlyDictionary<ResourceId, int> from,
        IReadOnlyDictionary<ResourceId, int> amount, IReadOnlyDictionary<ResourceId, Resource> resourceBlueprints)
    {
        foreach (var (resource, value) in amount)
        {
            var blueprint = resourceBlueprints[resource];
            if (blueprint.IsConsumable is false)
                continue;

            if (from.ContainsKey(resource) is false)
                return false;

            var amountAvailable = from.GetValueOrDefault(resource);
            if (amountAvailable < value)
                return false;
        }

        return true;
    }

    public static IReadOnlyDictionary<ResourceId, int> SubtractResources(IReadOnlyDictionary<ResourceId, int> from,
        IReadOnlyDictionary<ResourceId, int> amount, IReadOnlyDictionary<ResourceId, Resource> resourceBlueprints)
    {
        var result = from.ToDictionary();
        foreach (var (resource, value) in amount)
        {
            var blueprint = resourceBlueprints[resource];
            if (blueprint.IsConsumable is false)
                continue;

            if (result.ContainsKey(resource) is false)
                continue;

            var amountAvailable = result.GetValueOrDefault(resource);
            var newValue = Math.Max(amountAvailable - value, 0);
            result[resource] = newValue;
        }

        return result;
    }
    
    public static (IReadOnlyDictionary<ResourceId, int> UpdatedStockpile, 
        IEnumerable<IncomeProvider> UsedIncomeProviders) AddResources(
            IReadOnlyDictionary<ResourceId, int> stockpile, 
            IReadOnlyDictionary<IncomeProvider, IReadOnlyDictionary<ResourceId, int>> paymentsByProviders, 
            IReadOnlyDictionary<ResourceId, Resource> resourceBlueprints)
    {
        var updatedStockpile = stockpile.ToDictionary();
        var contributingIncomeProviders = GetContributingIncomeProviders(
            paymentsByProviders);
        
        foreach (var resource in updatedStockpile.Keys.ToList())
        {
            var resourceBlueprint = resourceBlueprints[resource];
            if (resourceBlueprint.HasBank is false)
                updatedStockpile[resource] = 0;
            
            var currentAmount = updatedStockpile.GetValueOrDefault(resource);
            var additionalAmount = 0;
            
            foreach (var gainedResources in contributingIncomeProviders.Values)
            {
                if (gainedResources.TryGetValue(resource, out var gainedAmount) is false)
                    continue;

                additionalAmount += gainedAmount;
            }
            
            updatedStockpile[resource] = currentAmount + additionalAmount;
        }

        while (contributingIncomeProviders.Count > 0 
               && StockpileIsOverLimit(updatedStockpile, resourceBlueprints, out var resourcesOverLimit))
        {
            var incomeProvidersByResourcesToReject = 
                GetIncomeProvidersByResourcesToReject(resourcesOverLimit, contributingIncomeProviders);
            if (incomeProvidersByResourcesToReject.IsEmpty())
                break; // Retain resources over limit if there are no income providers contributing to that resource
            
            var incomeProviderToReject = resourcesOverLimit
                .Where(incomeProvidersByResourcesToReject.ContainsKey)
                .Select(resourceId => incomeProvidersByResourcesToReject[resourceId])
                .First();

            foreach (var (resource, amount) in contributingIncomeProviders[incomeProviderToReject])
            {
                updatedStockpile[resource] -= amount;
            }

            // Pausing the cost by not resetting it (assuming that the returned adjustedIncomeProviders will
            // have their costs reset)
            if (incomeProviderToReject.WaitForAvailableStorage)
                contributingIncomeProviders.Remove(incomeProviderToReject);
        }
        
        return (updatedStockpile, contributingIncomeProviders.Keys);
    }

    private static Dictionary<IncomeProvider, IReadOnlyDictionary<ResourceId, int>> GetContributingIncomeProviders(
        IReadOnlyDictionary<IncomeProvider, IReadOnlyDictionary<ResourceId, int>> paymentsByProviders)
    {
        var adjustedIncomeByIncomeProviders = new Dictionary<IncomeProvider, IReadOnlyDictionary<ResourceId, int>>();
        
        var providersByType = paymentsByProviders
            .GroupBy(p => p.Key.ProviderType)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (_, providers) in providersByType)
        {
            var diminisher = 0;
            foreach (var (incomeProvider, paidSoFar) in providers)
            {
                var cost = ToDictionary(incomeProvider.Cost);
                if (IsPaymentComplete(cost, paidSoFar) is false)
                    continue;

                var providedResources = ToDictionary(incomeProvider.ResourcesGained);
                
                var adjustedResources = new Dictionary<ResourceId, int>();
                foreach (var (resource, currentAmount) in providedResources)
                {
                    var adjustedAmount = Math.Max(currentAmount - diminisher, 1);
                    adjustedResources[resource] = adjustedAmount;
                }

                adjustedIncomeByIncomeProviders[incomeProvider] = adjustedResources;
                diminisher += incomeProvider.DiminishingReturn;
            }
        }

        return adjustedIncomeByIncomeProviders;
    }

    private static bool StockpileIsOverLimit(IReadOnlyDictionary<ResourceId, int> stockpile,
        IReadOnlyDictionary<ResourceId, Resource> resourceBlueprints, out IList<ResourceId> resourcesOverLimit)
    {
        var fullAmountByResourceOverLimit = new Dictionary<ResourceId, int>(); 
        resourcesOverLimit = new List<ResourceId>(); 

        foreach (var resource in stockpile.Keys)
        {
            var resourceBlueprint = resourceBlueprints[resource];
            if (resourceBlueprint.HasLimit is false)
                continue;

            // TODO adjust in the future if AddMax is ever supported for ResourceModification
            if (resourceBlueprint.StoredAs.Equals(resource))
                continue;

            var allResourcesStoredAsThis = GetResourcesStoredAs(resourceBlueprint.StoredAs, 
                stockpile, resourceBlueprints);
            var maxStorageAmount = stockpile.GetValueOrDefault(resourceBlueprint.StoredAs);
            var currentStoredAmount = allResourcesStoredAsThis.Values.Sum();
            if (currentStoredAmount < maxStorageAmount)
                continue;

            foreach (var (resourceOverLimit, fullAmount) in allResourcesStoredAsThis)
                fullAmountByResourceOverLimit[resourceOverLimit] = fullAmount;
        }

        if (fullAmountByResourceOverLimit.Count == 0)
            return false;

        resourcesOverLimit = fullAmountByResourceOverLimit
            .OrderByDescending(kvp => kvp.Value) // Most abundant resources are first in line to be rejected
            .ThenBy(kvp => kvp.Key.ToString())   // Alphabetical sort for determinism
            .Select(kvp => kvp.Key)
            .ToList();
        
        return true;
    }

    private static Dictionary<ResourceId, int> GetResourcesStoredAs(ResourceId resource,
        IReadOnlyDictionary<ResourceId, int> stockpile, IReadOnlyDictionary<ResourceId, Resource> resourceBlueprints)
    {
        var resources = new Dictionary<ResourceId, int>();
        foreach (var (otherResource, amount) in stockpile)
        {
            var otherResourceBlueprint = resourceBlueprints[otherResource];
            if (otherResourceBlueprint.StoredAs.Equals(resource) is false)
                continue;
            
            resources[otherResource] = amount;
        }
        
        return resources;
    }
    
    private static Dictionary<ResourceId, IncomeProvider> GetIncomeProvidersByResourcesToReject(
        IList<ResourceId> resourcesOverLimit,
        Dictionary<IncomeProvider, IReadOnlyDictionary<ResourceId, int>> incomeProviders)
    {
        var incomeProvidersByResourceToReject = new Dictionary<ResourceId, IncomeProvider>();
        foreach (var resourceId in resourcesOverLimit)
        {
            var smallestIncomeProvider = GetSmallestIncomeProviderFor(resourceId, incomeProviders);
            if (smallestIncomeProvider is null)
                continue;
                
            incomeProvidersByResourceToReject[resourceId] = smallestIncomeProvider;
        }

        return incomeProvidersByResourceToReject;
    }
    
    private static IncomeProvider? GetSmallestIncomeProviderFor(
        ResourceId resource,
        IReadOnlyDictionary<IncomeProvider, IReadOnlyDictionary<ResourceId, int>> incomeProviders)
    {
        return incomeProviders
            .Where(kvp => kvp.Value.ContainsKey(resource))
            .OrderBy(kvp => kvp.Value.GetValueOrDefault(resource, 0)) // Lowest amount of provided resource
            .ThenBy(kvp => kvp.Value.Values.Sum())                    // Lowest amount of provided summed resources
            .ThenBy(kvp => kvp.Key.EntityId)                          // Finally sorted for determinism across clients
            .Select(kvp => kvp.Key)
            .FirstOrDefault();
    }
}