using System.Linq;
using Godot;

public class FocusedTile : AnimatedSprite
{
    [Export]
    public bool DebugEnabled { get; set; } = false;
    
    public Tiles.TileInstance CurrentTile { get; private set; }
    public bool IsWithinTheMap => CurrentTile != null;

    private Tiles _tiles;
    private EntityNode _focusedEntity = null;
    private Vector2 _previousPosition = Vector2.Zero;
    private bool _stateChanged = false;

    private RichTextLabel _zIndexText;

    public override void _Ready()
    {
        base._Ready();

        _zIndexText = GetNode<RichTextLabel>($"{nameof(RichTextLabel)}");
        _zIndexText.Visible = DebugEnabled;
        
        Disable();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        UpdateTile();
    }

    public void Initialize(Tiles tiles)
    {
        _tiles = tiles;
        
        Enable();
    }

    public void UpdateTile()
    {
        if (Visible is false)
            return;

        var availableTileAtMousePosition = _tiles.Elevatable.GetAvailableTileAtMousePosition();
        var mapPosition = availableTileAtMousePosition?.Position 
                          ?? _focusedEntity?.EntityPrimaryPosition 
                          ?? _tiles.GetMapPositionFromGlobalPosition(GetGlobalMousePosition());
        
        if (_previousPosition == mapPosition && _stateChanged is false)
            return;

        var tile = availableTileAtMousePosition 
                   ?? _tiles.GetTile(mapPosition, _focusedEntity is UnitNode unit && unit.IsOnHighGround);
        
        var hoveredTerrain = _tiles.GetTerrain(tile);
        EventBus.Instance.RaiseNewTileFocused(mapPosition, hoveredTerrain, tile?.Occupants);

        Tiles.TileInstance tileBelow = null;
        if (tile != null) 
            tileBelow = _tiles.GetTile(tile.Position, false);

        var zIndex = tile?.Occupants.FirstOrDefault()?.Renderer.ZIndex
                     ?? tileBelow?.Occupants.FirstOrDefault()?.Renderer.ZIndex + 1
                     ?? 0;
        
        MoveTo(mapPosition, tile?.Point.YSpriteOffset ?? 0, zIndex);
        _previousPosition = mapPosition;
        _stateChanged = false;

        if (tile is null)
        {
            CurrentTile = null;
            return;
        }

        CurrentTile = tile;
    }

    public void FocusEntity(EntityNode entity)
    {
        _focusedEntity = entity;
        _stateChanged = true;
    }

    public void StopEntityFocus()
    {
        _focusedEntity = null;
        _stateChanged = true;
    }

    public void Enable() => Visible = true;
    public void Disable() => Visible = false;
    
    private void MoveTo(Vector2 position, int height, int zIndex)
    {
        Enable();
        GlobalPosition = _tiles.GetGlobalPositionFromMapPosition(position) + Vector2.Up * height;
        ZIndex = zIndex;
        _zIndexText.Text = ZIndex.ToString();
    }
}
