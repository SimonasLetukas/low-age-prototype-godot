using System;
using Godot;
using LowAgeData.Domain.Common;

public partial class StatNode : Node2D, INodeFromBlueprint<Stat>
{
    public const string ScenePath = @"res://app/shared/game/misc/StatNode.tscn";
    public static StatNode Instance() => (StatNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static StatNode InstantiateAsChild(Stat blueprint, Node parentNode)
    {
        var stat = Instance();
        parentNode.AddChild(stat);
        stat.SetBlueprint(blueprint);
        return stat;
    }

    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public Stat Blueprint { get; private set; }
    public float CurrentValue { get; set; }

    public void SetBlueprint(Stat blueprint)
    {
        Blueprint = blueprint;
        CurrentValue = Blueprint.MaxAmount;
    }
}