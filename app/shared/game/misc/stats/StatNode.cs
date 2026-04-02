using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Modifications;

public abstract partial class StatNode : Node2D, INodeFromBlueprint<Stat>
{
    public event Action<StatNode> Updated = delegate { };
    
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    
    protected List<Modification> Modifications { get; } = [];
    
    private Stat Blueprint { get; set; } = null!;
    private bool HasCurrent => Blueprint.HasCurrent;

    public float CurrentAmount => HasCurrent ? GetCurrent() : GetMax();
    public int MaxAmount => GetMax();

    protected float BaseCurrentAmount { get; set; }
    protected int BaseMaxAmount { get; set; }

    public void SetBlueprint(Stat blueprint)
    {
        Blueprint = blueprint;
        BaseCurrentAmount = Blueprint.MaxAmount;
        BaseMaxAmount = Blueprint.MaxAmount;
    }

    public virtual void Apply(Change what, float amount) => Updated(this);

    public virtual void Apply(Modification modification, bool applyToBaseValue) => Updated(this);

    public virtual void Remove(Modification modification) => Updated(this);

    protected virtual float GetCurrent() => GetModified(BaseCurrentAmount, BaseMaxAmount, Modifications).NewCurrent;
    
    protected virtual int GetMax() => GetModified(BaseCurrentAmount, BaseMaxAmount, Modifications).NewMax;

    protected static (float NewCurrent, int NewMax) GetUpdated(Change what, float amount, float currentAmount,
        float maxAmount)
    {
        var (newCurrent, newMax) = what switch
        {
            _ when what.Equals(Change.AddMax) => (currentAmount, maxAmount + amount),
            _ when what.Equals(Change.AddCurrent) => (currentAmount + amount, maxAmount),
            _ when what.Equals(Change.SubtractMax) => (currentAmount, maxAmount - amount),
            _ when what.Equals(Change.SubtractCurrent) => (currentAmount - amount, maxAmount),
            _ when what.Equals(Change.SetMax) => (currentAmount, amount),
            _ when what.Equals(Change.SetCurrent) => (amount, maxAmount),
            _ when what.Equals(Change.MultiplyMax) => (currentAmount, maxAmount * amount),
            _ when what.Equals(Change.MultiplyCurrent) => (currentAmount * amount, maxAmount),
            _ => (currentAmount, maxAmount),
        };
        
        return (newCurrent, (int)newMax);
    }

    protected (float NewCurrent, int NewMax) GetModified(float currentAmount, int maxAmount, 
        IEnumerable<Modification> modifications)
    {
        foreach (var modification in modifications
                     .OrderByDescending(m => m.Change.DoesSet())
                     .ThenByDescending(m => m.Change.DoesAdd() || m.Change.DoesSubtract())
                     .ThenByDescending(m => m.Change.DoesMultiply()))
        {
            (currentAmount, maxAmount) = GetUpdated(modification.Change, modification.Amount, currentAmount, maxAmount);
        }

        if (Blueprint.AllowsOverflow is false)
            currentAmount = Math.Min(currentAmount, maxAmount);
        
        return (currentAmount, maxAmount);
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}