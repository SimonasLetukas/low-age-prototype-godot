using System.Collections.Generic;
using Godot;
using LowAgeData.Domain.Behaviours;

public partial class BuildableNode : BehaviourNode, INodeFromBlueprint<Buildable>
{
    public const string ScenePath = @"res://app/shared/game/behaviours/BuildableNode.tscn";
    public static BuildableNode Instance() => (BuildableNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static BuildableNode InstantiateAsChild(Buildable blueprint, Node parentNode)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }

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
}
