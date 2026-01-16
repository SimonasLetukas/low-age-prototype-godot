using System;
using System.Collections.Generic;
using System.Linq;

public class EntityCreationProgress
{
    public event Action<int, int> Updated = delegate { };
    public event Action Completed = delegate { };
    
    public Dictionary<BuildNode, float> Helpers { get; set; } = new();
    public int TotalCost { get; set; } = 0; // TODO into resources
    public int PaidCost { get; private set; } = 0; // TODO into resources
    public required Func<int> GetMaxHp { get; set; }
    public required Func<int> GetMaxShields { get; set; }

    public void UpdateProgress(int by) // TODO into resources
    {
        if (PaidCost >= TotalCost) // TODO check if ALL resources are paid
            return;

        var efficiencyFactor = CalculateEfficiencyFactor();
        var adjustedValue = (int)Math.Ceiling(by * efficiencyFactor);
        
        var previousPaidCost = PaidCost;
        
        var remainingCost = TotalCost - PaidCost;
        var appliedCost = Math.Min(adjustedValue, remainingCost);
        PaidCost += appliedCost;
        var completed = TotalCost == PaidCost;
        
        var deltaUnlockedHp = CalculateDeltaUnlockedValue(GetMaxHp(), previousPaidCost, completed);
        var deltaUnlockedShields = CalculateDeltaUnlockedValue(GetMaxShields(), previousPaidCost, completed);
        
        Updated(deltaUnlockedHp, deltaUnlockedShields);

        if (completed)
            Completed();
    }

    /// <summary>
    /// Calculates how much value of vitals should be gained depending on the amount of paid resources
    /// </summary>
    private int CalculateDeltaUnlockedValue(int maxValue, int previousPaidCost, bool completed)
    {
        var previouslyUnlockedValue = (maxValue * previousPaidCost) / TotalCost;
        var newUnlockedValue = (maxValue * PaidCost) / TotalCost;
        var completedOffset = completed ? 1 : 0;
        var deltaUnlockedValue = Math.Max(newUnlockedValue - previouslyUnlockedValue - completedOffset, 0);
        return deltaUnlockedValue;
    }

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
}