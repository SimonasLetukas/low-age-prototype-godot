using Godot;

public partial class AvailableHoveringTiles : ElevatableTileMap
{
    public const string ScenePath = @"res://app/client/game/map/tiles/elevatable/AvailableHoveringTiles.tscn";
    public static AvailableHoveringTiles Instance() => (AvailableHoveringTiles) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static AvailableHoveringTiles InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        instance.Clear();
        return instance;
    }
}
