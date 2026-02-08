using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;

public partial class BuildableNode : BehaviourNode, INodeFromBlueprint<Buildable>
{
    private const string ScenePath = @"res://app/shared/game/behaviours/BuildableNode.tscn";
    private static BuildableNode Instance() => (BuildableNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static BuildableNode InstantiateAsChild(Buildable blueprint, Node parentNode, EntityNode parentEntity)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.Parent = parentEntity;
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }
    
    public event Action<(int DeltaGainedHealth, int DeltaGainedShields)> Updated = delegate { };
    public event Action Completed = delegate { };
    
    public Dictionary<BuildNode, float> Helpers { get; } = new();
    public List<Payment> TotalCost { get; private set; } = [];
    private List<Payment> NonConsumableCost { get; set; } = [];
    private List<Payment> PaidCost { get; set; } = [];

    private Buildable Blueprint { get; set; } = null!;
    private GlobalRegistry Registry { get; } = GlobalRegistry.Instance;
    
    public void SetBlueprint(Buildable blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }

    public void SetTotalCost(IList<Payment> cost)
    {
        TotalCost = cost.ToList();
        NonConsumableCost = Registry.GetNonConsumableResources(cost).ToList();
    }

    public bool IsPlacementValid(IList<Tiles.TileInstance?> tiles) => ValidationHandler
        .Validate(Blueprint.PlacementValidators)
        .With(tiles)
        .Handle();

    /// <summary>
    /// This method assumes that <see cref="Helpers"/> was already checked to see if the desired helper is already
    /// helping, i.e. this method only checks for the amount of helpers, not whether a specific helper is helping.
    /// </summary>
    public bool CanAddNewHelper() => Blueprint.MaximumHelpers < 0 || Helpers.Count < Blueprint.MaximumHelpers;
    
    public int GetRemainingProductionLength(IList<Payment> nonConsumableStockpile)
    {
        if (GetResourcesSum(nonConsumableStockpile) <= 0)
            return int.MaxValue;
        
        var isPaymentComplete = Registry.IsPaymentComplete(NonConsumableCost, PaidCost);
        if (isPaymentComplete)
            return 0;

        if (Helpers.Count <= 0)
            return int.MaxValue;
        
        var simulatedPaidCost = PaidCost.ToList();
        var counter = 0;

        while (Registry.IsPaymentComplete(NonConsumableCost, simulatedPaidCost) is false)
        {
            for (var i = 0; i < Helpers.Count; i++)
            {
                var progress = CalculateProgressStep(simulatedPaidCost, nonConsumableStockpile);
                simulatedPaidCost = progress.NewPaidSoFar.ToList();
            }
            
            if (counter > 100)
            {
                counter = int.MaxValue;
                break;
            }
            
            counter++;
        }

        return counter;
    }

    public void UpdateDescription(IList<Payment> nonConsumableStockpile)
    {
        var helperText = Helpers.Count == 1 ? "1 helper is" : $"{Helpers.Count} helpers are";
        var remainingUpdateCount = GetRemainingProductionLength(nonConsumableStockpile);
        var remainingUpdateText = remainingUpdateCount == int.MaxValue ? "too many" : remainingUpdateCount.ToString();
        var appliedIncomeText = Registry.StringifyResources(GetAppliedIncome());
        var remainingCostText = Registry.StringifyResources(Registry.SubtractResources(NonConsumableCost, PaidCost)); // TODO for some reason PaidCost is ignored
        
        Description = $"{helperText} currently working to finish this in " +
                      $"{remainingUpdateText} turns (with a combined production of " +
                      $"{appliedIncomeText}, which is used to cover " + 
                      $"the remaining {remainingCostText} production).\n\n" + Blueprint.Description;
    }
    
    private IList<Payment> GetAppliedIncome()
    {
        var efficiencyFactor = CalculateEfficiencyFactor();
        var actualIncome = Registry.GetActualPlayerIncome(Parent.Player, efficiencyFactor, Helpers.Count);
        var nonConsumableIncome = Registry.GetNonConsumableResources(actualIncome);
        var filteredIncome = nonConsumableIncome
            .Where(i => NonConsumableCost.Any(c => c.Resource.Equals(i.Resource)))
            .ToList();
        
        return filteredIncome;
    }
    
