using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ElevatableTileMap : TileMap
{
    [Export]
    public Texture2D ElevatedSpriteTexture { get; set; }
    
    protected int Height { get; set; } = 0;
    protected Dictionary<Vector2, Sprite2D> SpritesByPosition { get; set; } = new Dictionary<Vector2, Sprite2D>();
    
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

    public void SetTile(Vector2I position, int index, int zIndex)
    {
        if (Height is 0)
        {
            SetCell(0, position, index);
            return;
        }

        if (SpritesByPosition.ContainsKey(position))
            return;

        var sprite = new Sprite2D();
        sprite.Texture = ElevatedSpriteTexture;
        var localPosition = MapToLocal(position);
        var worldPosition = ToGlobal(localPosition);
        sprite.Position = worldPosition + Vector2.Down * Constants.TileHeight / 2;
        sprite.ZIndex = zIndex + 1;
        AddChild(sprite);
        SpritesByPosition[position] = sprite;
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
        if (flattened && Height > 0)
        {
            Position = Vector2.Up * Constants.FlattenedHighGroundHeight;
            return;
        }

        Position = Vector2.Up * Height;
    }

    private void OnWhenFlattenedChanged(bool to) => DeterminePosition(to);
}
