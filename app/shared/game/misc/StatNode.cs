using Godot;
using low_age_data.Domain.Shared;

public class StatNode : Node2D
{
    public const string ScenePath = @"res://app/shared/game/misc/StatNode.tscn";
    public Stat Blueprint { get; private set; }
    public float CurrentValue { get; set; }

    public void SetBlueprint(Stat blueprint)
    {
        Blueprint = blueprint;
        CurrentValue = Blueprint.MaxAmount;
    }
}
