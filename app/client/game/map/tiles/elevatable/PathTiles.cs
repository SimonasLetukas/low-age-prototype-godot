using Godot;

public class PathTiles : ElevatableTileMap
{
    public const string ScenePath = @"res://app/client/game/map/tiles/elevatable/PathTiles.tscn";
    public static PathTiles Instance() => (PathTiles) GD.Load<PackedScene>(ScenePath).Instance();
    public static PathTiles InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        instance.Clear();
        return instance;
    }
}