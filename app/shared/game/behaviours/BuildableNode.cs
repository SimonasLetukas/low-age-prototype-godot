using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Common;

public class BuildableNode : BehaviourNode, INodeFromBlueprint<Buildable>
{
    public const string ScenePath = @"res://app/shared/game/behaviours/BuildableNode.tscn";
    public static BuildableNode Instance() => (BuildableNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static BuildableNode InstantiateAsChild(Buildable blueprint, Node parentNode)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }
    
    private Buildable Blueprint { get; set; }
    
    public void SetBlueprint(Buildable blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }

    public bool IsPlacementValid(IList<Tiles.TileInstance> tiles)
    {
        if (tiles.Any(x => x.Terrain.Equals(Terrain.Mountains)))
            return false;
        
        // TODO add rules according to data
        
        return true;
    }
}
