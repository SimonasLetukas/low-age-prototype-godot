using Godot;

public class AvailableTiles : ElevatableTileMap
{
    public const string ScenePath = @"res://app/client/game/map/tiles/elevatable/AvailableTiles.tscn";
    public static AvailableTiles Instance() => (AvailableTiles) GD.Load<PackedScene>(ScenePath).Instance();
    public static AvailableTiles InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        instance.Clear();
        return instance;
    }
}