    public void UpdateProgress(IList<Payment> nonConsumableStockpile)
    {
        UpdateDescription(nonConsumableStockpile);

        var isPaymentComplete = Registry.IsPaymentComplete(NonConsumableCost, PaidCost);
        if (isPaymentComplete)
        {
            Completed();
            return;
        }

        var previousPaidCost = PaidCost;
        var progress = CalculateProgressStep(PaidCost, nonConsumableStockpile);
        PaidCost = progress.NewPaidSoFar.ToList();

        UpdateVitals(previousPaidCost, progress.Completed);

        if (progress.Completed)
            Completed();
    }
    
    private ProgressStep CalculateProgressStep(IList<Payment> paidSoFar, IList<Payment> nonConsumableStockpile)
    {
        var isPaymentComplete = Registry.IsPaymentComplete(NonConsumableCost, paidSoFar);
        if (isPaymentComplete)
            return new ProgressStep
            {
                NewPaidSoFar = paidSoFar,
                Completed = true
            };

        var efficiencyFactor = CalculateEfficiencyFactor();
        var (_, updatedPaidSoFar) = Registry.SimulatePayment(NonConsumableCost,
            nonConsumableStockpile, paidSoFar, efficiencyFactor);

        return new ProgressStep
        {
            NewPaidSoFar = updatedPaidSoFar,
            Completed = Registry.IsPaymentComplete(NonConsumableCost, updatedPaidSoFar)
        };
    }

    private void UpdateVitals(IList<Payment> previousPaidCost, bool completed)
    {
        var maxHealth = 0;
        var maxShields = 0;
        if (Parent is ActorNode actor)
        {
            maxHealth = actor.HasHealth ? actor.Health!.MaxAmount : 0;
            maxShields = actor.HasShields ? actor.Shields!.MaxAmount : 0;
        }
        
        var deltaGainedHealth = CalculateDeltaGainedValue(maxHealth, previousPaidCost, completed);
        var deltaGainedShields = CalculateDeltaGainedValue(maxShields, previousPaidCost, completed);
        
        Updated((deltaGainedHealth, deltaGainedShields));
    }

    /// <summary>
    /// Calculates how much value of vitals should be gained depending on the amount of paid resources
    /// </summary>
    private int CalculateDeltaGainedValue(int maxValue, IList<Payment> previousPaidCost, bool completed)
    {
        var previousPaidCostSum = GetResourcesSum(previousPaidCost);
        var paidCostSum = GetResourcesSum(PaidCost);
        var totalCostSum = GetResourcesSum(NonConsumableCost);
        
        var previouslyGainedValue = (maxValue * previousPaidCostSum) / totalCostSum;
        var newGainedValue = (maxValue * paidCostSum) / totalCostSum;
        var completedOffset = completed ? 1 : 0;
        var deltaGainedValue = Math.Max(newGainedValue - previouslyGainedValue - completedOffset, 0);
        return deltaGainedValue;
    }

    private static int GetResourcesSum(IList<Payment> resources) => resources
        .Select(r => r.Amount)
        .Sum();

    /// <summary>
    /// Calculates the average efficiency factor given all the current helpers
    /// </summary>
    private float CalculateEfficiencyFactor()
    {
        var helpers = Helpers.Values
            .OrderDescending()
            .Skip(1); // First one is the main builder so it does not suffer from efficiency loss
        
        float currentProductionFactor = 1;
        float summedProductionBonus = 1;

        foreach (var helperEfficiency in helpers)
        {
            currentProductionFactor *= helperEfficiency;
            summedProductionBonus += currentProductionFactor;
        }

        return summedProductionBonus / Helpers.Count;
    }
    
    private readonly struct ProgressStep
    {
        public required IList<Payment> NewPaidSoFar { get; init; }
        public required bool Completed { get; init; }
    }
}
