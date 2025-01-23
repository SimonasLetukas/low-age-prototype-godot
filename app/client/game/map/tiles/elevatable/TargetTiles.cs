using Godot;

public class TargetTiles : ElevatableTileMap
{
    [Export]
    public Texture ElevatedNegativeSpriteTexture { get; set; }
    
    public const string ScenePath = @"res://app/client/game/map/tiles/elevatable/TargetTiles.tscn";
    public static TargetTiles Instance() => (TargetTiles) GD.Load<PackedScene>(ScenePath).Instance();
    public static TargetTiles InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        instance.Clear();
        return instance;
    }
    
    public const int TileMapNegativeTargetTileIndex = 11;
    public const int TileMapPositiveTargetTileIndex = 12;
    
    public void SetTile(Vector2 position, bool isPositive, int zIndex)
    {
        SetTile(position, 
            isPositive ? TileMapPositiveTargetTileIndex : TileMapNegativeTargetTileIndex, 
            zIndex);
        
        if (Height is 0)
            return;

        if (SpritesByPosition.TryGetValue(position, out var sprite) is false)
            return;

        sprite.Texture = isPositive ? ElevatedSpriteTexture : ElevatedNegativeSpriteTexture;
    }
}
