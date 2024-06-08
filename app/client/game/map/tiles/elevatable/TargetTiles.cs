using Godot;

public class TargetTiles : ElevatableTileMap
{
    public const string ScenePath = @"res://app/client/game/map/tiles/elevatable/TargetTiles.tscn";
    public static TargetTiles Instance() => (TargetTiles) GD.Load<PackedScene>(ScenePath).Instance();
    public static TargetTiles InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        instance.Clear();
        return instance;
    }
}
