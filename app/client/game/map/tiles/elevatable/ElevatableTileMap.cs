using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ElevatableTileMap : TileMapLayer
{
    [Export]
    public Texture2D ElevatedSpriteTexture { get; set; } = null!;

    protected int Height { get; private set; } = 0;
    protected Dictionary<Vector2, Sprite2D> SpritesByPosition { get; set; } = new();
    
    public override void _Ready()
    {
        base._Ready();

        EventBus.Instance.WhenFlattenedChanged += OnWhenFlattenedChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        EventBus.Instance.WhenFlattenedChanged -= OnWhenFlattenedChanged;
    }

    public void SetTiles(IEnumerable<(Vector2I, int)> positionsAndZIndexes, int terrainSet, int terrain)
    {
        if (Height is 0)
        {
            SetCellsTerrainConnect(positionsAndZIndexes.Select(x => x.Item1).ToGodotArray(), 
                terrainSet, terrain);
            return;
        }
        
        foreach (var (position, zIndex) in positionsAndZIndexes)
        {
            if (SpritesByPosition.ContainsKey(position))
                continue;

            var sprite = new Sprite2D();
            sprite.Texture = ElevatedSpriteTexture;
            var localPosition = MapToLocal(position);
            sprite.Position = localPosition + Vector2.Down * Constants.TileHeight / 2;
            sprite.ZIndex = zIndex + 1;
            AddChild(sprite);
            SpritesByPosition[position] = sprite;
        }
    }

    public new void Clear()
    {
        if (Height is 0)
        {
            base.Clear();
            return;
        }

        foreach (var sprite in GetChildren().OfType<Sprite2D>()) 
            sprite.QueueFree();
        SpritesByPosition.Clear();
    }

    public void SetTileZIndex(Vector2 at, int to)
    {
        if (Height is 0)
            return;

        if (SpritesByPosition.TryGetValue(at, out var sprite) is false)
            return;

        sprite.ZIndex = to + 1;
    }

    public void SetElevation(int height)
    {
        Height = height;
        DeterminePosition(ClientState.Instance.Flattened);
    }
    
    private void DeterminePosition(bool flattened)
    {
        var offset = Height == 0 ? Vector2.Zero : new Vector2(0, -(Constants.TileHeight / 2));
        
        if (flattened && Height > 0)
        {
            Position = Vector2.Up * Constants.FlattenedHighGroundHeight + offset;
            return;
        }

        Position = Vector2.Up * Height + offset;
    }

    private void OnWhenFlattenedChanged(bool to) => DeterminePosition(to);
}
