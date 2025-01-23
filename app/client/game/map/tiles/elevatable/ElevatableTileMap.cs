using System.Collections.Generic;
using System.Linq;
using Godot;

public class ElevatableTileMap : TileMap
{
    [Export]
    public Texture ElevatedSpriteTexture { get; set; }
    
    protected int Height { get; set; } = 0;
    protected Dictionary<Vector2, Sprite> SpritesByPosition { get; set; } = new Dictionary<Vector2, Sprite>();
    
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

    public void SetTile(Vector2 position, int index, int zIndex)
    {
        if (Height is 0)
        {
            SetCellv(position, index);
            return;
        }

        if (SpritesByPosition.ContainsKey(position))
            return;

        var sprite = new Sprite();
        sprite.Texture = ElevatedSpriteTexture;
        sprite.Position = MapToWorld(position) + Vector2.Down * Constants.TileHeight / 2;
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

        foreach (var sprite in GetChildren().OfType<Sprite>()) 
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
