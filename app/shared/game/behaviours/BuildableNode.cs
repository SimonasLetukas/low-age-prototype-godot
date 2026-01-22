using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Behaviours;

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
    
    public Dictionary<BuildNode, float> Helpers { get; set; } = new();
    public int TotalCost { get; set; } = 0; // TODO into resources
    public int PaidCost { get; private set; } = 0; // TODO into resources

    private Buildable Blueprint { get; set; } = null!;
    
    public void SetBlueprint(Buildable blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
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

    public int GetMaximumPotentialAppliedIncome(int income) // TODO into resources
    {
        var efficiencyFactor = CalculateEfficiencyFactor();
        GD.Print($"Income '{income}', efficiency factor '{efficiencyFactor}'");
        return ((int)Math.Ceiling(income * efficiencyFactor)) * Helpers.Count;
    }
    
    public int GetRemainingUpdateCount(int income) // TODO into resources
    {
        if (PaidCost >= TotalCost) // TODO check if ALL resources are paid
            return 0;

        if (Helpers.Count <= 0)
            return int.MaxValue;

        var simulatedPaidCost = PaidCost;
        var counter = 0;

        while (simulatedPaidCost < TotalCost)
        {
            for (var i = 0; i < Helpers.Count; i++)
            {
                var progress = CalculateProgressStep(simulatedPaidCost, income);
                simulatedPaidCost = progress.NewPaidCost;
            }
            
            counter++;
        }

        return counter;
    }

    public void UpdateDescription(int potentialIncome) // TODO into resources, could be fetched from GlobalRegistry
    {
        var helperText = Helpers.Count == 1 ? "1 helper is" : $"{Helpers.Count} helpers are";
        var remainingUpdateCount = GetRemainingUpdateCount(potentialIncome);
        var remainingUpdateText = remainingUpdateCount == int.MaxValue ? "too many" : remainingUpdateCount.ToString();
        
        Description = $"{helperText} currently working to finish this in " +
                      $"{remainingUpdateText} turns (with a combined production of " +
                      $"{GetMaximumPotentialAppliedIncome(potentialIncome)}, which is used to cover " +
                      $"the remaining {TotalCost - PaidCost} production).\n\n" + Blueprint.Description;
    }
    
    public void UpdateProgress(int income) // TODO into resources
    {
        UpdateDescription(income);

        if (PaidCost >= TotalCost) // TODO check if ALL resources are paid
        {
            Completed();
            return;
        }

        var previousPaidCost = PaidCost;
        var progress = CalculateProgressStep(PaidCost, income);
        PaidCost = progress.NewPaidCost;

        UpdateVitals(previousPaidCost, progress.Completed);

        if (progress.Completed)
            Completed();
    }
    
    private ProgressStep CalculateProgressStep(int currentPaidCost, int income)
    {
        if (currentPaidCost >= TotalCost)
            return new ProgressStep
            {
                NewPaidCost = currentPaidCost,
                Completed = true
            };

        var efficiencyFactor = CalculateEfficiencyFactor();
        var adjustedValue = (int)Math.Ceiling(income * efficiencyFactor);

        var remainingCost = TotalCost - currentPaidCost;
        var appliedCost = Math.Min(adjustedValue, remainingCost);
        var newPaidCost = currentPaidCost + appliedCost;

        return new ProgressStep
        {
            NewPaidCost = newPaidCost,
            Completed = newPaidCost == TotalCost,
        };
    }

    private void UpdateVitals(int previousPaidCost, bool completed)
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
        
        GD.Print($"Sending vitals update to {Parent.DisplayName}: health delta '{deltaGainedHealth}', " +
                 $"shields delta '{deltaGainedShields}'");
        
        Updated((deltaGainedHealth, deltaGainedShields));
    }

    /// <summary>
    /// Calculates how much value of vitals should be gained depending on the amount of paid resources
    /// </summary>
    private int CalculateDeltaGainedValue(int maxValue, int previousPaidCost, bool completed)
    {
        var previouslyGainedValue = (maxValue * previousPaidCost) / TotalCost;
        var newGainedValue = (maxValue * PaidCost) / TotalCost;
        var completedOffset = completed ? 1 : 0;
        var deltaGainedValue = Math.Max(newGainedValue - previouslyGainedValue - completedOffset, 0);
        return deltaGainedValue;
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
    
    private readonly struct ProgressStep
    {
        public required int NewPaidCost { get; init; }
        public required bool Completed { get; init; }
    }
}
