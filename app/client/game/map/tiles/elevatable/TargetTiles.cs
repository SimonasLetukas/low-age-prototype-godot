using System.Collections.Generic;
using Godot;

public partial class TargetTiles : ElevatableTileMap
{
    [Export]
    public Texture2D ElevatedNegativeSpriteTexture { get; set; }
    
    public const string ScenePath = @"res://app/client/game/map/tiles/elevatable/TargetTiles.tscn";
    public static TargetTiles Instance() => (TargetTiles) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static TargetTiles InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        instance.Clear();
        return instance;
    }

    public static readonly Vector2I TileMapPositiveTargetAtlasPosition = new(12, 3);
    public static readonly Vector2I TileMapNegativeTargetAtlasPosition = new(13, 3);
    private const int TerrainSet = 0;
    private const int TileMapPositiveTargetTerrainIndex = 7;
    private const int TileMapNegativeTargetTerrainIndex = 8;
    
    public void SetTiles(IList<(Vector2I, int)> positionsAndZIndexes, bool isPositive)
    {
        SetTiles(positionsAndZIndexes, 
            TerrainSet,
            isPositive 
                ? TileMapPositiveTargetTerrainIndex 
                : TileMapNegativeTargetTerrainIndex);
        
        if (Height is 0)
            return;

        foreach (var (position, _) in positionsAndZIndexes)
        {
            if (SpritesByPosition.TryGetValue(position, out var sprite) is false)
                continue;

            sprite.Texture = isPositive ? ElevatedSpriteTexture : ElevatedNegativeSpriteTexture;
        }
    }
}
