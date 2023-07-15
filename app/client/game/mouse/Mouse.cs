using Godot;

/// <summary>
/// Mouse actions by priority:
/// 1. SELECT (UI, unit, ability): LEFT-UP
/// 2. CANCEL / ESCAPE (UI, unit, ability): RIGHT-UP UI; LEFT-UP (when nothing to select)
/// 3. EXECUTE (movement, attack, ability): RIGHT-UP
/// 4. DRAG CAMERA: LEFT-DOWN (initiates) -> LEFT-HOLD (disables SELECT and CANCEL)
/// 5. EXAMINE / ALT-SELECT (statistics, area) - RIGHT-DOWN (initiates) -> 
///    RIGHT-HOLD (disables EXECUTE) or CTRL-HOLD
/// </summary>
public class Mouse : Node2D
{
    [Export(PropertyHint.File, "*.png")] public string ArrowCursorFileLocation { get; set; }
    [Export(PropertyHint.File, "*.png")] public string GrabCursorFileLocation { get; set; }
    [Export(PropertyHint.Range, "0.01,15")] public float MinimumMouseDistanceToStartDrag { get; set; } = 8f;
    
    [Signal] public delegate void MouseDragged(Vector2 by);
    [Signal] public delegate void TakingControl(bool flag);
    [Signal] public delegate void LeftReleasedWithoutDrag();
    [Signal] public delegate void RightReleasedWithoutExamine();

    private Resource _arrowCursor;
    private Resource _grabCursor;
    private bool _cameraIsMoving = false;
    private bool _mouseIsOnUi = false;
    private Vector2 _previousPosition = Vector2.Zero;
    private Vector2 _startPosition = Vector2.Zero;

    public override void _Ready()
    {
        _arrowCursor = GD.Load(ArrowCursorFileLocation);
        _grabCursor = GD.Load(GrabCursorFileLocation);
        
        SetCursorToArrow();
    }

    public override void _Process(float delta)
    {
        var mousePos = GetViewport().GetMousePosition();

        if (Input.IsActionJustPressed("mouse_left"))
        {
	        _startPosition = mousePos;
        }
        
        if (Input.IsActionPressed("mouse_left"))
        {
	        if (_cameraIsMoving)
	        {
		        // TODO test with map if the new logic works
		        /*var travelVector = _previousPosition - _startPosition;
		        if (travelVector.Length() > MinimumMouseDistanceToStartDrag)
		        {*/
			        var changeVector = _previousPosition - mousePos;
			        EmitSignal(nameof(MouseDragged), changeVector);
			        SetCursorToGrab();
			        _previousPosition = mousePos;
		        /*}
		        else
		        {
			        _previousPosition = mousePos + travelVector;
		        }*/
	        }
	        else
	        {
		        var initiationVector = mousePos - _startPosition;
		        if (_mouseIsOnUi is false && initiationVector.Length() > MinimumMouseDistanceToStartDrag)
		        {
			        _previousPosition = mousePos;
			        //_startPosition = mousePos;
			        _cameraIsMoving = true;
			        EmitSignal(nameof(TakingControl), true);
		        }
	        }
        }

        if (Input.IsActionJustReleased("mouse_left"))
        {
	        _cameraIsMoving = false;
	        SetCursorToArrow();
	        EmitSignal(nameof(TakingControl), false);
	        var travelVector = _previousPosition - _startPosition;
	        if (travelVector.Length() <= MinimumMouseDistanceToStartDrag)
	        {
		        EmitSignal(nameof(LeftReleasedWithoutDrag));
	        }
        }

        if (Input.IsActionJustPressed("mouse_right"))
        {
	        // TODO examine functionality etc
	        // Track holding of key
        }

        if (Input.IsActionJustReleased("mouse_right"))
        {
	        EmitSignal(nameof(RightReleasedWithoutExamine));
        }
    }
    
    internal void OnInterfaceMouseEntered() => _mouseIsOnUi = true;

    internal void OnInterfaceMouseExited() => _mouseIsOnUi = false;

    private void SetCursorToArrow() => Input.SetCustomMouseCursor(_arrowCursor);

    private void SetCursorToGrab() => Input.SetCustomMouseCursor(_grabCursor);
}
