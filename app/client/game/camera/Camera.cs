using Godot;
using System;

public class Camera : Camera2D
{
    [Export] public bool DebugEnabled { get; set; } = true;
    [Export(PropertyHint.Range, "-200,100,5")] public int HorizontalLimitMargin { get; set; } = 30;
    [Export(PropertyHint.Range, "-200,100,5")] public int VerticalLimitMargin { get; set; } = 5;
    [Export(PropertyHint.Range, "0.1,0.5,0.1")] public float MinimumZoom { get; set; } = 0.2f;
    [Export(PropertyHint.Range, "0.1,0.5,0.1")] public float MaximumZoom { get; set; } = 0.5f;
    [Export(PropertyHint.Range, "1,10,1")] public int MapScrollMargin { get; set; } = 5;
    [Export] public bool MapScrollOnBoundaryEnabled { get; set; } = false;
    [Export(PropertyHint.Range, "100,3000,100")] public int MapScrollSpeed { get; set; } = 1800;
    [Export(PropertyHint.Range, "1,10,1")] public int MapLimitElasticity { get; set; } = 6;
    
    private int _mapWidthPixels = 1;
    private int _mapHeightPixels = 2;
    private int _limitLeft;
    private int _limitRight;
    private int _limitTop;
    private int _limitBottom;
    private Vector2 _viewportSize;
    private bool _cameraIsMoving = false;

    public override void _Ready()
    {
        _viewportSize = GetViewport().Size;

        Position = new Vector2((float)_mapWidthPixels / 2, (float)_mapHeightPixels / 2);
        Zoom = new Vector2(MaximumZoom, MinimumZoom);

        if (DebugEnabled)
        {
            GD.Print($"{nameof(Camera)}: map width '{_mapWidthPixels}'");
            GD.Print($"{nameof(Camera)}: map height '{_mapHeightPixels}'");
            GD.Print($"{nameof(Camera)}: camera position '{Position}'");
        }

        SetLimits();
    }

    public override void _Process(float delta)
    {
        if (ZoomedIn()) ZoomIn();
        if (ZoomedOut()) ZoomOut(delta);

        var moveVector = Vector2.Zero;
        var mousePos = GetViewport().GetMousePosition();

        if (_cameraIsMoving)
        {
            return;
        }
        
        ClampPositionToBoundaries(delta);

        if (MovedLeft(mousePos)) moveVector.x -= 1;
        if (MovedRight(mousePos)) moveVector.x += 1;
        if (MovedUp(mousePos)) moveVector.y -= 1;
        if (MovedDown(mousePos)) moveVector.y += 1;
            
        GlobalTranslate(moveVector.Normalized() * delta * Zoom.x * MapScrollSpeed);
    }

    public void OnCreatorMapSizeDeclared(Vector2 mapSize)
    {
        _mapWidthPixels = Mathf.Max((int)mapSize.x, (int)mapSize.y) * Constants.TileWidth;
        _mapHeightPixels = Mathf.Max((int)mapSize.x, (int)mapSize.y) * Constants.TileHeight;
        SetLimits();
    }

    public void OnMouseDragged(Vector2 by) => Position += by * Zoom;

    public void OnMouseTakingControl(bool flag) => _cameraIsMoving = flag;

    private void SetLimits()
    {
        _limitLeft = HorizontalLimitMargin * -1;
        _limitRight = _mapWidthPixels + HorizontalLimitMargin;
        _limitTop = (VerticalLimitMargin * -1) - 25;
        _limitBottom = _mapHeightPixels + VerticalLimitMargin;
    }
    
    private bool ZoomedIn()
    {
        if (Zoom.x >= MinimumZoom) return false;
        if (Input.IsActionJustReleased("ui_zoom_in") is false) return false;
        return true;
    }

    private bool ZoomedOut()
    {
        if (Zoom.x >= MaximumZoom) return false;
        if (Input.IsActionJustReleased("ui_zoom_out") is false) return false;
        return true;
    }

    private void ZoomIn()
    {
        Zoom = Math.Abs(Mathf.Stepify(Zoom.x, 0.1f) - 0.4f) < 0.01f 
            ? new Vector2(Zoom.x - 0.2f, Zoom.y - 0.2f) 
            : new Vector2(Zoom.x - 0.1f, Zoom.y - 0.1f);
        _viewportSize = GetViewport().Size;
    }

    private void ZoomOut(float delta)
    {
        Zoom = Math.Abs(Mathf.Stepify(Zoom.x, 0.1f) - 0.2f) < 0.01f 
            ? new Vector2(Zoom.x + 0.2f, Zoom.y + 0.2f) 
            : new Vector2(Zoom.x + 0.1f, Zoom.y + 0.1f);
        _viewportSize = GetViewport().Size;
        ClampPositionToBoundaries(delta);
    }

    private void ClampPositionToBoundaries(float delta)
    {
        var newPosition = new Vector2(Position.x, Position.y);
        
        if (GetCurrentLeftBoundary() < _limitLeft) 
            newPosition.x += (_limitLeft - GetCurrentLeftBoundary()) * delta * MapLimitElasticity;
        else if (GetCurrentRightBoundary() > _limitRight)
            newPosition.x -= (GetCurrentRightBoundary() - _limitRight) * delta * MapLimitElasticity;
        
        if (GetCurrentTopBoundary() < _limitTop)
            newPosition.y += (_limitTop - GetCurrentTopBoundary()) * delta * MapLimitElasticity;
        else if (GetCurrentBottomBoundary() > _limitBottom)
            newPosition.y -= (GetCurrentBottomBoundary() - _limitBottom) * delta * MapLimitElasticity;

        Position = newPosition;
    }

    private float GetCurrentLeftBoundary() => Position.x - ((_viewportSize.x / 2) * Zoom.x);

    private float GetCurrentRightBoundary() => Position.x + ((_viewportSize.x / 2) * Zoom.x);

    private float GetCurrentTopBoundary() => Position.y - ((_viewportSize.y / 2) * Zoom.y);

    private float GetCurrentBottomBoundary() => Position.y + ((_viewportSize.y / 2) * Zoom.y);

    private bool MovedLeft(Vector2 mousePos)
    {
        if (GetCurrentLeftBoundary() >= _limitLeft) return false;
        if (Input.IsActionPressed("ui_left")) return true;
        if (mousePos.x < MapScrollMargin && MapScrollOnBoundaryEnabled) return true;
        return false;
    }
    
    private bool MovedRight(Vector2 mousePos)
    {
        if (GetCurrentRightBoundary() >= _limitRight) return false;
        if (Input.IsActionPressed("ui_right")) return true;
        if (mousePos.x > _viewportSize.x - MapScrollMargin && MapScrollOnBoundaryEnabled) return true;
        return false;
    }
    
    private bool MovedUp(Vector2 mousePos)
    {
        if (GetCurrentTopBoundary() >= _limitTop) return false;
        if (Input.IsActionPressed("ui_up")) return true;
        if (mousePos.y < MapScrollMargin && MapScrollOnBoundaryEnabled) return true;
        return false;
    }
    
    private bool MovedDown(Vector2 mousePos)
    {
        if (GetCurrentBottomBoundary() >= _limitBottom) return false;
        if (Input.IsActionPressed("ui_down")) return true;
        if (mousePos.y > _viewportSize.y - MapScrollMargin && MapScrollOnBoundaryEnabled) return true;
        return false;
    }
}
