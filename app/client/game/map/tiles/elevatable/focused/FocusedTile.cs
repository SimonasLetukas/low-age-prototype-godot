using System.Linq;
using Godot;
using LowAgeCommon;

public partial class FocusedTile : AnimatedSprite2D
{
    [Export]
    public bool DebugEnabled { get; set; } = false;
    
    public Tiles.TileInstance? CurrentTile { get; private set; }
    public bool IsWithinTheMap => CurrentTile != null;

    private Tiles _tiles = null!;
    private EntityNode? _focusedEntity = null;
    private Vector2Int _previousPosition = Vector2Int.Zero;
    private bool _stateChanged = false;

    private RichTextLabel _zIndexText = null!;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Pausable;
        Play();
        
        base._Ready();

        _zIndexText = GetNode<RichTextLabel>($"{nameof(RichTextLabel)}");
        _zIndexText.Visible = DebugEnabled;
        
        Disable();

        EventBus.Instance.WhenFlattenedChanged += OnWhenFlattenedChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        EventBus.Instance.WhenFlattenedChanged -= OnWhenFlattenedChanged;
    }

    public override void _Process(double delta)
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
        var (mapPosition, tile) = DetermineMapPositionAndTile();
        
        if (_previousPosition == mapPosition && _stateChanged is false)
            return;
        
        if (DebugEnabled)
            GD.Print($"Updating tile to position {mapPosition}, reason: new position " +
                     $"({_previousPosition != mapPosition}), state changed ({_stateChanged}).");
        
        var hoveredTerrain = _tiles.GetTerrain(tile);
        EventBus.Instance.RaiseNewTileFocused(mapPosition, hoveredTerrain, tile?.GetOccupants().ToList());
        
        MoveTo(mapPosition, GetHeight(tile), GetZIndex(tile));
        _previousPosition = mapPosition;
        _stateChanged = false;

        if (tile is null)
        {
            CurrentTile = null;
            return;
        }

        CurrentTile = tile;
    }

    private (Vector2Int, Tiles.TileInstance?) DetermineMapPositionAndTile()
    {
        var (availableTileElevation, availableTile) = _tiles.Elevatable.GetElevationAndAvailableTileAtMousePosition();

        if (availableTileElevation > 0 && availableTile != null)
            return (availableTile.Position, availableTile);

        if (_focusedEntity != null)
        {
            var entityPosition = _focusedEntity.EntityPrimaryPosition;
            return (entityPosition, _tiles.GetTile(entityPosition, 
                _focusedEntity is UnitNode { IsOnHighGround: true }));
        }
        
        if (availableTile != null)
            return (availableTile.Position, availableTile);

        var position = _tiles.GetMapPositionFromGlobalPosition(GetGlobalMousePosition());
        return (position, _tiles.GetTile(position, false));
    }

    public void FocusEntity(EntityNode entity)
    {
        if (_focusedEntity != null && _focusedEntity.Equals(entity))
            return;
        
        _focusedEntity = entity;
        _stateChanged = true;
    }

    public void StopEntityFocus()
    {
        if (_focusedEntity == null)
            return;
        
        _focusedEntity = null;
        _stateChanged = true;
    }

    public void Enable() => Visible = true;
    public void Disable() => Visible = false;
    
    private void MoveTo(Vector2Int position, int height, int zIndex)
    {
        GlobalPosition = _tiles.GetGlobalPositionFromMapPosition(position) + Vector2.Up * height;
        ZIndex = zIndex;
        _zIndexText.Text = ZIndex.ToString();
    }

    private static int GetHeight(Tiles.TileInstance? tile)
    {
        var height = tile?.YSpriteOffset ?? 0;
        height = height > 0 && ClientState.Instance.Flattened 
            ? Constants.FlattenedHighGroundHeight 
            : height;
        return height;
    }

    private int GetZIndex(Tiles.TileInstance? tile)
    {
        Tiles.TileInstance? tileBelow = null;
        if (tile != null) 
            tileBelow = _tiles.GetTile(tile.Position, false);

        return tile?.GetFirstOccupantOrNull()?.Renderer.ZIndex
               ?? tileBelow?.GetFirstOccupantOrNull()?.Renderer.ZIndex + 1
               ?? 0;
    }

    private void OnWhenFlattenedChanged(bool to) => _stateChanged = true;
}
