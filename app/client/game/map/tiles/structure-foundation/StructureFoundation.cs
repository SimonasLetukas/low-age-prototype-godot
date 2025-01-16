using Godot;

// TODO: Should indicate somehow when the structure is selected / hovered
public class StructureFoundation : TileMap
{
    public const string ScenePath = @"res://app/client/game/map/tiles/structure-foundation/StructureFoundation.tscn";
    public static StructureFoundation Instance() => (StructureFoundation) GD.Load<PackedScene>(ScenePath).Instance();
    public static StructureFoundation InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        return instance;
    }
    
    private const int StructureTileIndex = 13;
    private const int WalkableTileIndex = 14;

    public override void _Ready()
    {
        base._Ready();
        Clear();
    }

    public void Initialize(StructureNode structure)
    {
        foreach (var walkablePosition in structure.WalkablePositions)
        {
            var godotWalkablePosition = walkablePosition.ToGodotVector2();
            SetCellv(godotWalkablePosition, WalkableTileIndex);
            UpdateBitmaskRegion(godotWalkablePosition);
        }
        
        if (structure.FlattenedSprite != null && structure.FlattenedCenterOffset != null)
        {
            return;
        }

        foreach (var position in structure.NonWalkablePositions)
        {
            var godotPosition = position.ToGodotVector2();
            SetCellv(godotPosition, StructureTileIndex);
            UpdateBitmaskRegion(godotPosition);
        }
    }
}
