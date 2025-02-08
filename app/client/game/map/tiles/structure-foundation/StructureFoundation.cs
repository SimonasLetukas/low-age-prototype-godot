using System.Linq;
using Godot;

// TODO: Should indicate somehow when the structure is selected / hovered
public partial class StructureFoundation : TileMapLayer
{
    public const string ScenePath = @"res://app/client/game/map/tiles/structure-foundation/StructureFoundation.tscn";
    public static StructureFoundation Instance() => (StructureFoundation) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static StructureFoundation InstantiateAsChild(Node parentNode)
    {
        var instance = Instance();
        parentNode.AddChild(instance);
        return instance;
    }

    private const int FoundationTerrainSetIndex = 0;
    private const int FoundationTerrainIndex = 6;
    
    private const int WalkableFoundationTerrainSetIndex = 1;
    private const int WalkableFoundationTerrainIndex = 1;

    public override void _Ready()
    {
        base._Ready();
        Clear();
    }

    public void Initialize(StructureNode structure)
    {
        SetCellsTerrainConnect(structure.WalkablePositions
                .Select(x => x.ToGodotVector2I<int>())
                .ToGodotArray(), 
            WalkableFoundationTerrainSetIndex, 
            WalkableFoundationTerrainIndex,
            false);
        
        if (structure.FlattenedSprite != null && structure.FlattenedCenterOffset != null)
        {
            return;
        }
        
        SetCellsTerrainConnect(structure.NonWalkablePositions
                .Select(x => x.ToGodotVector2I<int>())
                .ToGodotArray(), 
            FoundationTerrainSetIndex, 
            FoundationTerrainIndex);
    }
}
