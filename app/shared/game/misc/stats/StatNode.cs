using System;
using Godot;
using LowAgeData.Domain.Common;

public partial class StatNode : Node2D, INodeFromBlueprint<Stat>
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    private Stat Blueprint { get; set; } = null!;

    public bool HasCurrent => Blueprint.HasCurrent;
    
    private float _currentAmount;
    public float CurrentAmount
    {
        get => HasCurrent ? _currentAmount : MaxAmount;
        set => SetCurrentValue(ref _currentAmount, ref _maxAmount, value, HasCurrent);
    }

    private int _maxAmount;
    public int MaxAmount
    {
        get => _maxAmount;
        set => SetMaxValue(ref _currentAmount, ref _maxAmount, value);
    }

    public void SetBlueprint(Stat blueprint)
    {
        Blueprint = blueprint;
        _currentAmount = Blueprint.MaxAmount;
        _maxAmount = Blueprint.MaxAmount;
    }

    protected static void SetCurrentValue(ref float currentProperty, ref int maxProperty, 
        float value, bool hasCurrent)
    {
        if (hasCurrent)
        {
            currentProperty = value;
            return;
        }

        maxProperty = (int)value;
    }
    
    protected static void SetMaxValue(ref float currentProperty, ref int maxProperty, 
        float value)
    {
        if (currentProperty > maxProperty) 
            currentProperty = value;

        maxProperty = (int)value;
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}