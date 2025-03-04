using System.Collections.Generic;
using Godot;

public partial class TargetTiles : ElevatableTileMap
{
    [Export] 
    public Texture2D ElevatedHoveringSpriteTexture { get; set; } = null!;
    
    public const string ScenePath = @"res://app/client/game/map/tiles/elevatable/TargetTiles.tscn";
    public static TargetTiles Instance() => (TargetTiles) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static TargetTiles InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        instance.Clear();
        return instance;
    }
    
    public static Vector2I TileMapNormalTargetAtlasPosition => new(12, 3);
    public static Vector2I TileMapHoveringTargetAtlasPosition => new(13, 3);
    
    private const int TerrainSet = 0;
    private const int TileMapNormalTargetTerrainIndex = 7;
    private const int TileMapNormalHoveringTargetTerrainIndex = 8;
    private const int TileMapMeleeTargetTerrainIndex = 11;
    private const int TileMapMeleeHoveringTargetTerrainIndex = 12;
    
    public void SetTiles(IList<(Vector2I, int)> positionsAndZIndexes, bool hovering, bool isMelee)
    {
        var terrain = hovering switch
        {
            false when isMelee => TileMapMeleeTargetTerrainIndex,
            false when isMelee is false => TileMapNormalTargetTerrainIndex,
            true when isMelee => TileMapMeleeHoveringTargetTerrainIndex,
            true when isMelee is false => TileMapNormalHoveringTargetTerrainIndex,
            _ => TileMapNormalTargetTerrainIndex
        };
        
        SetTiles(positionsAndZIndexes, TerrainSet, terrain);
        
        if (Height is 0)
            return;

        foreach (var (position, _) in positionsAndZIndexes)
        {
            if (SpritesByPosition.TryGetValue(position, out var sprite) is false)
                continue;

            // Melee is not checked here because it is configured through the editor by having different sprites.
            sprite.Texture = hovering ? ElevatedHoveringSpriteTexture : ElevatedSpriteTexture;
        }
    }
}
