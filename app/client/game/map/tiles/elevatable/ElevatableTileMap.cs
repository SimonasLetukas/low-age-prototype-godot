using Godot;

public class ElevatableTileMap : TileMap
{
    protected int Height { get; set; } = 0;
    
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
